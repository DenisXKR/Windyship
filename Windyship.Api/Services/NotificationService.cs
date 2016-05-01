using MoonAPNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windyship.Core;
using Windyship.Entities;
using Windyship.Repositories;

namespace Windyship.Api.Services
{
	public class NotificationService : INotificationService
	{
		private readonly IDeviceTokenRepository _deviceTokenRepository;
		private readonly INotificationRepository _notificationRepository;
		private readonly IUserRepository _userRepository;
		private IUnitOfWork _unitOfWork;

		public NotificationService(IDeviceTokenRepository deviceTokenRepository, INotificationRepository notificationRepository,
			IUserRepository userRepository, IUnitOfWork unitOfWork)
		{
			_deviceTokenRepository = deviceTokenRepository;
			_notificationRepository = notificationRepository;
			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<string> SendNotice(int fromUserId, int toUserId, Shipment shipment, int type)
		{
			var user = _userRepository.GetById(fromUserId);

			var notice = new Notification();
			notice.UserId = toUserId;
			_notificationRepository.Add(notice);
			await _unitOfWork.SaveChangesAsync();
			notice.SetData(shipment, type, user);
			await _unitOfWork.SaveChangesAsync();

			var tokens = await _deviceTokenRepository.GetAllAsync(t => t.UserId == toUserId);

			string result = null;

			foreach (var token in tokens)
			{
				switch (token.DeviceType)
				{
					case DeviceType.Android: await this.SendAndroidNotification(token.Token, notice.Data); break;
					case DeviceType.Ios: this.SendIosNotification(token.Token, notice.Data); break;
				}
			}

			return result;
		}

		protected string SendIosNotification(string deviceId, string strPushMessage)
		{
			try
			{
				var payload1 = new NotificationPayload(deviceId, strPushMessage, 1, "default");
				payload1.AddCustom("RegionID", "IDQ10150");
				var p = new List<NotificationPayload> { payload1 };
				string certificatePath = HttpContext.Current.Server.MapPath("/App_Data/aps_development.cer");
				var push = new PushNotification(true, certificatePath, "Windy#2016");
				string strfilename = push.P12File;
				var message1 = push.SendToApple(p);
				return string.Join(",", message1.ToArray());
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		protected async Task<string> SendAndroidNotification(string deviceId, string message)
		{
			try
			{
				string GoogleAppID = "AIzaSyBdBRcsZGQcfyXxO2CjkI1RQ4NqE0rRltE";
				var SENDER_ID = "140456046687";
				var value = message;
				WebRequest tRequest;
				tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
				tRequest.Method = "post";
				tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
				tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));

				tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

				string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" +
				System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";

				//Console.WriteLine(postData);
				Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
				tRequest.ContentLength = byteArray.Length;

				Stream dataStream = tRequest.GetRequestStream();
				dataStream.Write(byteArray, 0, byteArray.Length);
				dataStream.Close();

				WebResponse tResponse = tRequest.GetResponse();

				dataStream = tResponse.GetResponseStream();

				StreamReader tReader = new StreamReader(dataStream);

				String sResponseFromServer = await tReader.ReadToEndAsync();

				tReader.Close();
				dataStream.Close();
				tResponse.Close();
				return sResponseFromServer;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}