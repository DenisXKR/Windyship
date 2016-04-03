using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Windyship.Entities;
using Windyship.Repositories;

namespace Windyship.Api.Controllers
{
	[RoutePrefix("api")]
	public class HelpController : BaseApiController
	{
		private readonly IContentRepository _contentRepository;

		public HelpController(IContentRepository contentRepository)
		{
			_contentRepository = contentRepository;
		}

		[Route("about"), HttpGet]
		public async Task<IHttpActionResult> About([FromUri]string language)
		{
			var result = await _contentRepository.GetAllAsync(c => c.Language.ToString() == language && c.ContentPart == ContentPart.About);
			return ApiResult(true, new
			{
				message = string.Join(Environment.NewLine, result.Select(c => c.Text))
			});
		}

		[Route("legal"), HttpGet]
		public async Task<IHttpActionResult> Legal([FromUri]string language)
		{
			var result = await _contentRepository.GetAllAsync(c => c.Language.ToString() == language && c.ContentPart == ContentPart.LegalPrivacy);
			return ApiResult(true, new
			{
				message = string.Join(Environment.NewLine, result.Select(c => c.Text))
			});
		}
	}
}
