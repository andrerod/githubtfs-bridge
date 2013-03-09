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
    [DataContract]
    public class GithubComment
    {
        [DataMember(Name = "id", IsRequired = false, EmitDefaultValue = false)]
        public string Id { get; set; }

        [DataMember(Name = "url", IsRequired = false, EmitDefaultValue = false)]
        public string Url { get; set; }

        [DataMember(Name = "html_url", IsRequired = false, EmitDefaultValue = false)]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "body", IsRequired = false, EmitDefaultValue = false)]
        public string Body { get; set; }

        [DataMember(Name = "user", IsRequired = false, EmitDefaultValue = false)]
        public GithubUser User { get; set; }

        [DataMember(Name = "created_at", IsRequired = false, EmitDefaultValue = false)]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updated_at", IsRequired = false, EmitDefaultValue = false)]
        public string UpdatedAt { get; set; }
    }
}
