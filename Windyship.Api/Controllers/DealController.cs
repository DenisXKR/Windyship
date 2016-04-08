﻿using System;
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
		public async Task<IHttpActionResult> GetCategories([FromUri]string language)
		{
			var result = await _categoryRepository.GetAllAsync(c => c.Language.ToString() == language);
			return ApiResult(true, result.Select(c => new { id = c.Id, title = c.Name }));
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

				foreach (var loc in request.To)
				{
					var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
					_locationToRepository.Add(locationTo);
				}

				foreach (var loc in request.From)
				{
					var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
					_locationFromRepository.Add(locationFrom);
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

					foreach (var loc in request.To)
					{
						var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
						_locationToRepository.Add(locationTo);
					}

					_locationFromRepository.RemoveRange(l => l.ShipmentId == shipment.Id);

					foreach (var loc in request.From)
					{
						var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, ShipmentId = shipment.Id };
						_locationFromRepository.Add(locationFrom);
					}

					await _unitOfWork.SaveChangesAsync();
					return ApiResult(true, new { shipment_id = shipment.Id });
				}
			}

			return ApiResult(false);
		}

		[Route("deleteShipment"), HttpPost]
		public async Task<IHttpActionResult> DeleteShipment(int shipment_id)
		{
			try
			{
				_locationToRepository.RemoveRange(l => l.ShipmentId == shipment_id);
				_locationFromRepository.RemoveRange(l => l.ShipmentId == shipment_id);
				_shipmentRepository.RemoveRange(s => s.Id == shipment_id);

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}
			catch (Exception ex)
			{
				return ApiResult(true, ex.Message);
			}
		}

		[Route("getRecommendedBudget"), HttpPost]
		public IHttpActionResult GetRecommendedBudget(LocationViewModel from, LocationViewModel to)
		{
			return ApiResult(true, new { Budget = 20, Currency = "USD" });
		}
	}
}
