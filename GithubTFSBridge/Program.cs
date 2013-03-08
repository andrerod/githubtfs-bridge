using System;
using System.Configuration;
using GithubClient;

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

            Console.ReadLine();
        }
    }
}