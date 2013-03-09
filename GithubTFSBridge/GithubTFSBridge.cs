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

        public GithubTFSBridge (string githubUsername, string githubPassword, string githubOwner, string githubRepository, string tfsServerAddress, string tfsPath)
        {
            GithubOwner = githubOwner;
            GithubRepository = githubRepository;
            TfsServerAddress = tfsServerAddress;
            TfsPath = tfsPath;

            GithubChannel = GithubClient.GithubClient.CreateChannel(githubUsername, githubPassword);
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

        private GithubIssueRequest CreateOrUpdateGithubIssue(IList<GithubIssue> githubIssues, WorkItem workItem)
        {
            GithubIssueRequest githubIssue = new GithubIssueRequest
            {
                Title = workItem.Title,
                State = GetGithubIssueState(workItem),
                Body = workItem.Description,
                Labels = GetGithubLabels(workItem),
                Assignee = GetGithubUserAssignedTo(workItem).Login
            };

            GithubChannel.CreateIssue(GithubOwner, GithubRepository, githubIssue);

            return githubIssue;
        }

        private GithubUser GetGithubUserAssignedTo(WorkItem workItem)
        {
            var assignedTo = GetFieldValue(workItem, "assigned to");
            var collaborators = GithubChannel.GetCollaboratorsFromRepo(GithubOwner, GithubRepository);
            return collaborators.FirstOrDefault(u => u.Name.Equals(assignedTo, StringComparison.InvariantCultureIgnoreCase));
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
    }
}
