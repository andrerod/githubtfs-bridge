using System;
using System.Configuration;
using GithubClient;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace ConsoleApplication1
{
    public class Program
    {
        static void Main()
        {
            var githubChannel = GithubClient.GithubClient.CreateChannel(ConfigurationManager.AppSettings["GithubUsername"], ConfigurationManager.AppSettings["GithubPassword"]);
            var issues = githubChannel.GetIssuesFromRepo("WindowsAzure", "azure-sdk-tools");
            foreach (var issue in issues)
            {
                Console.WriteLine(issue.Title);
            }

                    // Get All Team Projects
            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://vstfrd.dns.corp.microsoft.com:8080"));
            var wiStore = tfs.GetService<WorkItemStore>();

            foreach (var project in wiStore.Projects)
            {
                Console.WriteLine(project.ToString());
            }

            Console.ReadLine();
        }
    }
}