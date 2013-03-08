﻿// ----------------------------------------------------------------------------------
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
    public static class GithubServiceEventsExtensionMethods
    {
        public static IList<GithubEvent> GetEventsFromIssue(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            return proxy.EndGetEventsFromIssue(proxy.BeginGetEventsFromIssue(owner, repo, number, null, null));
        }

        public static IList<GithubEvent> GetEventsFromRepo(this IGithubServiceManagement proxy, string owner, string repo)
        {
            return proxy.EndGetEventsFromRepo(proxy.BeginGetEventsFromRepo(owner, repo, null, null));
        }

        public static GithubEvent GetEvent(this IGithubServiceManagement proxy, string owner, string repo, string id)
        {
            return proxy.EndGetEvent(proxy.BeginGetEvent(owner, repo, id, null, null));
        }
    }
}
