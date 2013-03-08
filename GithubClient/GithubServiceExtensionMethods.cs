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

using System.Globalization;
using GithubClient.Model;
using System.Collections.Generic;

namespace GithubClient
{
    public static class GithubServiceExtensionMethods
    {
        public static GithubAuthorization CreateAuthorizationToken(this IGithubServiceManagement proxy, GithubAuthorizationRequest request)
        {
            return proxy.EndCreateAuthorizationToken(proxy.BeginCreateAuthorizationToken(request, null, null));
        }

        public static List<GithubOrganization> GetOrganizations(this IGithubServiceManagement proxy)
        {
            return proxy.EndGetOrganizations(proxy.BeginGetOrganizations(null, null));
        }

        public static List<GithubOrganization> GetOrganizationsFromUser(this IGithubServiceManagement proxy, string user)
        {
            return proxy.EndGetOrganizationsFromUser(proxy.BeginGetOrganizationsFromUser(user, null, null));
        }

        public static List<GithubRepository> GetRepositories(this IGithubServiceManagement proxy)
        {
            return proxy.EndGetRepositories(proxy.BeginGetRepositories(null, null));
        }

        public static List<GithubRepository> GetRepositoriesFromUser(this IGithubServiceManagement proxy, string user)
        {
            return proxy.EndGetRepositoriesFromUser(proxy.BeginGetRepositoriesFromUser(user, null, null));
        }

        public static List<GithubRepository> GetRepositoriesFromOrg(this IGithubServiceManagement proxy, string organization)
        {
            return proxy.EndGetRepositoriesFromOrg(proxy.BeginGetRepositoriesFromOrg(organization, null, null));
        }

        public static List<GithubRepositoryHook> GetRepositoryHooks(this IGithubServiceManagement proxy, string owner, string repository)
        {
            return proxy.EndGetRepositoryHooks(proxy.BeginGetRepositoryHooks(owner, repository, null, null));
        }

        public static GithubRepositoryHook CreateRepositoryHook(this IGithubServiceManagement proxy, string owner, string repository, GithubRepositoryHook hook)
        {
            return proxy.EndCreateRepositoryHook(proxy.BeginCreateRepositoryHook(owner, repository, hook, null, null));
        }

        public static GithubRepositoryHook UpdateRepositoryHook(this IGithubServiceManagement proxy, string owner, string repository, string id, GithubRepositoryHook hook)
        {
            return proxy.EndUpdateRepositoryHook(proxy.BeginUpdateRepositoryHook(owner, repository, id, hook, null, null));
        }

        public static void TestRepositoryHook(this IGithubServiceManagement proxy, string owner, string repository, string id)
        {
            proxy.EndTestRepositoryHook(proxy.BeginTestRepositoryHook(owner, repository, id, null, null));
        }

        public static List<GithubIssue> GetIssues(this IGithubServiceManagement proxy)
        {
            return proxy.EndGetIssues(proxy.BeginGetIssues(null, null));
        }

        public static List<GithubIssue> GetIssuesFromUser(this IGithubServiceManagement proxy)
        {
            return proxy.EndGetIssuesFromUser(proxy.BeginGetIssuesFromUser(null, null));
        }

        public static List<GithubIssue> GetIssuesFromOrg(this IGithubServiceManagement proxy, string organization)
        {
            return proxy.EndGetIssuesFromOrg(proxy.BeginGetIssuesFromOrg(organization, null, null));
        }

        public static List<GithubIssue> GetIssuesFromRepo(this IGithubServiceManagement proxy, string owner, string repo)
        {
            return proxy.EndGetIssuesFromRepo(proxy.BeginGetIssuesFromRepo(owner, repo, null, null));
        }

        public static GithubIssue GetIssue(this IGithubServiceManagement proxy, string owner, string repo, int number)
        {
            return proxy.EndGetIssue(proxy.BeginGetIssue(owner, repo, number.ToString(CultureInfo.InvariantCulture), null, null));
        }

        public static GithubIssue CreateIssue(this IGithubServiceManagement proxy, string owner, string repo, GithubIssue issue)
        {
            return proxy.EndCreateIssue(proxy.BeginCreateIssue(owner, repo, issue, null, null));
        }

        public static GithubIssue UpdateIssue(this IGithubServiceManagement proxy, string owner, string repo, int number, GithubIssue issue)
        {
            return proxy.EndUpdateIssue(proxy.BeginUpdateIssue(owner, repo, number.ToString(CultureInfo.InvariantCulture), issue, null, null));
        }
    }
}
