using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand;
using Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery;
using Wool.DevChallenge.Api.Application.Queries.GetSortedProductsQuery.SortingStrategy;
using Wool.DevChallenge.Api.Config;

namespace Wool.DevChallenge.Api
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
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            services.AddScoped<ITrolleyCalculatorFactory, TrolleyCalculatorFactory>();

            services.AddScoped<RemoteTrolleyCalculationService>()
                .AddScoped<ITrolleyCalculationService, RemoteTrolleyCalculationService>(s=>s.GetService<RemoteTrolleyCalculationService>());

            services.AddScoped<LocalTrolleyCalculationService>()
                .AddScoped<ITrolleyCalculationService, LocalTrolleyCalculationService>(s=>s.GetService<LocalTrolleyCalculationService>());

            services.AddScoped<ISortingStrategyFactory, SortingStrategyFactory>();

            services.AddScoped<PriceLowSortStrategy>()
                .AddScoped<ISortStrategy, PriceLowSortStrategy>(s => s.GetService<PriceLowSortStrategy>());
            services.AddScoped<PriceHighSortStrategy>()
                .AddScoped<ISortStrategy, PriceHighSortStrategy>(s => s.GetService<PriceHighSortStrategy>());
            services.AddScoped<NameAscendingSortStrategy>()
                .AddScoped<ISortStrategy, NameAscendingSortStrategy>(s => s.GetService<NameAscendingSortStrategy>());
            services.AddScoped<NameDescendingSortStrategy>()
                .AddScoped<ISortStrategy, NameDescendingSortStrategy>(s => s.GetService<NameDescendingSortStrategy>());
            services.AddScoped<RecommendedSortStrategy>()
                .AddScoped<ISortStrategy, RecommendedSortStrategy>(s => s.GetService<RecommendedSortStrategy>());
            services.AddScoped<NoSortStrategy>()
                .AddScoped<ISortStrategy, NoSortStrategy>(s => s.GetService<NoSortStrategy>());


            services.AddHttpClient<IRemoteProductsService, RemoteProductsService>();
            services.AddHttpClient<IRemoteShopperHistoryService, RemoteShopperHistoryService>();


            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddControllers().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            })
            .AddFluentValidation(fv =>
            {
                fv.ImplicitlyValidateChildProperties = true;
                fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wool.DevChallenge.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wool.DevChallenge.Api v1");
            });


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
