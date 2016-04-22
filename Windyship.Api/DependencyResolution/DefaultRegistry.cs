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

namespace Windyship.Api.DependencyResolution
{
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

	public class DefaultRegistry : Registry
	{
		#region Constructors and Destructors

		public DefaultRegistry()
		{
			Scan(
				scan =>
				{
					scan.TheCallingAssembly();
					scan.WithDefaultConventions();
				});

			For<DbContext>().Use<WindyContext>();
			For<IUnitOfWork>().Use<UnitOfWork>();
			For<IDataContext<User, int>>().Use<DataContext<User, int>>();
			For<IDataContext<Content, int>>().Use<DataContext<Content, int>>();
			For<IDataContext<Faq, int>>().Use<DataContext<Faq, int>>();
			For<IDataContext<UserRequest, int>>().Use<DataContext<UserRequest, int>>();
			For<IDataContext<Category, int>>().Use<DataContext<Category, int>>();
			For<IDataContext<LocationTo, int>>().Use<DataContext<LocationTo, int>>();
			For<IDataContext<LocationFrom, int>>().Use<DataContext<LocationFrom, int>>();
			For<IDataContext<TravelTo, int>>().Use<DataContext<TravelTo, int>>();
			For<IDataContext<TravelFrom, int>>().Use<DataContext<TravelFrom, int>>();
			For<IDataContext<DisabledCategories, int>>().Use<DataContext<DisabledCategories, int>>();
			For<IDataContext<UserPhone, int>>().Use<DataContext<UserPhone, int>>();
			For<IDataContext<DeviceToken, int>>().Use<DataContext<DeviceToken, int>>();

			For<IDataContext<CarryTravel, int>>().Use<DataContext<CarryTravel, int>>();
			For<IDataContext<Shipment, int>>().Use<DataContext<Shipment, int>>();
			For<IDataContext<CarrierReview, int>>().Use<DataContext<CarrierReview, int>>();
			For<IDataContext<Notification, int>>().Use<DataContext<Notification, int>>();
			For<IDataContext<InterestedShipment, int>>().Use<DataContext<InterestedShipment, int>>();

			For<IUserRepository>().Use<UserRepository>();
			For<IContentRepository>().Use<ContentRepository>();
			For<IFaqRepository>().Use<FaqRepository>();
			For<IUserRequestRepository>().Use<UserRequestRepository>();
			For<ICategoryRepository>().Use<CategoryRepository>();
			For<ILocationToRepository>().Use<LocationToRepository>();
			For<ILocationFromRepository>().Use<LocationFromRepository>();
			For<ITravelFromRepository>().Use<TravelFromRepository>();
			For<ITravelToRepository>().Use<TravelToRepository>();
			For<IDisabledCategoriesRepository>().Use<DisabledCategoriesRepository>();
			For<ICarrierReviewRepository>().Use<CarrierReviewRepository>();
			For<IUserPhoneRepository>().Use<UserPhoneRepository>();
			For<INotificationRepository>().Use<NotificationRepository>();
			For<IDeviceTokenRepository>().Use<DeviceTokenRepository>();
			For<IInterestedShipmentRepository>().Use<InterestedShipmentRepository>();

			For<IShipmentRepository>().Use<ShipmentRepository>();
			For<ICarryTravelRepository>().Use<CarryTravelRepository>();

			For<UserManager<WindyUser, int>>().Use<WindyUserManager>();
			For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
			For<IUserStore<WindyUser, int>>().Use<WindyUserStore>();

			//Database.SetInitializer<WindyContext>(null);
		}

		#endregion
	}
}