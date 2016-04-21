using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Windyship.Api.Model.Common;
using Windyship.Api.Model.Shipments;
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

		private readonly ILocationFromRepository _locationFromRepository;
		private readonly ILocationToRepository _locationToRepository;

		private readonly ITravelFromRepository _travelFromRepository;
		private readonly ITravelToRepository _travelToRepository;
		private readonly IDisabledCategoriesRepository _disabledCategoriesRepository;
		private readonly ICarrierReviewRepository _carrierReviewRepository;

		private IUnitOfWork _unitOfWork;

		public DealController(IUserRepository userRepository, ICategoryRepository categoryRepository, IShipmentRepository shipmentRepository,
			ILocationFromRepository locationFromRepository, ILocationToRepository locationToRepository, ITravelFromRepository travelFromRepository,
			ITravelToRepository travelToRepository, IDisabledCategoriesRepository disabledCategoriesRepository, ICarrierReviewRepository carrierReviewRepository,
			ICarryTravelRepository carryTravelRepository, IUnitOfWork unitOfWork)
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
						var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, ShipmentId = shipment.Id };
						_locationToRepository.Add(locationTo);
					}
				}

				if (request.From != null)
				{
					foreach (var loc in request.From)
					{
						var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, ShipmentId = shipment.Id };
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
							var locationTo = new LocationTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, ShipmentId = shipment.Id };
							_locationToRepository.Add(locationTo);
						}
					}

					_locationFromRepository.RemoveRange(l => l.ShipmentId == shipment.Id);

					if (request.From != null)
					{
						foreach (var loc in request.From)
						{
							var locationFrom = new LocationFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, ShipmentId = shipment.Id };
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
		public async Task<IHttpActionResult> AddCarryTravel([FromBody] CarryTravelViewModel request)
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
					RepeatDays = string.Join(",", request.repeat_days),
					TravelingDate = request.traveling_date,
				};

				_carryTravelRepository.Add(carryTravel);
				await _unitOfWork.SaveChangesAsync();

				if (request.To != null)
				{
					foreach (var loc in request.To)
					{
						var locationTo = new TravelTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, CarryTraveId = carryTravel.Id };
						_travelToRepository.Add(locationTo);
					}
				}

				if (request.From != null)
				{
					foreach (var loc in request.From)
					{
						var locationFrom = new TravelFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, CarryTraveId = carryTravel.Id };
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
		public async Task<IHttpActionResult> EditCarryTravel([FromBody] CarryTravelViewModel request)
		{
			var id = User.Identity.GetUserId<int>();

			var carryTravel = await _carryTravelRepository.GetFirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == id);

			if (carryTravel != null)
			{
				var days = JsonConvert.SerializeObject(request.repeat_days);

				carryTravel.ArrivalBefore = request.arrival_before;
				carryTravel.MaxSize = request.max_size;
				carryTravel.MaxWeight = request.max_weight;
				carryTravel.RepeatDays = days;
				carryTravel.TravelingDate = request.traveling_date;

				_carryTravelRepository.Update(carryTravel);

				_travelToRepository.RemoveRange(l => l.CarryTraveId == carryTravel.Id);

				if (request.To != null)
				{
					foreach (var loc in request.To)
					{
						var locationTo = new TravelTo { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, CarryTraveId = carryTravel.Id };
						_travelToRepository.Add(locationTo);
					}
				}

				_travelFromRepository.RemoveRange(l => l.CarryTraveId == carryTravel.Id);

				if (request.From != null)
				{
					foreach (var loc in request.From)
					{
						var locationFrom = new TravelFrom { Lat = loc.Lat, Long = loc.Long, Country = loc.Country, Address = loc.Address, CarryTraveId = carryTravel.Id };
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
				s.UserId == id && s.ShipmentStatus == ShipmentStatus.DeliveredToReceipt);

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
				await _unitOfWork.SaveChangesAsync();

				var rate = _carrierReviewRepository.GetQuery(u => u.Shipment.CarrierId == shipment.CarrierId).Average(r => (decimal)r.Rate);
				user.CarrierRating = rate;
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("getCarriersForShipment"), HttpPost]
		public async Task<IHttpActionResult> GetCarriersForShipment(CarriersForShipmentReqiest request)
		{
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id);

			if (shipment != null)
			{
				var travels = await _carryTravelRepository.GetAllAsync(t => t.TravelingDate >= DateTime.Now);

				var carriers = travels.Select(c => new CarrierViewModel
				{
					TravelId = c.Id,
					CarrierId = c.UserId,
					Delivery_date = c.ArrivalBefore,
					From = c.From.FirstOrDefault() != null ?
						new LocationViewModel
						{
							Lat = c.From.FirstOrDefault().Lat,
							Long = c.From.FirstOrDefault().Long,
							Country = c.From.FirstOrDefault().Country,
							Address = c.From.FirstOrDefault().Address
						} : null,
					To = c.To.FirstOrDefault() != null ?
						new LocationViewModel
						{
							Lat = c.To.FirstOrDefault().Lat,
							Long = c.To.FirstOrDefault().Long,
							Country = c.To.FirstOrDefault().Country,
							Address = c.From.FirstOrDefault().Address
						} : null,
					Image = string.Format("image/avatar?id={0}", c.UserId),
					Mobile = c.User.Phone,
					Name = c.User.FirstName,
					Rating = c.User.CarrierRating,
					CarrierRestrictions = c.DisabledCategories.Select(g => g.Category.GetName(request.Language)),
				});

				return ApiResult(true, carriers);
			}
			else return ApiResult(false);
		}

		[Route("hireCarrier"), HttpPost]
		public async Task<IHttpActionResult> HireCarrier(HireCarrierRequest request)
		{
			var id = User.Identity.GetUserId<int>();
			var shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.UserId == id && s.ShipmentStatus == ShipmentStatus.PostShipmentRequest);

			if (shipment != null)
			{
				shipment.CarrierId = request.Carrier_id;
				shipment.ShipmentStatus = ShipmentStatus.HireCarrier;
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
				s.CarrierId == id && s.ShipmentStatus == ShipmentStatus.PostShipmentRequest);

			if (shipment != null)
			{
				shipment.PinCode = TokenGenerator.GetUniqueDigits(6);

#warning Send shipment pin code by sms

				shipment.ShipmentStatus = ShipmentStatus.AcceptShipmentRequest;
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
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			shipment = await _shipmentRepository.GetFirstOrDefaultAsync(s => s.Id == request.Shipment_id &&
				s.CarrierId == id && s.ShipmentStatus == ShipmentStatus.DeliveredToCarrier);

			if (shipment != null && shipment.PinCode == request.Pin_code)
			{
				shipment.ShipmentStatus = ShipmentStatus.DeliveredToReceipt;
				shipment.DeleveryDate = DateTime.Now;
				await _unitOfWork.SaveChangesAsync();

				return ApiResult(true);
			}

			return ApiResult(false);
		}

		[Route("getMyShipments"), HttpPost]
		public IHttpActionResult GetMyShipments(GetMyShipmentsRequest request)
		{
			var id = User.Identity.GetUserId<int>();

			IQueryable<Shipment> shipments = null;

			if (request.View_as == "carrier")
			{
				switch (request.Type)
				{
					case "interested":
						shipments = _shipmentRepository.GetQuery(s => s.CarrierId == id && s.ShipmentStatus == ShipmentStatus.HireCarrier);
						break;

					case "in-progress":
						shipments = _shipmentRepository.GetQuery(s => s.CarrierId == id &&
							(s.ShipmentStatus == ShipmentStatus.AcceptShipmentRequest || s.ShipmentStatus == ShipmentStatus.DeliveredToCarrier ||
							 s.ShipmentStatus == ShipmentStatus.DeliveredToReceipt));
						break;

					case "history":
						shipments = _shipmentRepository.GetQuery(s => s.CarrierId == id && s.ShipmentStatus == ShipmentStatus.Review);
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
						shipments = _shipmentRepository.GetQuery(s => s.UserId == id && s.ShipmentStatus == ShipmentStatus.Review);
						break;
				}
			}

			var pagedShipment = shipments.OrderByDescending(s => s.PostDate).Skip((request.Page - 1) * 20).Take(20).ToList().Select(s => new ShipmentViewModel
			{
				budget = s.Budget,
				category_id = s.CategoryId,
				category = s.Category.GetName(request.language),
				currency = s.Currency,
				delevery_date = s.DeleveryDate,
				description = s.Description,
				from = s.From.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, Address = l.Address }),
				image1 = s.Image1 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 1) : null,
				image2 = s.Image2 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 2) : null,
				image3 = s.Image3 != null ? string.Format("Image/ShipmentImage?shipmentId={0}&imageId={1}", s.Id, 3) : null,
				post_date = s.PostDate,
				receiver = s.Carrier != null ? new SmallUserViewModel
				{
					id = s.Carrier.Id,
					name = s.Carrier.FirstName,
					image = s.Carrier.Avatar != null ? string.Format("Image/Avatar?id={0}", s.Carrier.Id) : null,
				} : null,
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
				shipment_id = s.Id,
				shipment_status = (int)s.ShipmentStatus,
				size = s.Size,
				title = s.Title,
				to = s.To.Select(l => new LocationViewModel { Lat = l.Lat, Long = l.Long, Country = l.Country, Address = l.Address }),
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
	}
}
