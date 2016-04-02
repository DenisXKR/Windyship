using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Windyship.Entities;

namespace Windyship.Core
{
	public interface IRepositoryBase<TEntity, TKey>
		where TEntity : IEntity<TKey>
		where TKey : struct, IEquatable<TKey>
	{
		ICollection<TEntity> All();

		TEntity GetById(TKey id);

		void Remove(TEntity item);

		TEntity Add(TEntity entity);

		TEntity Update(TEntity entity);

		Task<ICollection<TResult>> GetAllAsync<TOrder, TResult>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TOrder>> orderCriteria, OrderDirection orderDirection, Expression<Func<TEntity, TResult>> selector);

		Task<ICollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> searchCriteria);

		Task<ICollection<TResult>> GetAllAsync<TOrder, TResult>(Expression<Func<TResult, TOrder>> orderCriteria, OrderDirection orderDirection, Expression<Func<TEntity, TResult>> selector);

		ICollection<TResult> GetAll<TOrder, TResult>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TOrder>> orderCriteria, OrderDirection orderDirection, Expression<Func<TEntity, TResult>> selector);

		/// <summary>
		/// Asynchronously determines whether a sequence contains any elements.
		/// </summary>
		Task<bool> AnyAsync(Expression<Func<TEntity, bool>> searchCriteria);

		Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> searchCriteria);
		Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TResult>> selector);
		Task<TResult> GetFirstOrDefaultAsync<TResult, TProperty>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TResult>> selector, Expression<Func<TResult, TProperty>> path);
		void RemoveRange(Expression<Func<TEntity, bool>> predicate);

		IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> searchCriteria);
	}
}