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
    public class GithubUser
    {
        [DataMember(Name = "login", IsRequired = false)]
        public string Login { get; set; }

        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name = "avatar_url", IsRequired = false)]
        public string AvatarUrl { get; set; }

        [DataMember(Name = "gravatar_id", IsRequired = false)]
        public string GravatarId { get; set; }

        [DataMember(Name = "url", IsRequired = false)]
        public string Url { get; set; }

        [DataMember(Name = "name", IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Name = "company", IsRequired = false)]
        public string Company { get; set; }

        [DataMember(Name = "blog", IsRequired = false)]
        public string Blog { get; set; }

        [DataMember(Name = "location", IsRequired = false)]
        public string Location { get; set; }

        [DataMember(Name = "email", IsRequired = false)]
        public string Email { get; set; }

        [DataMember(Name = "hireable", IsRequired = false)]
        public string Hireable { get; set; }

        [DataMember(Name = "bio", IsRequired = false)]
        public string Bio { get; set; }

        [DataMember(Name = "public_repos", IsRequired = false)]
        public int PublicRepos { get; set; }

        [DataMember(Name = "public_gists", IsRequired = false)]
        public int PublicGists { get; set; }

        [DataMember(Name = "followers", IsRequired = false)]
        public string Followers { get; set; }

        [DataMember(Name = "following", IsRequired = false)]
        public string Following { get; set; }

        [DataMember(Name = "html_url", IsRequired = false)]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string CreatedAt { get; set; }

        [DataMember(Name = "type", IsRequired = false)]
        public string Type { get; set; }
    }
}