using System.Collections.Generic;
using System.Threading.Tasks;
using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Windyship.Repositories
{
	public class CarryTravelRepository : RepositoryBase<CarryTravel, int>, ICarryTravelRepository
	{
		public CarryTravelRepository(IDataContext<CarryTravel, int> context)
			: base(context)
		{
		}

		public IQueryable<User> GetCarriers()
		{
			return this.Context.DbSet().Select(c => c.User).Distinct();
		}
	}
}
