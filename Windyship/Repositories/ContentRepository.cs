using Windyship.Core;
using Windyship.Dal.Core;
using Windyship.Entities;

namespace Windyship.Repositories
{
	public sealed class ContentRepository : RepositoryBase<Content, int>, IContentRepository
	{
		public ContentRepository(IDataContext<Content, int> context)
			: base(context)
		{
		}
	}
}
