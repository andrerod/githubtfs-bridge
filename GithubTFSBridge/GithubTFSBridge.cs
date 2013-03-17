﻿// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GithubClient;
using GithubClient.Model;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace GithubTFSBridge
{
    public class GithubTFSBridge
    {
        private IGithubServiceManagement GithubChannel { get; set; }
        private string GithubOwner { get; set; }
        private string GithubRepository { get; set; }
        private string TfsUsername { get; set; }
        private string TfsPassword { get; set; }
        private string TfsDomain { get; set; }
        private string TfsServerAddress { get; set; }
        private string TfsProjectName { get; set; }
        private string TfsPath { get; set; }

        private const string WorkItemPriorityColor = "FF0000";
        private const string WorkItemIssueTypeColor = "DFDFDF";
        private const string WorkItemWorkStatusColor = "0000FF";

        private IDictionary<string, GithubUser> GithubUsers { get; set; }
        private IDictionary<string, GithubMilestone> GithubMilestones { get; set; } 

        public GithubTFSBridge (string githubUsername,
            string githubPassword,
            string githubOwner,
            string githubRepository,
            string tfsUsername,
            string tfsPassword,
            string tfsDomain,
            string tfsServerAddress,
            string tfsProjectName,
            string tfsPath)
        {
            GithubOwner = githubOwner;
            GithubRepository = githubRepository;
            TfsUsername = tfsUsername;
            TfsPassword = tfsPassword;
            TfsDomain = tfsDomain;
            TfsServerAddress = tfsServerAddress;
            TfsProjectName = tfsProjectName;
            TfsPath = tfsPath;

            GithubChannel = GithubClient.GithubClient.CreateChannel(githubUsername, githubPassword);

            GithubUsers = new Dictionary<string, GithubUser>();
            GithubMilestones = new Dictionary<string, GithubMilestone>();
        }

        public void Synchronize()
        {
            // Get All Team Projects
            IList<WorkItem> workItems = new List<WorkItem>(GetWorkItems("Shared Queries/Kudu/Portal - Kudu Future").Cast<WorkItem>());
            IList<GithubIssue> githubIssues = GithubChannel.GetIssuesFromRepo(GithubOwner, GithubRepository);
            
            // Create new github issues from work items
            foreach (WorkItem workItem in workItems)
            {
                githubIssues.Add(CreateOrUpdateGithubIssue(githubIssues, workItem));
            }
            
            // Create new work items from git issues
            foreach (GithubIssue githubIssue in githubIssues)
            {
                workItems.Add(CreateOrUpdateWorkItem(workItems, githubIssue));
            }
        }

        private WorkItemCollection GetWorkItems(string path)
        {
            var credentials = new NetworkCredential(TfsUsername, TfsPassword, TfsDomain);

            var tfs = new TfsTeamProjectCollection(new Uri(TfsServerAddress), credentials);
            tfs.EnsureAuthenticated();
            var wiStore = tfs.GetService<WorkItemStore>();

            return wiStore.Query("SELECT * FROM WorkItems WHERE [System.TeamProject] = @project".Replace("@project", "'" + TfsProjectName + "'"));
        }

        private GithubIssue CreateOrUpdateGithubIssue(IList<GithubIssue> githubIssues, WorkItem workItem)
        {
            // TODO: for now this is matching by title. Move to match by github id that is in TFS DB
            GithubIssue githubIssue = githubIssues.FirstOrDefault(i => i.Title.Equals(workItem.Title));

            GithubIssueRequest githubIssueRequest = new GithubIssueRequest
            {
                Title = workItem.Title,
                State = GetGithubIssueState(workItem),
                Body = workItem.Description,
                Labels = GetGithubLabels(workItem),
                Assignee = GetGithubUserAssignedTo(workItem),
                Milestone = CreateOrUpdateMilestone(workItem)
            };

            if (githubIssue == null)
            {
                githubIssue = GithubChannel.CreateIssue(GithubOwner, GithubRepository, githubIssueRequest);

                var historyComments = workItem.Revisions.Cast<Revision>().Where(
                        w => !string.IsNullOrEmpty(w.Fields["history"].Value as string));

                foreach (Revision commentWorkedItem in historyComments)
                {
                    GithubChannel.CreateComment(GithubOwner, GithubRepository, githubIssue.Number.ToString(), new GithubComment
                    {
                        Body = string.Format("[{0} - {1}]: {2}", commentWorkedItem.WorkItem.ChangedDate, commentWorkedItem.WorkItem.ChangedBy, commentWorkedItem.Fields["history"].Value)
                    });
                }
            }
            else
            {
                GithubChannel.UpdateIssue(GithubOwner, GithubRepository, githubIssue.Number.ToString(), githubIssueRequest);

                var historyComments = workItem.Revisions.Cast<Revision>().Where(
                        w => !string.IsNullOrEmpty(w.Fields["history"].Value as string));

                var comments = GithubChannel.GetCommentFromIssue(GithubOwner, GithubRepository, githubIssue.Number.ToString());
                // Add, added comments
                foreach (Revision commentWorkedItem in historyComments)
                {
                    var body = string.Format("[{0} - {1}]: {2}", commentWorkedItem.WorkItem.ChangedDate, commentWorkedItem.WorkItem.ChangedBy, commentWorkedItem.Fields["history"].Value);
                    if (!comments.Any(c => c.Body.Equals(body)))
                    {
                        GithubChannel.CreateComment(GithubOwner, GithubRepository, githubIssue.Number.ToString(), new GithubComment
                        {
                            Body = string.Format("[{0} - {1}]: {2}", commentWorkedItem.WorkItem.ChangedDate, commentWorkedItem.WorkItem.ChangedBy, commentWorkedItem.Fields["history"].Value)
                        });
                    }
                }
            }

            return githubIssueRequest;
        }

        private WorkItem CreateOrUpdateWorkItem(IList<WorkItem> workItems, GithubIssue githubIssue)
        {
            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(TfsServerAddress));
            var wiStore = tfs.GetService<WorkItemStore>();

            Project teamProject = wiStore.Projects["RD"];
            WorkItemType workItemType = teamProject.WorkItemTypes["RDBug"];

            WorkItem workItem = new WorkItem(workItemType)
            {
                Title = githubIssue.Title,
                Description = githubIssue.Body,
                State = GetWorkItemState(githubIssue)
            };

            /*
                Title = workItem.Title,
                State = GetGithubIssueState(workItem),
                Body = workItem.Description,
                Labels = GetGithubLabels(workItem),
                Assignee = GetGithubUserAssignedTo(workItem),
                Milestone = CreateOrUpdateMilestone(workItem)
            */

            workItem.Save();

            return workItem;
        }

        private string GetGithubUserAssignedTo(WorkItem workItem)
        {
            var assignedTo = GetFieldValue(workItem, "assigned to");
            var collaborators = GithubChannel.GetCollaboratorsFromRepo(GithubOwner, GithubRepository);
            var user = collaborators.FirstOrDefault(u =>
            {
                GithubUser githubUser = GetGithubUser(u.Login);
                if (string.IsNullOrEmpty(githubUser.Name))
                {
                    return false;
                }

                return githubUser.Name.Equals(assignedTo, StringComparison.InvariantCultureIgnoreCase);
            });

            return user != null ? user.Login : null;
        }

        private string CreateOrUpdateLabel(IEnumerable<GithubLabel> labels, string name, string color)
        {
            GithubLabel typeLabel = labels.FirstOrDefault(l => l.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (typeLabel == null)
            {
                GithubChannel.CreateLabel(GithubOwner, GithubRepository, new GithubLabel
                {
                    Color = color,
                    Name = name
                });
            }
            else if (typeLabel.Color != color)
            {
                typeLabel.Color = color;
                GithubChannel.UpdateLabel(GithubOwner, GithubRepository, typeLabel.Name, typeLabel);
            }

            return name;
        }

        private int CreateOrUpdateMilestone(WorkItem workItem)
        {
            // Iteration Path
            var iterationPath = GetFieldValue(workItem, "Iteration Path");
            var iterationParts = iterationPath.Split('\\');
            var iterationName = iterationParts[iterationParts.Length - 1];
            return (int) GetGithubMilestone(iterationName).Number;
        }

        private string GetGithubIssueState(WorkItem workItem)
        {
            if (workItem.State.Equals("Active", StringComparison.InvariantCultureIgnoreCase))
            {
                return "open";
            }

            return "closed";
        }

        private string GetWorkItemState(GithubIssue githubIssue)
        {
            if (githubIssue.State.Equals("open"))
            {
                return "Active";
            }

            return "Resolved";
        }

        private string GetFieldValue(WorkItem workItem, string fieldName)
        {
            Field field = workItem.Fields.Cast<Field>().FirstOrDefault(
                f => f.Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
            if (field != null)
            {
                return field.Value.ToString();
            }

            return null;
        }

        private IList<string> GetGithubLabels(WorkItem workItem)
        {
            var labels = GithubChannel.GetLabels(GithubOwner, GithubRepository);

            return new List<string>(new[] {
                CreateOrUpdateLabel(labels, "P" + GetFieldValue(workItem, "priority"), WorkItemPriorityColor),
                CreateOrUpdateLabel(labels, GetFieldValue(workItem, "issue"), WorkItemIssueTypeColor),
                CreateOrUpdateLabel(labels, GetFieldValue(workItem, "state"), WorkItemWorkStatusColor)
            });
        }

        // TODO: Move to some sort of repository
        private GithubUser GetGithubUser(string userLogin)
        {
            if (GithubUsers.ContainsKey(userLogin))
            {
                return GithubUsers[userLogin];
            }

            GithubUser githubUser = GithubChannel.GetUser(userLogin);
            GithubUsers[userLogin] = githubUser;
            return githubUser;
        }

        private GithubMilestone GetGithubMilestone(string title)
        {
            if (GithubMilestones == null || !GithubMilestones.ContainsKey(title))
            {
                GithubMilestones = GithubChannel.GetMilestonesFromRepo(GithubOwner, GithubRepository).ToDictionary(m => m.Title, m => m);    
            }

            if (GithubMilestones.ContainsKey(title))
            {
                return GithubMilestones[title];
            }

            return GithubChannel.CreateMilestoneOnRepo(GithubOwner, GithubRepository, new GithubMilestone
            {
                Title = title,
                State = GithubMilestoneStates.Open
            });
        }
    }
}