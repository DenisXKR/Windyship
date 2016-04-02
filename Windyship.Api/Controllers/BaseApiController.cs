using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Windyship.Api.Model.Api;

namespace Windyship.Api.Controllers
{
	public abstract class BaseApiController : ApiController
	{
		protected IHttpActionResult ApiResult(bool result)
		{
			var responseModel = new ApiResponse(result);
			return Ok(responseModel);
		}

		protected IHttpActionResult ApiResult( bool result, object data)
		{
			var responseModel = new ApiResponse(result, data);
			return Ok(responseModel);
		}

	}
}