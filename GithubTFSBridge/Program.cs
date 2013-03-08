using System;
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
        static void Main()
        {
            var githubChannel = GithubClient.GithubClient.CreateChannel(ConfigurationManager.AppSettings["GithubUsername"], ConfigurationManager.AppSettings["GithubPassword"]);
            
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

            QueryFolder sharedQueriesFolder = project.QueryHierarchy["Shared Queries"] as QueryFolder;
            QueryFolder kuduFolder = sharedQueriesFolder["Kudu"] as QueryFolder;

            QueryDefinition query = kuduFolder["Portal - Kudu Future"] as QueryDefinition;
            if (query != null)
            {
                var workItems = wiStore.Query(query.QueryText.Replace("@project", "'" + query.Project.Name + "'"));
                foreach (WorkItem workItem in workItems)
                {
                    githubChannel.CreateIssue("andrerod", "testrepo", CreateGithubIssue(workItem));
                }
            }
            
            Console.ReadLine();
        }

        public static GithubIssue CreateGithubIssue(WorkItem workItem)
        {
            GithubIssue githubIssue = new GithubIssue
            {
                Title = workItem.Title
            };

            return githubIssue;
        }
    }
}