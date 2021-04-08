using System.ComponentModel.DataAnnotations;

namespace GoogleReCaptcha.Examples.Mvc.Models
{
	public class HomeModel
	{
		[Required]
		public string Name { get; set; }

		public int? Age { get; set; }
	}
}
