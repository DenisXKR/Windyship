﻿using System.Linq;
using Windyship.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public interface ICarryTravelRepository : IRepositoryBase<CarryTravel, int>
	{
		IQueryable<User> GetCarriers();
	}
}
