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
using GithubClient;
using GithubClient.Model;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ConsoleApplication1
{
    public class GithubTFSBridge
    {
        private IGithubServiceManagement GithubChannel { get; set; }
        private string GithubOwner { get; set; }
        private string GithubRepository { get; set; }
        private string TfsServerAddress { get; set; }
        private string TfsPath { get; set; }

        private const string WorkItemTypeColor = "0000FF";
        private const string WorkItemPriorityColor = "FF0000";

        private IDictionary<string, GithubUser> GithubUsers { get; set; }
        private IDictionary<string, GithubMilestone> GithubMilestones { get; set; } 

        public GithubTFSBridge (string githubUsername, string githubPassword, string githubOwner, string githubRepository, string tfsServerAddress, string tfsPath)
        {
            GithubOwner = githubOwner;
            GithubRepository = githubRepository;
            TfsServerAddress = tfsServerAddress;
            TfsPath = tfsPath;

            GithubChannel = GithubClient.GithubClient.CreateChannel(githubUsername, githubPassword);

            GithubUsers = new Dictionary<string, GithubUser>();
            GithubMilestones = new Dictionary<string, GithubMilestone>();
        }

        public void Synchronize()
        {
            // Get All Team Projects
            WorkItemCollection workItems = GetWorkItems("Shared Queries/Kudu/Portal - Kudu Future");
            IList<GithubIssue> githubIssues = GithubChannel.GetIssuesFromRepo(GithubOwner, GithubRepository);
            
            // Create new github issues from work items
            foreach (WorkItem workItem in workItems)
            {
                githubIssues.Add(CreateOrUpdateGithubIssue(githubIssues, workItem));
            }
            
            // Area Path
            // "RD\\Azure App Plat\\Azure UX\\Experiences\\Kudu"

            // Iteration Path
            // RD\Azure App Plat\Azure UX\AUX Portal\Future

            // TODO: Create new workitems from github issues

            // TODO: Remove github issues that don't exist in workitems

            // TODO: Remove workItems that don't exist in github issues
        }

        private WorkItemCollection GetWorkItems(string path)
        {
            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(TfsServerAddress));
            var wiStore = tfs.GetService<WorkItemStore>();

            var project = wiStore.Projects.Cast<Project>().FirstOrDefault(p =>
                p.Name.Equals("RD", StringComparison.InvariantCultureIgnoreCase));

            QueryFolder queryFolder = project.QueryHierarchy;
            var parts = path.Split('/');
            for (int part = 0; part < parts.Length - 1; part++)
            {
                queryFolder = queryFolder[parts[part]] as QueryFolder;
            }

            QueryDefinition query = (queryFolder[parts[parts.Length - 1]] as QueryDefinition);
            return wiStore.Query(query.QueryText.Replace("@project", "'" + query.Project.Name + "'"));
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
                GithubChannel.CreateIssue(GithubOwner, GithubRepository, githubIssueRequest);

                var actionHistory = workItem.GetActionsHistory();
            }
            else
            {
                GithubChannel.UpdateIssue(GithubOwner, GithubRepository, githubIssue.Number.ToString(), githubIssueRequest);

                // Create or update issue comments
                var comments = GithubChannel.GetCommentFromIssue(GithubOwner, GithubRepository, githubIssue.Number.ToString());                
            }


            return githubIssueRequest;
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

            IList<GithubMilestone> milestones = GithubChannel.GetMilestonesFromRepo(GithubOwner, GithubRepository);
            return (int)GetGithubMilestone(iterationName).Number;
        }

        private string GetGithubIssueState(WorkItem workItem)
        {
            if (workItem.State.Equals("Active", StringComparison.InvariantCultureIgnoreCase))
            {
                return "open";
            }

            return "closed";
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
                CreateOrUpdateLabel(labels, workItem.Type.Name, WorkItemTypeColor),
                CreateOrUpdateLabel(labels, GetFieldValue(workItem, "priority"), WorkItemPriorityColor)
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
            if (GithubMilestones == null)
            {
                GithubMilestones = GithubChannel.GetMilestonesFromRepo(GithubOwner, GithubRepository).ToDictionary(m => m.Description, m => m);    
            }

            if (GithubMilestones.ContainsKey(title))
            {
                return GithubMilestones[title];
            }

            return GithubChannel.CreateMilestoneFromRepo(GithubOwner, GithubRepository, new GithubMilestone
            {
                Title = title
            });
        }
    }
}
