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
// ---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using GithubClient;
using GithubClient.Model;

namespace ConsoleApplication1.Repositories
{
    namespace Remondo.Database.Repositories
    {
        public class Repository<T> : IRepository<T> where T : class, IGithubEntity
        {
            protected IDictionary<string, T> Entries { get; set; }
            protected IGithubServiceManagement GithubChannel { get; set; }

            public Repository(IGithubServiceManagement githubChannel)
            {
                Entries = new Dictionary<string, T>();
                GithubChannel = githubChannel;
            }

            #region IRepository<T> Members

            public void Insert(T entity)
            {
                Entries.Add(entity.Id, entity);
            }

            public void Delete(T entity)
            {
                Entries.Remove(entity.Id);
            }

            public IQueryable<T> SearchFor(Func<T, bool> predicate)
            {
                return Entries.Values.Where(predicate).AsQueryable();
            }

            public IQueryable<T> GetAll()
            {
                return Entries.Values.AsQueryable();
            }

            public T GetById(string id)
            {
                return Entries.Single(e => e.Key.Equals(id)).Value;
            }

            #endregion
        }
    }
}
