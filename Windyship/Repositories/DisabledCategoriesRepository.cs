using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public class DisabledCategoriesRepository : RepositoryBase<DisabledCategories, int>, IDisabledCategoriesRepository
	{
		public DisabledCategoriesRepository(IDataContext<DisabledCategories, int> context)
			: base(context)
		{
		}
	}
}
