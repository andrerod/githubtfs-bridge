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
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using GithubClient.Model;

namespace GithubClient
{
    /// <summary>
    /// Provides the Github Api. 
    /// </summary>
    [ServiceContract]
    public interface IGithubServiceManagement
    {
        [Description("Creates a new authorization")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "/authorizations", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginCreateAuthorizationToken(GithubAuthorizationRequest request, AsyncCallback callback, object state);
        GithubAuthorization EndCreateAuthorizationToken(IAsyncResult asyncResult);

        [Description("Gets the organizations for the authenticated user")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/user/orgs", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetOrganizations(AsyncCallback callback, object state);
        List<GithubOrganization> EndGetOrganizations(IAsyncResult asyncResult);

        [Description("Gets the organizations for an user")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/users/{user}/orgs", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetOrganizationsFromUser(string user, AsyncCallback callback, object state);
        List<GithubOrganization> EndGetOrganizationsFromUser(IAsyncResult asyncResult);

        [Description("Gets the repositories for the authenticated user")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/user/repos", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetRepositories(AsyncCallback callback, object state);
        List<GithubRepository> EndGetRepositories(IAsyncResult asyncResult);

        [Description("Gets the repositories for an user")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/users/{user}/repos", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetRepositoriesFromUser(string user, AsyncCallback callback, object state);
        List<GithubRepository> EndGetRepositoriesFromUser(IAsyncResult asyncResult);

        [Description("Gets the repositories for an organization")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/orgs/{organization}/repos?sort=updated&desc=desc", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetRepositoriesFromOrg(string organization, AsyncCallback callback, object state);
        List<GithubRepository> EndGetRepositoriesFromOrg(IAsyncResult asyncResult);

        [Description("Gets the repository hooks")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/repos/{owner}/{repository}/hooks", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetRepositoryHooks(string owner, string repository, AsyncCallback callback, object state);
        List<GithubRepositoryHook> EndGetRepositoryHooks(IAsyncResult asyncResult);

        [Description("Creates a repository hook")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "/repos/{owner}/{repository}/hooks", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginCreateRepositoryHook(string owner, string repository, GithubRepositoryHook hook, AsyncCallback callback, object state);
        GithubRepositoryHook EndCreateRepositoryHook(IAsyncResult asyncResult);

        [Description("Updates a repository hook")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PATCH", UriTemplate = "/repos/{owner}/{repository}/hooks/{id}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginUpdateRepositoryHook(string owner, string repository, string id, GithubRepositoryHook hook, AsyncCallback callback, object state);
        GithubRepositoryHook EndUpdateRepositoryHook(IAsyncResult asyncResult);

        [Description("Tests a repository hook")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "/repos/{owner}/{repository}/hooks/{id}/test", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginTestRepositoryHook(string owner, string repository, string id, AsyncCallback callback, object state);
        void EndTestRepositoryHook(IAsyncResult asyncResult);

        [Description("Gets the issues visible for the authenticated user accross all owned, member and organizational repositories")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/issues", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetIssues(AsyncCallback callback, object state);
        List<GithubIssue> EndGetIssues(IAsyncResult asyncResult);

        [Description("Gets the issues visible the authenticated user accross all owned and member repositories")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/user/issues", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetIssuesFromUser(AsyncCallback callback, object state);
        List<GithubIssue> EndGetIssuesFromUser(IAsyncResult asyncResult);

        [Description("Gets the repositories for an organization")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/orgs/{organization}/issues", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetIssuesFromOrg(string organization, AsyncCallback callback, object state);
        List<GithubIssue> EndGetIssuesFromOrg(IAsyncResult asyncResult);

        [Description("Gets the repositories for a repository")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/repos/{owner}/{repo}/issues", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetIssuesFromRepo(string owner, string repo, AsyncCallback callback, object state);
        List<GithubIssue> EndGetIssuesFromRepo(IAsyncResult asyncResult);

        [Description("Gets a single issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/repos/{owner}/{repo}/issues/{number}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetIssue(string owner, string repo, string number, AsyncCallback callback, object state);
        GithubIssue EndGetIssue(IAsyncResult asyncResult);

        [Description("Creates an issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "/repos/{owner}/{repo}/issues", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginCreateIssue(string owner, string repo, GithubIssue issue, AsyncCallback callback, object state);
        GithubIssue EndCreateIssue(IAsyncResult asyncResult);

        [Description("Updates an issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PATCH", UriTemplate = "/repos/{owner}/{repo}/issues/{number}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginUpdateIssue(string owner, string repo, string number, GithubIssue issue, AsyncCallback callback, object state);
        GithubIssue EndUpdateIssue(IAsyncResult asyncResult);

        [Description("List all labels for this repository")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/repos/{owner}/{repo}/labels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetLabels(string owner, string repo, AsyncCallback callback, object state);
        IList<GithubLabel> EndGetLabels(IAsyncResult asyncResult);

        [Description("Get a single label")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/repos/{owner}/{repo}/labels/{name}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetLabel(string owner, string repo, string name, AsyncCallback callback, object state);
        GithubLabel EndGetLabel(IAsyncResult asyncResult);

        [Description("Create a label")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "/repos/{owner}/{repo}/labels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginCreateLabel(string owner, string repo, GithubLabel label, AsyncCallback callback, object state);
        GithubLabel EndCreateLabel(IAsyncResult asyncResult);

        [Description("Update a label")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PATCH", UriTemplate = "/repos/{owner}/{repo}/labels/{name}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginUpdateLabel(string owner, string repo, string name, GithubLabel label, AsyncCallback callback, object state);
        GithubLabel EndUpdateLabel(IAsyncResult asyncResult);

        [Description("Delete a label")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = "/repos/{owner}/{repo}/labels/{name}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginDeleteLabel(string owner, string repo, string name, AsyncCallback callback, object state);
        GithubLabel EndDeleteLabel(IAsyncResult asyncResult);

        [Description("List labels on an issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/repos/{owner}/{repo}/issues/{number}/labels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetLabelsFromIssue(string owner, string repo, string number, AsyncCallback callback, object state);
        IList<GithubLabel> EndGetLabelsFromIssue(IAsyncResult asyncResult);

        [Description("Add labels to an issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = "/repos/{owner}/{repo}/issues/{number}/labels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginCreateLabelsOnIssue(string owner, string repo, string number, IList<string> labels, AsyncCallback callback, object state);
        IList<GithubLabel> EndCreateLabelsOnIssue(IAsyncResult asyncResult);

        [Description("Remove a label from an issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = "/repos/{owner}/{repo}/issues/{number}/labels/{name}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginDeleteLabelFromIssue(string owner, string repo, string number, string name, AsyncCallback callback, object state);
        void EndDeleteLabelFromIssue(IAsyncResult asyncResult);

        [Description("Replace all labels for an issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PUT", UriTemplate = "/repos/{owner}/{repo}/issues/{number}/labels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginUpdateLabelsOnIssue(string owner, string repo, string number, IList<string> labels, AsyncCallback callback, object state);
        void EndUpdateLabelsOnIssue(IAsyncResult asyncResult);

        [Description("Remove all labels from an issue")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = "/repos/{owner}/{repo}/issues/{number}/labels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginDeleteLabelsFromIssue(string owner, string repo, string number, AsyncCallback callback, object state);
        void EndDeleteLabelsFromIssue(IAsyncResult asyncResult);

        [Description("Get labels for every issue in a milestone")]
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "GET", UriTemplate = "/repos/{owner}/{repo}/milestones/{number}/labels", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        IAsyncResult BeginGetLabelsFromMilestone(string owner, string repo, string number, AsyncCallback callback, object state);
        IList<GithubLabel> EndGetLabelsFromMilestone(IAsyncResult asyncResult);
    }
}