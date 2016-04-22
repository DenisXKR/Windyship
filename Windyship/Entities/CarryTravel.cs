using System;
using System.Collections.Generic;

namespace Windyship.Entities
{
	public class CarryTravel : IEntity<int>
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public bool Active { get; set; }

		public DateTime TravelingDate { get; set; }

		public DateTime ArrivalBefore { get; set; }

		public string RepeatDays { get; set; } //: [array of days sat, sun, mon] // Optional

		public string MaxSize { get; set; } // s, m, l

		public decimal MaxWeight { get; set; } // number (1...10)

		public virtual User User { get; set; }

		public virtual ICollection<TravelFrom> From { get; set; }  // location

		public virtual ICollection<TravelTo> To { get; set; }

		public virtual ICollection<DisabledCategories> DisabledCategories { get; set; }
	}
}
