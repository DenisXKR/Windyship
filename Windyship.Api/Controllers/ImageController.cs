using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Windyship.Repositories;
using System.Threading.Tasks;

namespace Windyship.Api.Controllers
{
    public class ImageController : Controller
    {
		private readonly IUserRepository _userRepository;

		public ImageController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		[Authorize]
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