using CDP.Common.Caching;
using CDP.Common.Logging;
using CDP.ContestHost.FileStoreRepository;
using CDP.ContestHost.Site.Config;
using CDP.ContestHost.Site.Hubs;
using CDP.CoreApp.Features.Contests.Commands;
using CDP.CoreApp.Features.Contests.Queries;
using CDP.CoreApp.Features.FlightMatrices.Commands;
using CDP.CoreApp.Features.FlightMatrices.Queries;
using CDP.CoreApp.Features.Pilots.Commands;
using CDP.CoreApp.Features.Pilots.Queries;
using CDP.CoreApp.Features.Tasks.Queries;
using CDP.CoreApp.Interfaces.Contests;
using CDP.CoreApp.Interfaces.FlightMatrices;
using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
using CDP.CoreApp.Interfaces.Pilots;
using CDP.CoreApp.Interfaces.Tasks;
using CDP.ScoringAndSortingImpl.F3K.Sorting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

namespace CDP.ContestHost.Site
{
    /// <summary>
    /// Starts up the host
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The hosting environment
        /// </summary>
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            Configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(config =>
            {
                config.JsonSerializerSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Objects
                };
            });

            services.AddCors(
                options => options.AddPolicy("AllowCors",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    })
                );
            services.AddMvc();

            ConfigureDI(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("AllowCors");

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".cdp"] = "application/json";

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                ContentTypeProvider = provider
            });

            app.UseSignalR(router => router.MapHub<ContestScoringHub>("scoring"));

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDeveloperExceptionPage();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");

                //    routes.MapSpaFallbackRoute(
                //        name: "spa-fallback",
                //        defaults: new { controller = "Home", action = "Index" });
            });
        }

        /// <summary>
        /// Configures the dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureDI(IServiceCollection services)
        {
            services.AddSingleton<CDP.Common.Logging.ILoggingService, DebugLogger>();
            services.AddSingleton<ICache, InMemoryCache>();

            services.AddScoped<IContestRepository, ContestRespository>(provider =>
                new ContestRespository(Path.Combine(hostingEnvironment.WebRootPath, StorageFileConfig.ContestsFileName), provider.GetService<ICache>(), provider.GetService<ILoggingService>()));
            services.AddScoped<IPilotRepository, PilotFileSystemRepository>(provider =>
                new PilotFileSystemRepository(Path.Combine(hostingEnvironment.WebRootPath, StorageFileConfig.PilotsFileName), provider.GetService<ICache>(), provider.GetService<ILoggingService>()));
            services.AddScoped<IFlightMatrixRepository, FlightMatrixRepository>(provider =>
                new FlightMatrixRepository(Path.Combine(hostingEnvironment.WebRootPath, StorageFileConfig.FlightMatrixFileName), provider.GetService<ICache>(), provider.GetService<ILoggingService>()));
            services.AddScoped<ITaskRepository, TaskRepository>(provider =>
                new TaskRepository());
            services.AddSingleton<ISortingAlgo, RandomSortNoTeamProtection>(provider => new RandomSortNoTeamProtection());

            services.AddScoped(provider => new ContestStorageCmdInteractor(provider.GetService<IContestRepository>(), provider.GetService<ILoggingService>()));
            services.AddScoped(provider => new ContestQueryInteractor(provider.GetService<IContestRepository>(), provider.GetService<ILoggingService>()));
            services.AddScoped(provider => new PilotStorageCmdInteractor(provider.GetService<IPilotRepository>(), provider.GetService<ILoggingService>()));
            services.AddScoped(provider => new PilotQueryInteractor(provider.GetService<IPilotRepository>(), provider.GetService<ILoggingService>()));
            services.AddScoped(provider => new FlightMatrixStorageCmdInteractor(provider.GetService<IFlightMatrixRepository>(), provider.GetService<ILoggingService>()));
            services.AddScoped(provider => new FlightMatrixQueryInteractor(provider.GetService<IFlightMatrixRepository>(), provider.GetService<ILoggingService>()));
            services.AddScoped(provider => new TaskQueryInteractor(provider.GetService<ITaskRepository>(), provider.GetService<ILoggingService>()));
            services.AddScoped(provider => new FlightMatrixGenInteractor(provider.GetService<ISortingAlgo>(), provider.GetService<ILoggingService>()));
        }
    }
}
