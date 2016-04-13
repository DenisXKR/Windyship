using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Windyship.Repositories;

namespace Windyship.Api.Controllers
{
    public class ImageController : Controller
    {
		private readonly IUserRepository _userRepository;
		private readonly IShipmentRepository _shipmentRepository;

		public ImageController(IUserRepository userRepository, IShipmentRepository shipmentRepository)
		{
			_userRepository = userRepository;
			_shipmentRepository = shipmentRepository;
		}

		/*
		public ActionResult Index()
		{
			var result = new FilePathResult("~/index.html", "text/html");
			return result;
		}*/

		//[Authorize]
        public async Task<ActionResult> Avatar(int id)
        {
			var userId = User.Identity.GetUserId<int>();
			var user = await _userRepository.GetFirstOrDefaultAsync(u => u.Id == id);

			if (user == null)
			{
				return HttpNotFound();
			}

			return new FileContentResult(user.Avatar, "image/png");
        }

		//[Authorize]
		public async Task<ActionResult> ShipmentImage(int shipmentId, int imageId)
		{
			var userId = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(u => u.Id == shipmentId);

			if (shipment == null)
			{
				return HttpNotFound();
			}

			byte[] image = null;

			switch (imageId)
			{
				case 1: image = shipment.Image1; break;
				case 2: image = shipment.Image2; break;
				case 3: image = shipment.Image3; break;
			}

			if (image != null) return new FileContentResult(image, "image/png");

			return HttpNotFound();
		}
    }
}