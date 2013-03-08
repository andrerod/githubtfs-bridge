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
using System.ServiceModel;
using System.ServiceModel.Web;

namespace GithubClient
{
    public static class GithubClient
    {
        private static readonly Dictionary<string, WebChannelFactory<IGithubServiceManagement>> Factories =
            new Dictionary<string, WebChannelFactory<IGithubServiceManagement>>();

        public static IGithubServiceManagement CreateChannel(string username, string password)
        {
            return CreateChannel(new Uri("https://api.github.com"), username, password);
        }

        public static IGithubServiceManagement CreateChannel(Uri remoteUri, string username, string password)
        {
            WebChannelFactory<IGithubServiceManagement> factory;
            if (Factories.ContainsKey(remoteUri.ToString()))
            {
                factory = Factories[remoteUri.ToString()];
            }
            else
            {
                factory = new WebChannelFactory<IGithubServiceManagement>(remoteUri);
                factory.Endpoint.Behaviors.Add(new GithubAutHeaderInserter { Username = username, Password = password });

                var wb = factory.Endpoint.Binding as WebHttpBinding;
                if (wb != null)
                {
                    wb.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                    wb.Security.Mode = WebHttpSecurityMode.Transport;
                    wb.MaxReceivedMessageSize = 10000000;
                }

                if (factory.Credentials != null)
                {
                    if (!string.IsNullOrEmpty(username))
                    {
                        factory.Credentials.UserName.UserName = username;
                    }

                    if (!string.IsNullOrEmpty(password))
                    {
                        factory.Credentials.UserName.Password = password;
                    }
                }

                Factories[remoteUri.ToString()] = factory;
            }

            return factory.CreateChannel();
        }
    }
}
