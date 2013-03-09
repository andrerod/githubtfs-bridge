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

using System.Runtime.Serialization;

namespace GithubClient.Model
{
    public sealed class GithubMilestoneStates
    {
        public static readonly string Open = "open";
        public static readonly string Closed = "closed";
    }

    public class GithubMilestone
    {
        [DataMember(Name = "state", IsRequired = false)]
        public string State { get; set; }

        [DataMember(Name = "title", IsRequired = false)]
        public string Title { get; set; }

        [DataMember(Name = "description", IsRequired = false)]
        public string Description { get; set; }

        [DataMember(Name = "due_on", IsRequired = false)]
        public string DueOn { get; set; }
    }

    public class GithubMilestoneResponse : GithubMilestone
    {
        [DataMember(Name = "url", IsRequired = false)]
        public string Url { get; set; }

        [DataMember(Name = "number", IsRequired = false)]
        public int? Number { get; set; }

        [DataMember(Name = "creator", IsRequired = false)]
        public GithubUser Creator { get; set; }

        [DataMember(Name = "open_issues", IsRequired = false)]
        public int? OpenIssues { get; set; }

        [DataMember(Name = "closed_issues", IsRequired = false)]
        public int? ClosedIssues { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string CreatedAt { get; set; }
    }
}
