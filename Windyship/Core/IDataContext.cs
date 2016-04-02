using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Windyship.Entities;

namespace Windyship.Core
{
	public interface IDataContext<TEntity, in TKey> : IDisposable
		where TEntity : IEntity<TKey>
		where TKey : struct, IEquatable<TKey>
	{
		IQueryable<TEntity> DbSet();

		void Add(TEntity entity);

		void ApplyCurrentValues(TEntity item);

		TEntity GetById(TKey id);

		void Remove(TEntity item);

		/// <summary>
		/// Saves all changes made in this context to the underlying database.
		/// 
		/// </summary>
		/// 
		/// <returns>
		/// The number of objects written to the underlying database.
		/// </returns>
		/// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">An error occurred sending updates to the database.</exception><exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">A database command did not affect the expected number of rows. This usually indicates an optimistic
		///             concurrency violation; that is, a row has been changed in the database since it was queried.
		///             </exception><exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">The save was aborted because validation of entity property values failed.
		///             </exception><exception cref="T:System.NotSupportedException">An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
		///             on the same context instance.</exception><exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception><exception cref="T:System.InvalidOperationException">Some error occurred attempting to process entities in the context either before or after sending commands
		///             to the database.
		///             </exception>
		int SaveChanges();

		/// <summary>
		/// Asynchronously saves all changes made in this context to the underlying database.
		/// 
		/// </summary>
		/// 
		/// <remarks>
		/// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///             that any asynchronous operations have completed before calling another method on this context.
		/// 
		/// </remarks>
		/// 
		/// <returns>
		/// A task that represents the asynchronous save operation.
		///             The task result contains the number of objects written to the underlying database.
		/// 
		/// </returns>
		/// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">An error occurred sending updates to the database.</exception><exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">A database command did not affect the expected number of rows. This usually indicates an optimistic
		///             concurrency violation; that is, a row has been changed in the database since it was queried.
		///             </exception><exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">The save was aborted because validation of entity property values failed.
		///             </exception><exception cref="T:System.NotSupportedException">An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
		///             on the same context instance.</exception><exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception><exception cref="T:System.InvalidOperationException">Some error occurred attempting to process entities in the context either before or after sending commands
		///             to the database.
		///             </exception>
		Task<int> SaveChangesAsync();

		/// <summary>
		/// Asynchronously saves all changes made in this context to the underlying database.
		/// 
		/// </summary>
		/// 
		/// <remarks>
		/// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///             that any asynchronous operations have completed before calling another method on this context.
		/// 
		/// </remarks>
		/// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> to observe while waiting for the task to complete.
		///             </param>
		/// <returns>
		/// A task that represents the asynchronous save operation.
		///             The task result contains the number of objects written to the underlying database.
		/// 
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
		Task<int> SaveChangesAsync(CancellationToken cancellationToken);

		void RemoveRange(Expression<Func<TEntity, bool>> predicate);

		Task<int> ExecuteStoredProcedure(string procedureName, params object[] parameters);
	}
}