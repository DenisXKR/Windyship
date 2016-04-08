using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Windyship.Api.Model.Account;
using Windyship.Core;
using Windyship.Entities;
using Windyship.Repositories;

namespace Windyship.Api.Controllers
{
	[RoutePrefix("api")]
	public class HelpController : BaseApiController
	{
		private readonly IContentRepository _contentRepository;
		private readonly IFaqRepository _faqRepository;
		private readonly IUserRequestRepository _userRequestRepository;
		private readonly IUnitOfWork _unitOfWork;

		public HelpController(IContentRepository contentRepository, IFaqRepository faqRepository, IUserRequestRepository userRequestRepository, IUnitOfWork unitOfWork)
		{
			_contentRepository = contentRepository;
			_faqRepository = faqRepository;
			_userRequestRepository = userRequestRepository;
			_unitOfWork = unitOfWork;
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

		[Route("faq"), HttpGet]
		public async Task<IHttpActionResult> GetFaq([FromUri]string language)
		{
			var result = await _faqRepository.GetAllAsync(f => f.Language.ToString() == language);
			return ApiResult(true, result.Select(f => new 
			{ 
				Question = f.Question, 
				Answer = f.Answer 
			}));
		}

		[Route("contact"), HttpPost]
		public async Task<IHttpActionResult> PostUserRequest(UserRequestViewModel userRequest)
		{
			try
			{
				_userRequestRepository.Add(new UserRequest
				{
					Name = userRequest.Name,
					Email = userRequest.Email,
					Request = userRequest.Message,
					Date = DateTime.Now
				});

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}
			catch (Exception)
			{
				return ApiResult(false);
			}
		}

		[Route("test"), HttpGet]
		public IHttpActionResult Test()
		{
			return ApiResult(true, new
			{
				message = "Hello"
			});
		}
	}
}
