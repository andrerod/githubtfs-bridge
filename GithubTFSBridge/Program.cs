using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GithubClient;
using GithubClient.Model;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ConsoleApplication1
{
    public class Program
    {
        private static IGithubServiceManagement _githubChannel;
        private static string _owner = "andrerod";
        private static string _repository = "testissuesrepo";

        private const string WorkItemTypeColor = "0000FF";

        public static void Main()
        {
            _githubChannel = GithubClient.GithubClient.CreateChannel(ConfigurationManager.AppSettings["GithubUsername"], ConfigurationManager.AppSettings["GithubPassword"]);
            
            /*
            var issues = githubChannel.GetIssuesFromRepo("andrerod", "github-tfs-bridge");
            foreach (var issue in issues)
            {
                Console.WriteLine(issue.Title);
            }
            */

            // Get All Team Projects
            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://vstfrd.dns.corp.microsoft.com:8080"));
            var wiStore = tfs.GetService<WorkItemStore>();

            var project = wiStore.Projects.Cast<Project>().FirstOrDefault(p =>
                p.Name.Equals("RD", StringComparison.InvariantCultureIgnoreCase));

            QueryDefinition query = GetQuery(project.QueryHierarchy, "Shared Queries/Kudu/Portal - Kudu Future");
            if (query != null)
            {
                var workItems = wiStore.Query(query.QueryText.Replace("@project", "'" + query.Project.Name + "'"));
                foreach (WorkItem workItem in workItems)
                {
                    _githubChannel.CreateIssue(_owner, _repository, CreateGithubIssue(workItem));
                }
            }
            
            Console.ReadLine();
        }

        public static QueryDefinition GetQuery(QueryFolder queryFolder, string path)
        {
            var parts = path.Split('/');
            for (int part = 0; part < parts.Length - 1; part++)
            {
                queryFolder = queryFolder[parts[part]] as QueryFolder;
            }

            return (queryFolder[parts[parts.Length - 1]] as QueryDefinition);
        }

        public static GithubIssue CreateGithubIssue(WorkItem workItem)
        {
            EnsureLabels(workItem);

            GithubIssue githubIssue = new GithubIssue
            {
                Title = workItem.Title,
                State = GetGithubIssueState(workItem),
                Body = workItem.Description,
                Labels = GetGithubLabels(workItem)
            };

            return githubIssue;
        }

        public static void EnsureLabels(WorkItem workItem)
        {
            var labels = _githubChannel.GetLabels(_owner, _repository);
            if (labels.FirstOrDefault(l => l.Name.Equals(workItem.Type)) == null)
            {
                _githubChannel.CreateLabel(_owner, _repository, new GithubLabel()
                {
                    Color = WorkItemTypeColor,
                    Name = workItem.Type.Name
                });
            }
        }

        public static string GetGithubIssueState(WorkItem workItem)
        {
            if (workItem.State.Equals("Active", StringComparison.InvariantCultureIgnoreCase))
            {
                return "open";
            }

            return "closed";
        }

        public static IList<string> GetGithubLabels(WorkItem workItem)
        {
            return new List<string>(new [] {
                workItem.Type.Name
            });
        }
    }
}