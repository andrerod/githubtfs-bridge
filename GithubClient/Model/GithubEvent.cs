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
    public class GithubEvent
    {
        [DataMember(Name = "url", IsRequired = false)]
        public string Url { get; set; }

        [DataMember(Name = "actor", IsRequired = false)]
        public GithubUser Actor { get; set; }

        [DataMember(Name = "event", IsRequired = false)]
        public string Event { get; set; }

        [DataMember(Name = "commit_id", IsRequired = false)]
        public string CommitId { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string CreatedAt { get; set; }
    }
}
