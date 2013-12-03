using System;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace GithubTFSBridge.Providers
{
    public class TfsIssuesProvider : IIssuesProvider<WorkItem>
    {
        private string TfsServerAddress { get; set; }
        private string TfsProjectName { get; set; }
        private string TfsAreaPath { get; set; }

        private TfsTeamProjectCollection TfsClient { get; set; }

        public TfsIssuesProvider(string tfsServerAddress, string tfsProjectName, string tfsAreaPath)
        {
            TfsServerAddress = tfsServerAddress;
            TfsProjectName = tfsProjectName;
            TfsAreaPath = tfsAreaPath;

            TfsClient = new TfsTeamProjectCollection(new Uri(TfsServerAddress));
        }

        public void AddIssue(IWorkItem workItem)
        {
            throw new NotImplementedException();
        }

        public void RemoveIssue(IWorkItem workItem)
        {
            throw new NotImplementedException();
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
            TfsClient.EnsureAuthenticated();

            return TfsClient.GetService<WorkItemStore>()
                .Query(string.Format(
                        "SELECT * FROM WorkItems WHERE [System.TeamProject] = '{0}' AND [System.AreaPath] under '{1}'",
                        TfsProjectName, TfsAreaPath))
                .Cast<Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem>()
                .Select(Convert).AsQueryable();
        }

        public WorkItem GetById(string id)
        {
            throw new NotImplementedException();
        }

        private WorkItem Convert(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem tfsWorkItem)
        {
            WorkItem workItem = new WorkItem();
            workItem.Title = tfsWorkItem.Title;
            workItem.Severity = int.Parse((string) tfsWorkItem.Fields["severity"].Value);
            workItem.Priority = (int) tfsWorkItem.Fields["priority"].Value;

            return workItem;
        }

        private Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem Convert(WorkItem workItem)
        {
            throw new NotImplementedException();
        }
    }
}