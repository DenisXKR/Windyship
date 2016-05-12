using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Windyship.Api.Model.Common;
using Windyship.Api.Model.Shipments;
using Windyship.Api.Services;
using Windyship.Api.Services.IdentitySvc;
using Windyship.Core;
using Windyship.Entities;
using Windyship.Repositories;

namespace Windyship.Api.Controllers
{
	[RoutePrefix("api"), Authorize]
	public class DealController : BaseApiController
	{
		private readonly IUserRepository _userRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IShipmentRepository _shipmentRepository;
		private readonly ICarryTravelRepository _carryTravelRepository;
		private readonly IInterestedShipmentRepository _interestedShipmentRepository;

		private readonly ILocationFromRepository _locationFromRepository;
		private readonly ILocationToRepository _locationToRepository;

		private readonly ITravelFromRepository _travelFromRepository;
		private readonly ITravelToRepository _travelToRepository;
		private readonly IDisabledCategoriesRepository _disabledCategoriesRepository;
		private readonly ICarrierReviewRepository _carrierReviewRepository;
		private readonly INotificationRepository _notificationRepository;
		private readonly IArhivedShipmentRepository _arhivedShipmentRepository;

		private readonly INotificationService _notificationService;

		private IUnitOfWork _unitOfWork;

		public DealController(IUserRepository userRepository, ICategoryRepository categoryRepository, IShipmentRepository shipmentRepository,
			ILocationFromRepository locationFromRepository, ILocationToRepository locationToRepository, ITravelFromRepository travelFromRepository,
			ITravelToRepository travelToRepository, IDisabledCategoriesRepository disabledCategoriesRepository, ICarrierReviewRepository carrierReviewRepository,
			ICarryTravelRepository carryTravelRepository, INotificationRepository notificationRepository, IInterestedShipmentRepository interestedShipmentRepository,
			INotificationService notificationService, IArhivedShipmentRepository arhivedShipmentRepository, IUnitOfWork unitOfWork)
		{
			_userRepository = userRepository;
			_categoryRepository = categoryRepository;
			_shipmentRepository = shipmentRepository;
			_locationFromRepository = locationFromRepository;
			_locationToRepository = locationToRepository;
			_travelFromRepository = travelFromRepository;
			_travelToRepository = travelToRepository;
			_disabledCategoriesRepository = disabledCategoriesRepository;
			_carryTravelRepository = carryTravelRepository;
			_carrierReviewRepository = carrierReviewRepository;
			_notificationRepository = notificationRepository;
			_interestedShipmentRepository = interestedShipmentRepository;
			_notificationService = notificationService;
			_arhivedShipmentRepository = arhivedShipmentRepository;

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
					PostDate = DateTime.Now,
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
					Weight = request.Weight,
					ShipmentStatus = ShipmentStatus.PostShipmentRequest
				};

				_shipmentRepository.Add(shipment);

				await _unitOfWork.SaveChangesAsync();

				if (request.To != null)
				{
					foreach (var loc in request.To)
					{
						var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, ShipmentId = shipment.Id };
						_locationToRepository.Add(locationTo);
					}
				}

				if (request.From != null)
				{
					foreach (var loc in request.From)
					{
						var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, ShipmentId = shipment.Id };
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
				var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Id && s.UserId == id && (int)s.ShipmentStatus < 3);

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
							var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, ShipmentId = shipment.Id };
							_locationToRepository.Add(locationTo);
						}
					}

					_locationFromRepository.RemoveRange(l => l.ShipmentId == shipment.Id);

					if (request.From != null)
					{
						foreach (var loc in request.From)
						{
							var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, ShipmentId = shipment.Id };
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
			var id = User.Identity.GetUserId<int>();

			try
			{
				var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.UserId == id && s.Id == request.Shipment_id && (int)s.ShipmentStatus < 3);

				if (shipment != null)
				{
					_locationToRepository.RemoveRange(l => l.ShipmentId == request.Shipment_id);
					_locationFromRepository.RemoveRange(l => l.ShipmentId == request.Shipment_id);
					_interestedShipmentRepository.RemoveRange(i => i.ShipmentId == request.Shipment_id);
					await _unitOfWork.SaveChangesAsync();

					_shipmentRepository.RemoveRange(s => s.Id == request.Shipment_id);
					await _unitOfWork.SaveChangesAsync();

					return ApiResult(true);
				}

				return ApiResult(false);
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
			var id = User.Identity.GetUserId<int>();

			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == shipmentId && s.UserId == id);

			if (shipment == null) return ApiResult(false, "Shipment not found");

			if (Request.Content.IsMimeMultipartContent())
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

		[Route("delShipmentImage"), HttpPost]
		public async Task<IHttpActionResult> DelShipmentImage([FromUri]int shipmentId, [FromUri]int imageId)
		{
			var id = User.Identity.GetUserId<int>();

			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == shipmentId && s.UserId == id);
			if (shipment == null) return ApiResult(false, "Shipment not found");

			switch (imageId)
			{
				case 1: shipment.Image1 = null; break;
				case 2: shipment.Image2 = null; break;
				case 3: shipment.Image3 = null; break;
			}

			await _unitOfWork.SaveChangesAsync();
			return ApiResult(true);
		}

		[Route("addCarryTravel"), HttpPost]
		public async Task<IHttpActionResult> AddCarryTravel([FromBody] CarryTravelRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			if (ModelState.IsValid)
			{
				//var days = JsonConvert.SerializeObject(request.repeat_days);

				var carryTravel = new CarryTravel
				{
					UserId = id,
					ArrivalBefore = request.arrival_before,
					MaxSize = request.max_size,
					MaxWeight = request.max_weight,
					RepeatDays = request.repeat_days != null ? string.Join(",", request.repeat_days) : null,
					TravelingDate = request.traveling_date,
					Active = true
				};

				_carryTravelRepository.Add(carryTravel);
				await _unitOfWork.SaveChangesAsync();

				if (request.To != null)
				{
					foreach (var loc in request.To)
					{
						var locationTo = new TravelTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, CarryTraveId = carryTravel.Id };
						_travelToRepository.Add(locationTo);
					}
				}

				if (request.From != null)
				{
					foreach (var loc in request.From)
					{
						var locationFrom = new TravelFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, CarryTraveId = carryTravel.Id };
						_travelFromRepository.Add(locationFrom);
					}
				}

				if (request.not_allowed_categories != null)
				{
					foreach (var cat in request.not_allowed_categories)
					{
						var disabledCategories = new DisabledCategories { CategoryId = cat, CarryTravelId = carryTravel.Id };
						_disabledCategoriesRepository.Add(disabledCategories);
					}
				}

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true, new { travel_id = carryTravel.Id });
			}

			return ApiResult(false);
		}

		[Route("editCarryTravel"), HttpPost]
		public async Task<IHttpActionResult> EditCarryTravel([FromBody] CarryTravelRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			var carryTravel = await _carryTravelRepository.GetFirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == id);

			if (carryTravel != null)
			{
				carryTravel.Active = true;
				carryTravel.ArrivalBefore = request.arrival_before;
				carryTravel.MaxSize = request.max_size;
				carryTravel.MaxWeight = request.max_weight;
				carryTravel.RepeatDays = request.repeat_days != null ? string.Join(",", request.repeat_days) : null;
				carryTravel.TravelingDate = request.traveling_date;

				_carryTravelRepository.Update(carryTravel);

				_travelToRepository.RemoveRange(l => l.CarryTraveId == carryTravel.Id);

				if (request.To != null)
				{
					foreach (var loc in request.To)
					{
						var locationTo = new TravelTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, CarryTraveId = carryTravel.Id };
						_travelToRepository.Add(locationTo);
					}
				}

				_travelFromRepository.RemoveRange(l => l.CarryTraveId == carryTravel.Id);

				if (request.From != null)
				{
					foreach (var loc in request.From)
					{
						var locationFrom = new TravelFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, City = loc.City, Address = loc.Address, CarryTraveId = carryTravel.Id };
						_travelFromRepository.Add(locationFrom);
					}
				}

				_disabledCategoriesRepository.RemoveRange(c => c.CarryTravelId == carryTravel.Id);

				if (request.not_allowed_categories != null)
				{
					foreach (var cat in request.not_allowed_categories)
					{
						var disabledCategories = new DisabledCategories { CategoryId = cat, CarryTravelId = carryTravel.Id };
						_disabledCategoriesRepository.Add(disabledCategories);
					}
				}

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true, new { travel_id = carryTravel.Id });
			}

			return ApiResult(false);
		}

		[Route("deleteCarryTravel"), HttpPost]
		public async Task<IHttpActionResult> DeleteCarryTravel(DeleteTravelRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			try
			{
				var shipment = await _carryTravelRepository.GetFirstOrDefaultAsync(s => s.UserId == id && s.Id == request.travel_id);

				if (shipment != null)
				{
					_travelFromRepository.RemoveRange(l => l.CarryTraveId == request.travel_id);
					_travelToRepository.RemoveRange(l => l.CarryTraveId == request.travel_id);
					_disabledCategoriesRepository.RemoveRange(l => l.CarryTravelId == request.travel_id);
					_carryTravelRepository.RemoveRange(s => s.Id == request.travel_id);

					await _unitOfWork.SaveChangesAsync();

					return ApiResult(true);
				}

				return ApiResult(false);
			}
			catch (Exception ex)
			{
				return ApiResult(true, ex.Message);
			}
		}

		[Route("review"), HttpPost]
		public async Task<IHttpActionResult> SetReview(SetReviewRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.UserId == id && (s.ShipmentStatus == ShipmentStatus.DeliveredToReceipt || s.ShipmentStatus == ShipmentStatus.NotDelivered));

			var user = _userRepository.GetById(id);

			if (shipment != null && user != null)
			{
				shipment.ShipmentStatus = ShipmentStatus.Review;

				var review = new CarrierReview
				{
					ShipmentId = shipment.Id,
					Comment = request.Comment,
					Rate = request.Rate
				};

				_carrierReviewRepository.Add(review);
				_unitOfWork.SaveChanges();

				await _notificationService.SendNotice(id, shipment.CarrierId.Value, shipment, 6);

				var rate = _carrierReviewRepository.GetQuery(u => u.Shipment.CarrierId == shipment.CarrierId).Average(r => (decimal?)r.Rate);
				shipment.Carrier.CarrierRating = rate ?? 0;
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("getCarriersForShipment"), HttpPost]
		public async Task<IHttpActionResult> GetCarriersForShipment(CarriersForShipmentReqiest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id && s.UserId == id);

			if (shipment != null)
			{
				var fromShipment = shipment.From.FirstOrDefault();
				var toShipment = shipment.To.FirstOrDefault();

				if (fromShipment == null || toShipment == null) return ApiResult(false, "Empty shipment location");

				var @from = DbGeography.FromText(String.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", fromShipment.Long, fromShipment.Lat), 4326);
				var @to = DbGeography.FromText(String.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", toShipment.Long, toShipment.Lat), 4326);

				var fromPoints = _travelFromRepository.GetQuery(t => t.CarryTravel.UserId != id && t.CarryTravel.Active &&
					t.CarryTravel.TravelingDate >= DateTime.Now && t.Country == fromShipment.Country && t.City == fromShipment.City);

				var toPoints = _travelToRepository.GetQuery(t => t.CarryTravel.UserId != id && t.CarryTravel.Active &&
					t.CarryTravel.TravelingDate >= DateTime.Now && t.Country == toShipment.Country && t.City == toShipment.City);

				var distance = from f in fromPoints
							   join t in toPoints on f.CarryTraveId equals t.CarryTraveId
							   select new
							   {
								   Id = f.CarryTraveId,
								   TravelFrom = f.CarryTravel,
								   FromPoint = f,
								   ToPoin = t,
								   Distanse = @from.Distance(DbGeography.FromText("POINT(" + t.Long.ToString() + " " + t.Lat.ToString() + ")", 4326)) +
											  @to.Distance(DbGeography.FromText("POINT(" + t.Long.ToString() + " " + t.Lat.ToString() + ")", 4326))
							   };

				var travels = distance.ToList();

				var carriers = travels.Select(c => new CarrierViewModel
				{
					TravelId = c.TravelFrom.Id,
					CarrierId = c.TravelFrom.UserId,
					Delivery_date = c.TravelFrom.ArrivalBefore,
					From = c.FromPoint != null ?
						new LocationViewModel
						{
							Lat = c.FromPoint.Lat,
							Long = c.FromPoint.Long,
							Country = c.FromPoint.Country,
							City = c.FromPoint.City ,
							Address = c.FromPoint.Address
						} : null,
					To = c.ToPoin != null ?
						new LocationViewModel
						{
							Lat = c.ToPoin.Lat,
							Long = c.ToPoin.Long,
							Country = c.ToPoin.Country,
							City = c.FromPoint.City,
							Address = c.ToPoin.Address
						} : null,
					Image = string.Format("image/avatar?id={0}", c.TravelFrom.UserId),
					Mobile = c.TravelFrom.User.Phone,
					Name = c.TravelFrom.User.FirstName,
					Rating = c.TravelFrom.User.CarrierRating,
					CarrierRestrictions = c.TravelFrom.DisabledCategories.Select(g => g.Category.GetName(request.Language)),
					Distance = c.Distanse.Value
				});
				
				IEnumerable<CarrierViewModel> sortedCarriers = null;

				switch (request.Sort)
				{
					case "soonest": sortedCarriers = carriers.OrderBy(s => s.Delivery_date); break;
					case "rate": sortedCarriers = carriers.OrderBy(s => s.Rating); break;
					default: sortedCarriers = carriers.OrderBy(s => s.Distance); break;
				}

				return ApiResult(true, sortedCarriers);
			}
			else return ApiResult(false);
		}

		[Route("getTravels"), HttpPost]
		public async Task<IHttpActionResult> GetTravels(GetTravelsRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var now = DateTime.Now.Date;

			var travels = await _carryTravelRepository.GetAllAsync(t => t.UserId == id);
			var pagedTrevels = travels.OrderByDescending(t => t.TravelingDate).Skip((request.Page - 1) * 20).Take(20).ToList();

			var result = pagedTrevels.Select(t => new CarryTravelViewModel
			{
				Id = t.Id,
				arrival_before = t.ArrivalBefore,
				repeat_days = t.RepeatDays != null ? t.RepeatDays.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null,
				not_allowed_categories = t.DisabledCategories.Select(c => c.CategoryId),
				not_allowed_categories_str = t.DisabledCategories.Select(c => c.Category.GetName(request.language)),
				From = t.From.Select(l => new TravelFrom { Lat = l.Lat, Long = l.Long, Country = l.Country, Address = l.Address }),
				To = t.To.Select(l => new TravelFrom { Lat = l.Lat, Long = l.Long, Country = l.Country, Address = l.Address }),
				max_size = t.MaxSize,
				max_weight = t.MaxWeight,
				traveling_date = t.TravelingDate,
				Carrier = new SmallUserViewModel
				{
					id = t.UserId,
					name = t.User.FirstName,
					image = t.User.Avatar != null ? string.Format("Image/Avatar?id={0}", t.UserId) : null,
				}
			});

			return Ok(new
			{
				status = true,
				pages_count = Math.Ceiling((float)travels.Count() / 20),
				page = request.Page,
				items = result
			});
		}

		[Route("hireCarrier"), HttpPost]
		public async Task<IHttpActionResult> HireCarrier(HireCarrierRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.UserId == id && (s.ShipmentStatus == ShipmentStatus.PostShipmentRequest));

			if (shipment != null)
			{
				shipment.CarrierId = request.Carrier_id;
				shipment.ShipmentStatus = ShipmentStatus.HireCarrier;

				await _notificationService.SendNotice(shipment.UserId, shipment.CarrierId.Value, shipment, 1);

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("rejectHire"), HttpPost]
		public async Task<IHttpActionResult> RejectHire(RejectHireRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.UserId == id && s.ShipmentStatus == ShipmentStatus.HireCarrier);

			if (shipment != null)
			{
				await _notificationService.SendNotice(shipment.UserId, shipment.CarrierId.Value, shipment, 11);
				shipment.CarrierId = null;
				shipment.ShipmentStatus = ShipmentStatus.PostShipmentRequest;
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.CarrierId == id && s.ShipmentStatus == ShipmentStatus.HireCarrier);

			if (shipment != null)
			{
				await _notificationService.SendNotice(shipment.CarrierId.Value, shipment.UserId, shipment, 11);
				shipment.CarrierId = null;
				shipment.ShipmentStatus = ShipmentStatus.PostShipmentRequest;
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("interest"), HttpPost]
		public async Task<IHttpActionResult> Interest(InterestRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			if (!_carryTravelRepository.Any(c => c.UserId == id && c.Active)) return ApiResult(false);

			var shipmentIn = await _interestedShipmentRepository.GetFirstOrDefaultAsync(s => s.ShipmentId == request.shipment_id && s.UserId == id);

			if (shipmentIn != null)
			{
				if (!request.interest)
				{
					_interestedShipmentRepository.Remove(shipmentIn);
					await _unitOfWork.SaveChangesAsync();
					return ApiResult(true);
				}

				return ApiResult(false);
			}

			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.shipment_id && s.UserId != id &&
				s.ShipmentStatus == ShipmentStatus.PostShipmentRequest);

			if (shipment != null)
			{
				var inShip = new InterestedShipment
				{
					ShipmentId = request.shipment_id,
					UserId = id
				};

				_interestedShipmentRepository.Add(inShip);
				await _notificationService.SendNotice(id, shipment.UserId, shipment, 7);

				await _unitOfWork.SaveChangesAsync();
				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("acceptShipment"), HttpPost]
		public async Task<IHttpActionResult> AcceptShipment(AcceptShipmentRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.CarrierId == id && s.ShipmentStatus == ShipmentStatus.HireCarrier);

			if (shipment != null)
			{
				shipment.PinCode = TokenGenerator.GetUniqueDigits(6);

				WindySmsService.SendMessage(String.Format("Shipment pincode : {0}", shipment.PinCode), shipment.RecipiantMobile);

				shipment.ShipmentStatus = ShipmentStatus.AcceptShipmentRequest;

				await _notificationService.SendNotice(id, shipment.UserId, shipment, 2);

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("deliveredToCarrier"), HttpPost]
		public async Task<IHttpActionResult> DeliveredToCarrier(DeliveredToCarrierRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.UserId == id && s.ShipmentStatus == ShipmentStatus.AcceptShipmentRequest);

			if (shipment != null)
			{
				shipment.ShipmentStatus = ShipmentStatus.DeliveredToCarrier;

				await _notificationService.SendNotice(id, shipment.CarrierId.Value, shipment, 4);

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("deliveredToRecipient"), HttpPost]
		public async Task<IHttpActionResult> DeliveredToRecipient(DeliveredToRecipientRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.UserId == id && s.ShipmentStatus == ShipmentStatus.DeliveredToCarrier);

			if (shipment != null)
			{
				shipment.ShipmentStatus = ShipmentStatus.DeliveredToReceipt;
				shipment.DeleveryDate = DateTime.Now;
				await _notificationService.SendNotice(id, shipment.CarrierId.Value, shipment, 5);
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.CarrierId == id && s.ShipmentStatus == ShipmentStatus.DeliveredToCarrier);

			if (shipment != null && shipment.PinCode == request.Pin_code)
			{
				shipment.ShipmentStatus = ShipmentStatus.DeliveredToReceipt;
				shipment.DeleveryDate = DateTime.Now;
				await _notificationService.SendNotice(id, shipment.CarrierId.Value, shipment, 5);
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("getMyShipments"), HttpPost]
		public IHttpActionResult GetMyShipments(GetMyShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			IEnumerable<Shipment> shipments = null;

			if (request.View_as == "carrier")
			{
				switch (request.Type)
				{
					case "interested":
						var interested = _interestedShipmentRepository.All().Where(i => i.UserId == id).Select(si => si.ShipmentId).ToList();
						shipments = _shipmentRepository.GetQuery(s => s.ShipmentStatus != ShipmentStatus.Cancelled && interested.Contains(s.Id));
						break;

					case "in-progress":
						shipments = _shipmentRepository.GetQuery(s => s.CarrierId == id &&
							(s.ShipmentStatus == ShipmentStatus.AcceptShipmentRequest || s.ShipmentStatus == ShipmentStatus.DeliveredToCarrier ||
							 s.ShipmentStatus == ShipmentStatus.DeliveredToReceipt || s.ShipmentStatus == ShipmentStatus.HireCarrier));
						break;

					case "history":
						shipments = from s in _shipmentRepository.All()
									where s.CarrierId == id && !_arhivedShipmentRepository.Any(a => a.UserId == id && s.Id == a.ShipmentId) &&
									(s.ShipmentStatus == ShipmentStatus.Review || s.ShipmentStatus == ShipmentStatus.NotDelivered)
									select s;
						break;
				}
			}
			else //"sender"
			{
				switch (request.Type)
				{
					case "requested":
						shipments = _shipmentRepository.GetQuery(s => s.UserId == id &&
							(s.ShipmentStatus == ShipmentStatus.HireCarrier || s.ShipmentStatus == ShipmentStatus.PostShipmentRequest));
						break;

					case "in-progress":
						shipments = _shipmentRepository.GetQuery(s => s.UserId == id &&
							(s.ShipmentStatus == ShipmentStatus.AcceptShipmentRequest || s.ShipmentStatus == ShipmentStatus.DeliveredToCarrier ||
							 s.ShipmentStatus == ShipmentStatus.DeliveredToReceipt));
						break;

					case "history":
						shipments = from s in _shipmentRepository.All()
									where s.UserId == id && !_arhivedShipmentRepository.Any(a => a.UserId == id && s.Id == a.ShipmentId) &&
									(s.ShipmentStatus == ShipmentStatus.Review || s.ShipmentStatus == ShipmentStatus.Cancelled || s.ShipmentStatus == ShipmentStatus.NotDelivered)
									select s;
						break;
				}
			}

			var pagedShipment = shipments.OrderByDescending(s => s.PostDate).Skip((request.Page - 1) * 20).Take(20).ToList().Select(s => new ShipmentViewModel
			{
				budget = s.Budget,
				category_id = s.CategoryId,
				category = s.Category != null ? s.Category.GetName(request.language) : null,
				currency = s.Currency,
				delevery_date = s.DeleveryDate,
				description = s.Description,
				from = s.From.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				image1 = s.Image1 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 1) : null,
				image2 = s.Image2 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 2) : null,
				image3 = s.Image3 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 3) : null,
				post_date = s.PostDate,
				recipiant_mobile = s.RecipiantMobile,
				recipiant_name = s.RecipiantName,
				recipiant_secoundary_mobile = s.RecipiantSecoundary_mobile,
				recipiant_secoundary_name = s.RecipiantSecoundary_name,
				sender = new SmallUserViewModel
				{
					id = s.User.Id,
					name = s.User.FirstName,
					image = s.User.Avatar != null ? string.Format("Image/Avatar?id={0}", s.User.Id) : null,
				},
				carrier = s.CarrierId == null ? null : new SmallUserViewModel
				{
					id = s.CarrierId.Value,
					name = s.Carrier.FirstName,
					image = s.Carrier.Avatar != null ? string.Format("Image/Avatar?id={0}", s.Carrier.Id) : null,
					mobile = s.Carrier.Phone
				},
				shipment_id = s.Id,
				shipment_status = (int)s.ShipmentStatus,
				size = s.Size,
				title = s.Title,
				to = s.To.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				weight = s.Weight
			});

			return Ok(new
			{
				status = true,
				pages_count = Math.Ceiling((float)shipments.Count() / 20),
				page = request.Page,
				items = pagedShipment
			});
		}

		[Route("deactivateTravels"), HttpPost]
		public async Task<IHttpActionResult> DeactivateTravels(DeactivateTravelsRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var travels = await _carryTravelRepository.GetAllAsync(t => t.UserId == id && t.Active != request.Active);

			foreach (var travel in travels)
			{
				travel.Active = request.Active;
				_carryTravelRepository.Update(travel);
			}

			await _unitOfWork.SaveChangesAsync();

			return ApiResult(true);
		}

		[Route("waitingList"), HttpPost]
		public IHttpActionResult WaitingList(GetShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipments = _shipmentRepository.GetQuery(s => s.ShipmentStatus == ShipmentStatus.PostShipmentRequest && s.UserId != id);

			var pagedShipment = shipments.OrderByDescending(s => s.PostDate).Skip((request.Page - 1) * 20).Take(20).ToList();

			var result = pagedShipment.Select(s => new ShipmentViewModel
			{
				budget = s.Budget,
				category_id = s.CategoryId,
				category = s.Category != null ? s.Category.GetName(request.language) : null,
				currency = s.Currency,
				delevery_date = s.DeleveryDate,
				description = s.Description,
				from = s.From.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				image1 = s.Image1 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 1) : null,
				image2 = s.Image2 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 2) : null,
				image3 = s.Image3 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 3) : null,
				post_date = s.PostDate,
				sender = new SmallUserViewModel
				{
					id = s.User.Id,
					name = s.User.FirstName,
					image = s.User.Avatar != null ? string.Format("Image/Avatar?id={0}", s.User.Id) : null,
				},
				carrier = s.CarrierId == null ? null : new SmallUserViewModel
				{
					id = s.CarrierId.Value,
					name = s.Carrier.FirstName,
					image = s.Carrier.Avatar != null ? string.Format("Image/Avatar?id={0}", s.Carrier.Id) : null,
				},
				shipment_id = s.Id,
				shipment_status = (int)s.ShipmentStatus,
				size = s.Size,
				title = s.Title,
				to = s.To.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				weight = s.Weight
			}).ToList();

			foreach (var shipment in result)
			{
				var fromCountry = shipment.from.FirstOrDefault();
				var toCountry = shipment.to.FirstOrDefault();

				if (fromCountry != null && toCountry != null)
				{
					shipment.isInterestActive = _carryTravelRepository.Any(t => t.TravelingDate >= DateTime.Now && t.Active && t.UserId == id &&
						t.From.Any(f => f.Country == fromCountry.Country) && t.To.Any(p => p.Country == toCountry.Country));
				}
				else
				{
					shipment.isInterestActive = false;
				}
			}

			return Ok(new
			{
				status = true,
				pages_count = Math.Ceiling((float)shipments.Count() / 20),
				page = request.Page,
				items = result
			});
		}

		[Route("getProfile"), HttpPost]
		public async Task<IHttpActionResult> GetProfile(GetShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var user = _userRepository.GetById(id);

			var shipments = _shipmentRepository.GetQuery(s => s.CarrierId == user.Id &&
				(s.ShipmentStatus == ShipmentStatus.PostShipmentRequest || s.ShipmentStatus == ShipmentStatus.Review)).ToList();

			var history = shipments.Select(s => new ShipmentViewModel
			{
				budget = s.Budget,
				category_id = s.CategoryId,
				category = s.Category != null ? s.Category.GetName(request == null ? "en" : request.language) : null,
				currency = s.Currency,
				delevery_date = s.DeleveryDate,
				description = s.Description,
				rate = s.CarrierReview.FirstOrDefault() != null ? s.CarrierReview.FirstOrDefault().Rate : 0,
				comment = s.CarrierReview.FirstOrDefault() != null ? s.CarrierReview.FirstOrDefault().Comment : null,
				from = s.From.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				image1 = s.Image1 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 1) : null,
				image2 = s.Image2 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 2) : null,
				image3 = s.Image3 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 3) : null,
				post_date = s.PostDate,
				sender = new SmallUserViewModel
				{
					id = s.User.Id,
					name = s.User.FirstName,
					image = s.User.Avatar != null ? string.Format("Image/Avatar?id={0}", s.User.Id) : null,
				},
				shipment_id = s.Id,
				shipment_status = (int)s.ShipmentStatus,
				size = s.Size,
				title = s.Title,
				to = s.To.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				weight = s.Weight
			});

			var travelActive = await _carryTravelRepository.GetFirstOrDefaultAsync(c => c.UserId == user.Id);

			return ApiResult(true, new
			{
				name = user.FirstName,
				mobile = user.Phone,
				isTravelActive = travelActive == null ? true : travelActive.Active,
				rating = user.CarrierRating,
				image = user.Avatar != null ? string.Format("Image/Avatar?id={0}", user.Id) : null,
				history = history
			});
		}

		[Route("getCarrierProfile"), HttpPost]
		public IHttpActionResult GetCarrierProfile(GetUserRequest request)
		{
			var user = _userRepository.GetById(request.User_id);

			if (user == null) return ApiResult(false, "User not found");

			var shipments = _shipmentRepository.GetQuery(s => s.CarrierId == user.Id &&
				(s.ShipmentStatus == ShipmentStatus.PostShipmentRequest || s.ShipmentStatus == ShipmentStatus.Review)).ToList();

			var history = shipments.Select(s => new ShipmentViewModel
			{
				budget = s.Budget,
				category_id = s.CategoryId,
				category = s.Category != null ? s.Category.GetName(request.language) : null,
				currency = s.Currency,
				delevery_date = s.DeleveryDate,
				description = s.Description,
				rate = s.CarrierReview.FirstOrDefault() != null ? s.CarrierReview.FirstOrDefault().Rate : 0,
				comment = s.CarrierReview.FirstOrDefault() != null ? s.CarrierReview.FirstOrDefault().Comment : null,
				from = s.From.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				image1 = s.Image1 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 1) : null,
				image2 = s.Image2 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 2) : null,
				image3 = s.Image3 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 3) : null,
				post_date = s.PostDate,
				sender = new SmallUserViewModel
				{
					id = s.User.Id,
					name = s.User.FirstName,
					image = s.User.Avatar != null ? string.Format("Image/Avatar?id={0}", s.User.Id) : null,
				},
				shipment_id = s.Id,
				shipment_status = (int)s.ShipmentStatus,
				size = s.Size,
				title = s.Title,
				to = s.To.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				weight = s.Weight
			});

			var travelsDb = _carryTravelRepository.GetQuery(t => t.UserId == request.User_id).ToList();

			var travels = travelsDb.Select(t => new
			{
				TravelId = t.Id,
				from = t.From.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				to = t.To.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
				isTravelActive = t.Active,
				TravelingDate = t.TravelingDate,
				Image = user.Avatar != null ? string.Format("Image/Avatar?id={0}", user.Id) : null,
				CarrierRestrictions = t.DisabledCategories.Select(c => c.Category.GetName(request.language))
			});

			return ApiResult(true, new
			{
				name = user.FirstName,
				rating = user.CarrierRating,
				mobile = user.Phone,
				image = user.Avatar != null ? string.Format("Image/Avatar?id={0}", user.Id) : null,
				history = history,
				travels = travels
			});
		}

		[Route("getNotifications"), HttpPost]
		public IHttpActionResult GetNotifications(PagedRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var noties = _notificationRepository.GetQuery(n => n.UserId == id);

			var notiesPaged = noties.OrderByDescending(n => n.Id).Skip((request.page - 1) * 20).Take(20).Select(n => new
			{
				aps = n.Aps,
				data = n.Data
			});

			return Ok(new
			{
				status = true,
				pages_count = Math.Ceiling((float)noties.Count() / 20),
				page = request.page,
				items = notiesPaged
			});
		}

		[Route("getShipment"), HttpPost]
		public async Task<IHttpActionResult> GetShipment(ShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var result = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.shipment_id && (s.UserId == id || s.CarrierId == id));

			if (result != null)
			{
				var shipment = new ShipmentViewModel
				{
					budget = result.Budget,
					category_id = result.CategoryId,
					category = result.Category != null ? result.Category.GetName(request.language) : null,
					currency = result.Currency,
					delevery_date = result.DeleveryDate,
					description = result.Description,
					from = result.From.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
					image1 = result.Image1 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", result.Id, 1) : null,
					image2 = result.Image2 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", result.Id, 2) : null,
					image3 = result.Image3 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", result.Id, 3) : null,
					post_date = result.PostDate,

					recipiant_mobile = result.RecipiantMobile,
					recipiant_name = result.RecipiantName,
					recipiant_secoundary_mobile = result.RecipiantSecoundary_mobile,
					recipiant_secoundary_name = result.RecipiantSecoundary_name,
					sender = new SmallUserViewModel
					{
						id = result.User.Id,
						name = result.User.FirstName,
						image = result.User.Avatar != null ? string.Format("Image/Avatar?id={0}", result.User.Id) : null,
					},
					carrier = result.CarrierId == null ? null : new SmallUserViewModel
					{
						id = result.CarrierId.Value,
						name = result.Carrier.FirstName,
						image = result.Carrier.Avatar != null ? string.Format("Image/Avatar?id={0}", result.Carrier.Id) : null,
						mobile = result.Carrier.Phone
					},
					shipment_id = result.Id,
					shipment_status = (int)result.ShipmentStatus,
					size = result.Size,
					title = result.Title,
					to = result.To.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, City = l.City, Address = l.Address }),
					weight = result.Weight
				};

				return ApiResult(true, shipment);
			}

			return ApiResult(false);
		}

		[Route("notDelivered"), HttpPost]
		public async Task<IHttpActionResult> NotDelivered(ShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.shipment_id && s.UserId == id);

			if (shipment != null)
			{
				shipment.ShipmentStatus = ShipmentStatus.NotDelivered;
				await _unitOfWork.SaveChangesAsync();

				await _notificationService.SendNotice(id, shipment.CarrierId.Value, shipment, 10);
				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("cancelShipment"), HttpPost]
		public async Task<IHttpActionResult> CancelShipment(ShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.shipment_id && (s.UserId == id || s.CarrierId == id)
				&& (s.ShipmentStatus == ShipmentStatus.HireCarrier || s.ShipmentStatus == ShipmentStatus.DeliveredToCarrier ||
				s.ShipmentStatus == ShipmentStatus.AcceptShipmentRequest));

			if (shipment != null)
			{
				if (shipment.UserId == id && shipment.CarrierId.HasValue)
				{
					await _notificationService.SendNotice(id, shipment.CarrierId.Value, shipment, 8);
				}
				else
				{
					await _notificationService.SendNotice(shipment.UserId, id, shipment, 8);
				}

				if (shipment.ShipmentStatus == ShipmentStatus.HireCarrier)
				{
					shipment.ShipmentStatus = ShipmentStatus.PostShipmentRequest;
				}
				else
				{
					shipment.ShipmentStatus = ShipmentStatus.Cancelled;
				}

				shipment.CarrierId = null;

				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("resetStatus"), HttpGet]
		public async Task<IHttpActionResult> ResetStatus()
		{
			var all = _shipmentRepository.All();

			foreach (var s in all)
			{
				s.ShipmentStatus = ShipmentStatus.PostShipmentRequest;
				s.CarrierId = null;
			}

			await _unitOfWork.SaveChangesAsync();

			return ApiResult(true);
		}

		[Route("hideHistoryShipment"), HttpPost]
		public async Task<IHttpActionResult> HideHistoryShipment(ShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.shipment_id);

			if (shipment != null)
			{
				var arhivedShipment = new ArhivedShipment
				{
					UserId = id,
					ShipmentId = shipment.Id
				};

				_arhivedShipmentRepository.Add(arhivedShipment);
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}
	}
}
