using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Windyship.Core;
using Windyship.Entities;

namespace Windyship.Dal.Core
{
	public sealed class DataContext<TEntity, TKey> : IDataContext<TEntity, TKey>
		where TEntity : class, IEntity<TKey>
		where TKey : struct, IEquatable<TKey>
	{
		private DbContext _context;

		private DbContext Context
		{
			get
			{
				if (_context == null)
					throw new ObjectDisposedException("context");

				return _context;
			}
		}

		public DataContext(DbContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			_context = context;
		}

		public IQueryable<TEntity> DbSet()
		{
			return Context.Set<TEntity>();
		}

		public void Add(TEntity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			Context.Set<TEntity>().Add(entity);
		}

		public void ApplyCurrentValues(TEntity item)
		{
			if (item == null) throw new ArgumentNullException("item");

			var entry = _context.Entry(item);
			if (entry.State == EntityState.Detached)
			{
				var attacheItem = GetById(item.Id);
				if (attacheItem != null)
				{
					var attachedEntry = _context.Entry(attacheItem);
					attachedEntry.CurrentValues.SetValues(item);
				}
				else
				{
					entry.State = EntityState.Modified;
				}
			}
		}

		public TEntity GetById(TKey id)
		{
			return Context.Set<TEntity>().FirstOrDefault(entity => entity.Id.Equals(id));
		}

		public void Remove(TEntity item)
		{
			if (item == null) throw new ArgumentNullException("item");

			var entry = _context.Entry(item);
			if (entry.State == EntityState.Detached)
			{
				item = GetById(item.Id);
			}

			_context.Set<TEntity>().Remove(item);
		}

		public int SaveChanges()
		{
			return Context.SaveChanges();
		}

		public Task<int> SaveChangesAsync()
		{
			return Context.SaveChangesAsync();
		}

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return Context.SaveChangesAsync(cancellationToken);
		}

		public void RemoveRange(Expression<Func<TEntity, bool>> predicate)
		{
			Context.Set<TEntity>().RemoveRange(DbSet().Where(predicate));
		}

		public Task<int> ExecuteStoredProcedure(string procedureName, params object[] parameters)
		{
			return Context.Database.ExecuteSqlCommandAsync( string.Format( "exec {0}", procedureName ), parameters );
		}

		public void Dispose()
		{
			if (_context != null)
			{
				_context.Dispose();
				_context = null;
			}
		}
	}
}