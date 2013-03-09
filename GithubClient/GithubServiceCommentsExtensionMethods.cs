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
    public static class GithubServiceCommentsExtensionMethods
    {
        public static IList<GithubComment> GetCommentsFromIssue(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            return proxy.EndGetCommentsFromIssue(proxy.BeginGetCommentsFromIssue(owner, repo, number, null, null));
        }

        public static IList<GithubComment> GetCommentsFromRepo(this IGithubServiceManagement proxy, string owner, string repo)
        {
            return proxy.EndGetCommentsFromRepo(proxy.BeginGetCommentsFromRepo(owner, repo, null, null));
        }

        public static IList<GithubComment> GetCommentFromIssue(this IGithubServiceManagement proxy, string owner, string repo, string id)
        {
            return proxy.EndGetCommentFromIssue(proxy.BeginGetCommentFromIssue(owner, repo, id, null, null));
        }

        public static GithubComment CreateComment(this IGithubServiceManagement proxy, string owner, string repo, string number, GithubComment comment)
        {
            return proxy.EndCreateComment(proxy.BeginCreateComment(owner, repo, number, comment, null, null));
        }

        public static GithubComment BeginUpdateComment(this IGithubServiceManagement proxy, string owner, string repo, string number, GithubComment comment)
        {
            return proxy.EndUpdateComment(proxy.BeginUpdateComment(owner, repo, number, comment, null, null));
        }

        public static void DeleteComment(this IGithubServiceManagement proxy, string owner, string repo, string number)
        {
            proxy.EndDeleteComment(proxy.BeginDeleteComment(owner, repo, number, null, null));
        }
    }
}
