// ----------------------------------------------------------------------------------
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
using System.ServiceModel;
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
        private bool SyncGithub { get; set; }
        private bool SyncTfs { get; set; }
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
            string tfsPath,
            bool syncGithub,
            bool syncTfs)
        {
            GithubOwner = githubOwner;
            GithubRepository = githubRepository;
            TfsUsername = tfsUsername;
            TfsPassword = tfsPassword;
            TfsDomain = tfsDomain;
            TfsServerAddress = tfsServerAddress;
            TfsProjectName = tfsProjectName;
            TfsPath = tfsPath;
            SyncGithub = syncGithub;
            SyncTfs = syncTfs;

            GithubChannel = GithubClient.GithubClient.CreateChannel(githubUsername, githubPassword);

            GithubUsers = new Dictionary<string, GithubUser>();
            GithubMilestones = new Dictionary<string, GithubMilestone>();
        }

        public void Synchronize()
        {
            // Get All Team Projects
            IList<WorkItem> workItems = new List<WorkItem>(GetWorkItems("Shared Queries/Kudu/Portal - Kudu Future").Cast<WorkItem>());
            IList<GithubIssueResponse> githubIssues = GithubChannel.GetIssuesFromRepo(GithubOwner, GithubRepository);

            // Create new github issues from work items
            if (SyncGithub)
            {
                foreach (WorkItem workItem in workItems)
                {
                    githubIssues.Add(CreateOrUpdateGithubIssue(githubIssues, workItem));
                }
            }

            if (SyncTfs)
            {
                // Create new work items from git issues
                foreach (GithubIssueResponse githubIssue in githubIssues)
                {
                    var newWorkItem = CreateOrUpdateWorkItem(workItems, githubIssue);
                    if (newWorkItem != null)
                    {
                        workItems.Add(newWorkItem);
                    }
                }
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

        private GithubIssueResponse CreateOrUpdateGithubIssue(IList<GithubIssueResponse> githubIssues, WorkItem workItem)
        {
            // TODO: for now this is matching by title. Move to match by github id that is in TFS DB
            GithubIssueResponse githubIssue = githubIssues.FirstOrDefault(i => i.Title.Equals(workItem.Title));

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
                if (DateTime.Parse(githubIssue.UpdatedAt) < workItem.ChangedDate)
                {
                    githubIssue = GithubChannel.UpdateIssue(GithubOwner, GithubRepository, githubIssue.Number.ToString(),
                                                            githubIssueRequest);
                }

                var historyComments = workItem.Revisions.Cast<Revision>().Where(
                        w => !string.IsNullOrEmpty(w.Fields["history"].Value as string));

                var comments = GithubChannel.GetCommentsFromIssue(GithubOwner, GithubRepository, githubIssue.Number.ToString());
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

            return githubIssue;
        }

        private WorkItem CreateOrUpdateWorkItem(IList<WorkItem> workItems, GithubIssueResponse githubIssue)
        {
            var credentials = new NetworkCredential(TfsUsername, TfsPassword, TfsDomain);

            var tfs = new TfsTeamProjectCollection(new Uri(TfsServerAddress), credentials);
            tfs.EnsureAuthenticated();
            var wiStore = tfs.GetService<WorkItemStore>();

            Project teamProject = wiStore.Projects[TfsProjectName];

            WorkItem workItem = workItems.FirstOrDefault(wi => wi.Title.Equals(githubIssue.Title, StringComparison.InvariantCultureIgnoreCase));

            var saveWorkItem = true;
            var newWorkItem = false;
            if (workItem != null)
            {
                workItem.Open();

                if (DateTime.Parse(githubIssue.UpdatedAt) < workItem.ChangedDate)
                {
                    saveWorkItem = false;
                }
            }
            else
            {
                newWorkItem = true;
                workItem = new WorkItem(teamProject.WorkItemTypes["Bug"]);
            }

            if (saveWorkItem)
            {
                workItem.Title = githubIssue.Title;
                workItem.Description = githubIssue.Body;
                workItem.State = GetWorkItemState(githubIssue);

                if (githubIssue.Assignee != null && !string.IsNullOrEmpty(githubIssue.Assignee.Login))
                {
                    workItem.Fields["assigned to"].Value = githubIssue.Assignee.Login;
                }
                else
                {
                    workItem.Fields["assigned to"].Value = "Active";
                }

                workItem.Fields["issue"].Value = GetWorkItemValue(githubIssue, "issue") ?? "Code defect";

                var priority = GetWorkItemValue(githubIssue, "priority");
                if (!string.IsNullOrEmpty(priority))
                {
                    priority = priority.Substring(1);
                    workItem.Fields["priority"].Value = priority;
                }
            }

            // TODO: make sure there is a comment with link to github issue

            if (saveWorkItem)
            {
                workItem.Save();
            }

            if (newWorkItem)
            {
                return workItem;
            }

            return null;
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

        private string GetWorkItemValue(GithubIssueResponse githubIssue, string name)
        {
            var color = GetLabelColor(name);
            var label = githubIssue.Labels.FirstOrDefault(l => l.Color.Equals(color));
            
            if (label != null)
            {
                return label.Name;
            }

            return null;
        }

        private string GetLabelColor(string name)
        {
            switch (name)
            {
                case "priority":
                    return WorkItemPriorityColor;
                case "issue":
                    return WorkItemIssueTypeColor;
                case "state":
                    return WorkItemWorkStatusColor;
                default:
                    throw new NotSupportedException();
            }
        }

        private IList<string> GetGithubLabels(WorkItem workItem)
        {
            var labels = GithubChannel.GetLabels(GithubOwner, GithubRepository);

            return new List<string>(new[] {
                CreateOrUpdateLabel(labels, GetFieldValue(workItem, "priority"), GetLabelColor("priority")),
                CreateOrUpdateLabel(labels, GetFieldValue(workItem, "issue"), GetLabelColor("issue")),
                CreateOrUpdateLabel(labels, GetFieldValue(workItem, "state"), GetLabelColor("state"))
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