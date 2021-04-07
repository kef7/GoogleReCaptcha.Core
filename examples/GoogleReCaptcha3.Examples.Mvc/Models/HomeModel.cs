using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleReCaptcha3.Examples.Mvc.Models
{
	public class HomeModel
	{
		[Required]
		public string Name { get; set; }

		public int? Age { get; set; }
	}
}
