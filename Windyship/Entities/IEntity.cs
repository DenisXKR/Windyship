using System;

namespace Windyship.Entities
{
	public interface IEntity<TId>
		where TId : struct, IEquatable<TId>
	{
		TId Id { get; set; }
	}
}
