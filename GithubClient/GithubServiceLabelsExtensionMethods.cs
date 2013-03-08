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

using GithubClient.Model;
using System.Collections.Generic;

namespace GithubClient
{
    public static class GithubServiceLabelsExtensionMethods
    {
        public static IList<GithubLabel> GetLabels(this IGithubServiceManagement proxy, string owner, string repo)
        {
            return proxy.EndGetLabels(proxy.BeginGetLabels(owner, repo, null, null));
        }

        public static GithubLabel GetLabel(this IGithubServiceManagement proxy, string owner, string repo, string name)
        {
            return proxy.EndGetLabel(proxy.BeginGetLabel(owner, repo, name, null, null));
        }

        public static GithubLabel CreateLabel(this IGithubServiceManagement proxy, string owner, string repo, GithubLabel label)
        {
            return proxy.EndCreateLabel(proxy.BeginCreateLabel(owner, repo, label, null, null));
        }

        public static GithubLabel UpdateLabel(this IGithubServiceManagement proxy, string owner, string repo, string name, GithubLabel label)
        {
            return proxy.EndUpdateLabel(proxy.BeginUpdateLabel(owner, repo, name, label, null, null));
        }

        public static GithubLabel DeleteLabel(this IGithubServiceManagement proxy, string owner, string repo, string name)
        {
            return proxy.EndDeleteLabel(proxy.BeginDeleteLabel(owner, repo, name, null, null));
        }

        public static IList<GithubLabel> GetLabelsFromIssue(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            return proxy.EndGetLabelsFromIssue(proxy.BeginGetLabelsFromIssue(owner, repo, number, null, null));
        }

        public static IList<GithubLabel> CreateLabelsOnIssue(this IGithubServiceManagement proxy, string owner, string repo, string number, IList<string> labels)
        {
            return proxy.EndCreateLabelsOnIssue(proxy.BeginCreateLabelsOnIssue(owner, repo, number, labels, null, null));
        }

        public static void DeleteLabelFromIssue(this IGithubServiceManagement proxy, string owner, string repo, string number, string name)
        {
            proxy.EndDeleteLabelFromIssue(proxy.BeginDeleteLabelFromIssue(owner, repo, number, name, null, null));
        }

        public static void UpdateLabelsOnIssue(this IGithubServiceManagement proxy, string owner, string repo, string number, IList<string> labels)
        {
            proxy.EndUpdateLabelsOnIssue(proxy.BeginUpdateLabelsOnIssue(owner, repo, number, labels, null, null));
        }

        public static void DeleteLabelsFromIssue(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            proxy.EndDeleteLabelsFromIssue(proxy.BeginDeleteLabelsFromIssue(owner, repo, number, null, null));
        }

        public static IList<GithubLabel> GetLabelsFromMilestone(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            return proxy.EndGetLabelsFromMilestone(proxy.BeginGetLabelsFromMilestone(owner, repo, number, null, null));
        }
    }
}
