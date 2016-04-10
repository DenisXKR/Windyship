using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Windyship.Repositories;

namespace Windyship.Api.Controllers
{
    public class ImageController : Controller
    {
		private readonly IUserRepository _userRepository;

		public ImageController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public ActionResult Index()
		{
			var result = new FilePathResult("~/index.html", "text/html");
			return result;
		}

		[AllowAnonymous]
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
    }
}