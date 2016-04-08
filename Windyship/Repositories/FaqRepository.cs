﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
