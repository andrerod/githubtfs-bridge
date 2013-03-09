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

using System.Collections.Generic;
using GithubClient.Model;

namespace GithubClient
{
    public static class GithubServiceMilestonesExtensionMethods
    {
        public static IList<GithubMilestone> GetMilestonesFromRepo(this IGithubServiceManagement proxy, string owner, string repo)
        {
            return proxy.EndGetMilestonesFromRepo(proxy.BeginGetMilestonesFromRepo(owner, repo, null, null));
        }

        public static GithubMilestone GetMilestoneFromRepo(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            return proxy.EndGetMilestoneFromRepo(proxy.BeginGetMilestoneFromRepo(owner, repo, number, null, null));
        }

        public static GithubMilestone CreateMilestoneOnRepo(this IGithubServiceManagement proxy, string owner, string repo, GithubMilestone milestone)
        {
            return proxy.EndCreateMilestoneOnRepo(proxy.BeginCreateMilestoneOnRepo(owner, repo, milestone, null, null));
        }

        public static GithubMilestone UpdateMilestoneOnRepo(this IGithubServiceManagement proxy, string owner, string repo, string number, GithubMilestone milestone)
        {
            return proxy.EndUpdateMilestoneOnRepo(proxy.BeginUpdateMilestoneOnRepo(owner, repo, number, milestone, null, null));
        }

        public static void DeleteMilestoneFromRepo(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            proxy.EndDeleteMilestoneFromRepo(proxy.BeginDeleteMilestoneFromRepo(owner, repo, number, null, null));
        }
    }
}
