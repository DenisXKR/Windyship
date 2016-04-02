using System;
using System.Collections.Generic;
using Windyship.Core;

namespace Windyship.Dal.Core
{
	public sealed class PagedCollection<T> : IPagedCollection<T>
	{
		public int TotalPages { get; private set; }
		public int PageSize { get; private set; }
		public IEnumerable<T> Items { get; private set; }
		public int TotalCount { get; private set; }

		public PagedCollection(IEnumerable<T> items, int totalCount, int pageSize)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			PageSize = pageSize;
			TotalPages = totalCount / pageSize;

			if (TotalPages * pageSize < totalCount)
				TotalPages ++;
			
			Items = items;
			TotalCount = totalCount;
		}

		/// <summary>
		/// For API Help Page only!
		/// </summary>
		public PagedCollection()
		{
			Items = new List<T>();
			PageSize = 10;
			TotalPages = 1;
			TotalCount = 2;
		}
	}
}