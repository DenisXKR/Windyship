using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Windyship.Api.Model.Shipments;
using Windyship.Entities;
using Windyship.Repositories;
using Microsoft.AspNet.Identity;
using Windyship.Core;
using Windyship.Api.Model.Common;
using System.Device.Location;

namespace Windyship.Api.Controllers
{
	[RoutePrefix("api"), Authorize]
	public class DealController : BaseApiController
	{
		private readonly IUserRepository _userRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IShipmentRepository _shipmentRepository;

		private readonly ILocationFromRepository _locationFromRepository;
		private readonly ILocationToRepository _locationToRepository;

		private IUnitOfWork _unitOfWork;

		public DealController(IUserRepository userRepository, ICategoryRepository categoryRepository, IShipmentRepository shipmentRepository,
			ILocationFromRepository locationFromRepository, ILocationToRepository locationToRepository, IUnitOfWork unitOfWork)
		{
			_userRepository = userRepository;
			_categoryRepository = categoryRepository;
			_shipmentRepository = shipmentRepository;
			_locationFromRepository = locationFromRepository;
			_locationToRepository = locationToRepository;
			_unitOfWork = unitOfWork;
		}

		[Route("getCategories"), HttpGet]
		public IHttpActionResult GetCategories([FromUri]string language)
		{
			var result = _categoryRepository.All();
			return ApiResult(true, result.Select(c => new { id = c.Id, title = c.GetName(language) }));
		}

		[Route("sendShipment"), HttpPost]
		public async Task<IHttpActionResult> SendShipment(ShipmentRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			if (ModelState.IsValid)
			{
				var shipment = new Shipment
				{
					UserId = id,
					Budget = request.Budget,
					CategoryId = request.Category_id,
					Currency = request.Currency,
					DeleveryDate = request.Delevery_date,
					Description = request.Description,
					RecipiantMobile = request.Recipiant_mobile,
					RecipiantName = request.Recipiant_name,
					RecipiantSecoundary_mobile = request.Recipiant_secoundary_mobile,
					RecipiantSecoundary_name = request.Recipiant_secoundary_name,
					Size = request.Size,
					Title = request.Title,
					Weight = request.Weight
				};

				_shipmentRepository.Add(shipment);

				await _unitOfWork.SaveChangesAsync();

				if (request.To != null)
				{
					foreach (var loc in request.To)
					{
						var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
						_locationToRepository.Add(locationTo);
					}
				}

				if (request.From != null)
				{
					foreach (var loc in request.From)
					{
						var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
						_locationFromRepository.Add(locationFrom);
					}
				}

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true, new { shipment_id = shipment.Id });
			}
			else
			{
				return ApiResult(false);
			}
		}

		[Route("editShipment"), HttpPost]
		public async Task<IHttpActionResult> EditShipment(ShipmentRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			if (ModelState.IsValid)
			{
				var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Id);

				if (shipment != null)
				{
					shipment.UserId = id;
					shipment.Budget = request.Budget;
					shipment.CategoryId = request.Category_id;
					shipment.Currency = request.Currency;
					shipment.DeleveryDate = request.Delevery_date;
					shipment.Description = request.Description;
					shipment.RecipiantMobile = request.Recipiant_mobile;
					shipment.RecipiantName = request.Recipiant_name;
					shipment.RecipiantSecoundary_mobile = request.Recipiant_secoundary_mobile;
					shipment.RecipiantSecoundary_name = request.Recipiant_secoundary_name;
					shipment.Size = request.Size;
					shipment.Title = request.Title;
					shipment.Weight = request.Weight;

					_shipmentRepository.Update(shipment);
					_locationToRepository.RemoveRange(l => l.ShipmentId == shipment.Id);

					if (request.To != null)
					{
						foreach (var loc in request.To)
						{
							var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
							_locationToRepository.Add(locationTo);
						}
					}

					_locationFromRepository.RemoveRange(l => l.ShipmentId == shipment.Id);

					if (request.From != null)
					{
						foreach (var loc in request.From)
						{
							var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
							_locationFromRepository.Add(locationFrom);
						}
					}

					await _unitOfWork.SaveChangesAsync();
					return ApiResult(true, new { shipment_id = shipment.Id });
				}
			}

			return ApiResult(false);
		}

		[Route("deleteShipment"), HttpPost]
		public async Task<IHttpActionResult> DeleteShipment(DeleteShipmentRequest request)
		{
			try
			{
				_locationToRepository.RemoveRange(l => l.ShipmentId == request.Shipment_id);
				_locationFromRepository.RemoveRange(l => l.ShipmentId == request.Shipment_id);
				_shipmentRepository.RemoveRange(s => s.Id == request.Shipment_id);

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}
			catch (Exception ex)
			{
				return ApiResult(true, ex.Message);
			}
		}

		[Route("getRecommendedBudget"), HttpPost]
		public IHttpActionResult GetRecommendedBudget(Distance request)
		{
			var geoCoordinate = new GeoCoordinate(Convert.ToDouble(request.From.Lat), Convert.ToDouble(request.From.Long));
			var distance = geoCoordinate.GetDistanceTo(new GeoCoordinate(Convert.ToDouble(request.To.Lat), Convert.ToDouble(request.To.Long)));

			return ApiResult(true, new { Budget = Math.Round(distance * 0.001), Currency = "USD" });
		}

		[Route("uploadShipmentImage"), HttpPost]
		public async Task<IHttpActionResult> UploadShipmentImage([FromUri]int shipmentId, [FromUri]int imageId)
		{
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == shipmentId);

			if (shipment == null) return ApiResult(false, "Shipment not found");

			if (!Request.Content.IsMimeMultipartContent())
			{
				var provider = new MultipartMemoryStreamProvider();
				await Request.Content.ReadAsMultipartAsync(provider);

				foreach (var file in provider.Contents)
				{
					var data = await file.ReadAsByteArrayAsync();

					switch (imageId)
					{
						case 1: shipment.Image1 = data; break;
						case 2: shipment.Image2 = data; break;
						case 3: shipment.Image3 = data; break;
					}
				}

				await _unitOfWork.SaveChangesAsync();
			}

			return ApiResult(true);
		}



	}
}
