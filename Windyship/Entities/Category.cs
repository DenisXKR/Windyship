using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Windyship.Entities
{
	public class Category : IEntity<int>
	{
		public int Id { get; set; }

		public string NameEn { get; set; }

		public string NameAr { get; set; }

		public string GetName(string lang)
		{
			switch (lang)
			{
				case "en": return NameEn;
				case "ar": return NameAr;
			}

			return NameEn;
		}
	}
}
