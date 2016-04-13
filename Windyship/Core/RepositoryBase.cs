using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Windyship.Core;
using Windyship.Entities;

namespace Windyship.Dal.Core
{
	public abstract class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
		where TEntity : IEntity<TKey>
		where TKey : struct, IEquatable<TKey>
	{
		private readonly IDataContext<TEntity, TKey> _context;

		protected IDataContext<TEntity, TKey> Context
		{
			get { return _context; }
		}

		protected RepositoryBase(IDataContext<TEntity, TKey> context)
		{
			if (context == null) throw new ArgumentNullException( "context" );
			_context = context;
		}

		public virtual ICollection<TEntity> All()
		{
			return Context.DbSet().ToList();
		}

		public virtual TEntity GetById(TKey id)
		{
			return Context.GetById( id );
		}

		public virtual void Remove(TEntity item)
		{
			Context.Remove( item );
		}

		public virtual TEntity Add(TEntity entity)
		{
			Context.Add( entity );

			return entity;
		}

		public virtual TEntity Update(TEntity entity)
		{
			_context.ApplyCurrentValues( entity );

			return entity;
		}

		public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> searchCriteria)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );

			return Context
				.DbSet()
				.Where( searchCriteria );
		}

		private IOrderedQueryable<TEntity> GetQuery<TOrder>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TOrder>> orderCriteria, OrderDirection orderDirection)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );
			if (orderCriteria == null) throw new ArgumentNullException( "orderCriteria" );

			return orderDirection == OrderDirection.Asc
				? GetQuery( searchCriteria ).OrderBy( orderCriteria )
				: GetQuery( searchCriteria ).OrderByDescending( orderCriteria );
		}

		private IQueryable<TResult> GetAllCore<TOrder, TResult>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TOrder>> orderCriteria, OrderDirection orderDirection, Expression<Func<TEntity, TResult>> selector)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );
			if (orderCriteria == null) throw new ArgumentNullException( "orderCriteria" );
			if (selector == null) throw new ArgumentNullException( "selector" );

			return GetQuery( searchCriteria, orderCriteria, orderDirection ).Select( selector );
		}

		/// <summary>
		/// Получить коллекцию всех записей таблицы соответствующих критерию.
		/// Асинхронная версия.
		/// </summary>
		/// <param name="searchCriteria">Критерий отбора. Обязательный параметр.</param>
		/// <returns>Коллекция с результатами запроса.</returns>
		public async Task<ICollection<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> searchCriteria)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );

			return await Context.DbSet()
				.Where( searchCriteria )
				.ToListAsync();
		}

		/// <summary>
		/// Получить отсортированную коллекцию всех записей таблицы.
		/// Асинхронная версия.
		/// </summary>
		/// <returns>Коллекция с результатами запроса.</returns>
		public async Task<ICollection<TResult>> GetAllAsync<TOrder, TResult>(Expression<Func<TResult, TOrder>> orderCriteria, OrderDirection orderDirection, Expression<Func<TEntity, TResult>> selector)
		{
			var query = Context
				.DbSet()
				.Select( selector );

			query = orderDirection == OrderDirection.Asc
				? query.OrderBy( orderCriteria )
				: query.OrderByDescending( orderCriteria );

			return await query.ToListAsync();
		}

		public ICollection<TResult> GetAll<TOrder, TResult>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TOrder>> orderCriteria, OrderDirection orderDirection, Expression<Func<TEntity, TResult>> selector)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );
			if (orderCriteria == null) throw new ArgumentNullException( "orderCriteria" );
			if (selector == null) throw new ArgumentNullException( "selector" );

			return GetAllCore( searchCriteria, orderCriteria, orderDirection, selector ).ToList();
		}

		public virtual async Task<ICollection<TResult>> GetAllAsync<TOrder, TResult>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TOrder>> orderCriteria, OrderDirection orderDirection, Expression<Func<TEntity, TResult>> selector)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );
			if (orderCriteria == null) throw new ArgumentNullException( "orderCriteria" );
			if (selector == null) throw new ArgumentNullException( "selector" );

			return await GetAllCore( searchCriteria, orderCriteria, orderDirection, selector ).ToListAsync();
		}

		public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> searchCriteria)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );
			return await GetQuery( searchCriteria ).FirstOrDefaultAsync();
		}

		public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TResult>> selector)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );
			if (selector == null) throw new ArgumentNullException( "selector" );

			return await GetQuery( searchCriteria ).Select( selector ).FirstOrDefaultAsync();
		}

		public async Task<TResult> GetFirstOrDefaultAsync<TResult, TProperty>(Expression<Func<TEntity, bool>> searchCriteria, Expression<Func<TEntity, TResult>> selector, Expression<Func<TResult, TProperty>> path)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );
			if (selector == null) throw new ArgumentNullException( "selector" );
			if (path == null) throw new ArgumentNullException( "selector" );

			return await GetQuery( searchCriteria ).Select( selector ).Include( path ).FirstOrDefaultAsync();
		}

		public void RemoveRange(Expression<Func<TEntity, bool>> predicate)
		{
			if (predicate == null) throw new ArgumentNullException( "predicate" );

			Context.RemoveRange( predicate );
		}

		public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> searchCriteria)
		{
			if (searchCriteria == null) throw new ArgumentNullException( "searchCriteria" );

			return await GetQuery( searchCriteria ).AnyAsync();
		}
	}
}