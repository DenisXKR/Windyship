using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class LocationFromRepository : RepositoryBase<LocationFrom, int>, ILocationFromRepository
	{
		public LocationFromRepository(IDataContext<LocationFrom, int> context)
			: base(context)
		{
		}
	}
}
