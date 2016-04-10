using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public sealed class FaqRepository : RepositoryBase<Faq, int>, IFaqRepository
	{
		public FaqRepository(IDataContext<Faq, int> context)
			: base(context)
		{
		}
	}
}
