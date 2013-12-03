using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Octokit;

namespace GithubTFSBridge.Providers
{
    public class GitHubIssuesProvider : IIssuesProvider<WorkItem>
    {
        private string Username { get; set; }
        private string Password { get; set; }
        private string Owner { get; set; }
        private string Repository { get; set; }

        private GitHubClient GitHubClient { get; set; }

        public GitHubIssuesProvider(string username, string password, string owner, string repository)
        {
            Username = username;
            Password = password;
            Owner = owner;
            Repository = repository;

            GitHubClient = new GitHubClient(new ProductHeaderValue("Microsoft Windows Azure"));
        }

        public void Insert(WorkItem entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(WorkItem entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<WorkItem> SearchFor(Func<WorkItem, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<WorkItem> GetAll()
        {
            return GitHubClient.Issue.GetForRepository(Owner, Repository).Result
                .Select(Convert).AsQueryable();
        }

        public WorkItem GetById(string id)
        {
            throw new NotImplementedException();
        }

        private WorkItem Convert(Issue gitHubIssue)
        {
            WorkItem workItem = new WorkItem();

            return workItem;
        }

        private Issue Convert(WorkItem workItem)
        {
            throw new NotImplementedException();
        }
    }
}
