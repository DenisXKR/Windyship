using System.Collections.Generic;

namespace Windyship.Core
{
	public interface IPagedCollection<out T>
	{
		int TotalCount { get; }

		int TotalPages { get; }

		int PageSize { get; }

		IEnumerable<T> Items { get; }
	}
}