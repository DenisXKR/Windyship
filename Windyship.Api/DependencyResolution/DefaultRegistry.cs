// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Windyship.Api.DependencyResolution {
	using Microsoft.AspNet.Identity;
	using Microsoft.Owin.Security;
	using StructureMap.Configuration.DSL;
	using StructureMap.Graph;
	using System.Data.Entity;
	using System.Web;
	using Windyship.Api.Services.IdentitySvc;
	using Windyship.Core;
	using Windyship.Dal;
	using Windyship.Dal.Core;
	using Windyship.Entities;
	using Windyship.Repositories;
	
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });
            //For<IExample>().Use<Example>();

			For<DbContext>().Use<WindyContext>();
			For<IUnitOfWork>().Use<UnitOfWork>();
			For<IDataContext<User, int>>().Use<DataContext<User, int>>();
			For<IUserRepository>().Use<UserRepository>();

			For<UserManager<WindyUser, int>>().Use<WindyUserManager>();
			For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
			For<IUserStore<WindyUser, int>>().Use<WindyUserStore>();

			//Database.SetInitializer<WindyContext>(null);
        }

        #endregion
    }
}