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
using System.Runtime.Serialization;

namespace GithubClient.Model
{
    [DataContract]
    public class GithubIssue : IComparable
    {
        [DataMember(Name = "url", IsRequired = false)]
        public string Url { get; set; }

        [DataMember(Name = "html_url", IsRequired = false)]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "number", IsRequired = false)]
        public int Number { get; set; }

        [DataMember(Name = "state", IsRequired = false)]
        public string State { get; set; }

        [DataMember(Name = "title", IsRequired = true)]
        public string Title { get; set; }

        [DataMember(Name = "body", IsRequired = true)]
        public string Body { get; set; }

        [DataMember(Name = "user", IsRequired = false)]
        public GithubUser User { get; set; }

        [DataMember(Name = "label", IsRequired = false)]
        public IList<GithubLabel> Labels { get; set; }

        [DataMember(Name = "assignee", IsRequired = false)]
        public GithubUser Assignee { get; set; }

        [DataMember(Name = "milestone", IsRequired = false)]
        public GithubMilestone Milestone { get; set; }

        [DataMember(Name = "comments", IsRequired = false)]
        public string Comments { get; set; }

        [DataMember(Name = "pull_request", IsRequired = false)]
        public GithubPullRequest PullRequest { get; set; }

        [DataMember(Name = "closed_at", IsRequired = false)]
        public string ClosedAt { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updated_at", IsRequired = false)]
        public string UpdatedAt { get; set; }

        public int CompareTo(object obj)
        {
            return Number - ((GithubIssue)obj).Number;
        }
    }
}