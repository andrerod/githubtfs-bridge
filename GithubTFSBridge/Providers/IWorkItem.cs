using System.Collections.Generic;

namespace GithubTFSBridge.Providers
{
    public interface IWorkItem
    {
        int TfsId { get; set; }
        string Author { get; set; }
        string Iteration { get; set; }
        int Priority { get; set; }
        int Severity { get; set; }
        string WorkStatus { get; set; }
        string Title { get; set; }
        IList<string> Comments { get; set; } 
    }
}
