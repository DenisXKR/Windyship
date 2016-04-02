using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Windyship.Api.Model.Api
{
	public sealed class ApiResponse
	{
		[JsonProperty("status")]
		public bool Status { get; set; }

		[JsonProperty("items")]
		public object Items { get; set; }


		public ApiResponse(bool code)
		{
			this.Status = code;
		}

		public ApiResponse(bool code, object data)
			: this(code)
		{
			this.Items = data;
		}
	}
}