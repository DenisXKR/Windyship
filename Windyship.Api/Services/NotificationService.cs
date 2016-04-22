using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Windyship.Repositories;

namespace Windyship.Api.Services
{
	public class NotificationService
	{
		private IDeviceTokenRepository _deviceTokenRepository;

		public NotificationService(IDeviceTokenRepository deviceTokenRepository)
		{
			_deviceTokenRepository = deviceTokenRepository;
		}
	}
}