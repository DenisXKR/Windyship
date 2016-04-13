using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class CarrierReviewRepository : RepositoryBase<CarrierReview, int>, ICarrierReviewRepository
	{
		public CarrierReviewRepository(IDataContext<CarrierReview, int> context)
			: base(context)
		{
		}
	}
}
