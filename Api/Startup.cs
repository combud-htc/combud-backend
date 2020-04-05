using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Caching.Redis;
using StackExchange.Redis;
using System.Net;
using NetTopologySuite.Geometries;
using Microsoft.OpenApi.Models;

namespace Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.AddDistributedRedisCache(options => options.Configuration = Environment.GetEnvironmentVariable("REDIS_URL"));

			services.AddDbContext<Db>(options => options.UseLazyLoadingProxies().UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONNSTR"), x =>
			{
				x.UseNetTopologySuite();
			}));

			services.AddSession(options =>
			{
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
				options.Cookie.SameSite = SameSiteMode.Strict;
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			services.AddSingleton(new ComBudConfiguration()
			{
				GMAP_API_KEY = Environment.GetEnvironmentVariable("GMAP_API_KEY"),
				AWS_ID = Environment.GetEnvironmentVariable("AWS_ID"),
				AWS_KEY = Environment.GetEnvironmentVariable("AWS_KEY"),
				SENDER_ADDRESS = Environment.GetEnvironmentVariable("SENDER_ADDRESS"),
				BASE_URL = Environment.GetEnvironmentVariable("BASE_URL")
			});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
			});

			services.AddControllers();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

			app.UseHttpsRedirection();

			app.UseDefaultFiles();
			app.UseSwagger();
			
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseStaticFiles();

			app.UseSession();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}