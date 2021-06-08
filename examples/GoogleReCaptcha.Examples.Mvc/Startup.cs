using GoogleReCaptcha.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GoogleReCaptcha.Examples.Mvc
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public IWebHostEnvironment WebHostEnvironment { get; }

		public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
		{
			Configuration = configuration;
			WebHostEnvironment = webHostEnvironment;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRouting((options) =>
			{
				options.LowercaseUrls = true;
			});

			services.AddControllersWithViews();

			services.AddGoogleReCaptchaV3(Configuration);
			services.AddGoogleReCaptchaV2(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			var isDev = WebHostEnvironment.IsDevelopment();
			if (isDev)
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseCors();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
