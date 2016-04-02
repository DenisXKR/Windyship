using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windyship.Core
{
	public interface IUnitOfWork : IDisposable
	{
		int SaveChanges();

		Task<int> SaveChangesAsync();
	}
}
