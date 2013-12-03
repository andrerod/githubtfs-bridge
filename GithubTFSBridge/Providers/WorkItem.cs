using System.Collections.Generic;

namespace GithubTFSBridge.Providers
{
    public class WorkItem : IWorkItem
    {
        public string Author { get; set; }
        public string Iteration { get; set; }
        public int Priority { get; set; }
        public int Severity { get; set; }
        public string WorkStatus { get; set; }
        public string Title { get; set; }
        public IList<string> Comments { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
    }
}
