﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using Adapter.Notification.Email;
using Adapter.Persistence.MySql;
using Domain.UseCases;
using Host.WebService.Client1.BookOrders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Host.WebService.Client1
{
    /// <summary>
    /// Building the IHost using this approach allows for a lot more testing flexibility than using the standard
    /// Startup.cs file that is demonstrated all over the web 
    /// </summary>
    public class ApplicationHostBuilder
    {
        private readonly Container _container;
        private readonly string[] _args;
        private readonly string _applicationName;

        public ApplicationHostBuilder(string[] args, string applicationName, Container container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            _container = container;
            _args = args;
            _applicationName = applicationName;
            
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }
        
        public virtual IHost Build()
        {
            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(_args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(services => ConfigureServices(services))
                        .Configure(app => Configure(app));
                })
                .UseConsoleLifetime();
            
            RegisterPersistenceAdapter();
            RegisterNotificationAdapter();
            RegisterDomain();

            PreHostBuildActions(hostBuilder, _container);

            var host = hostBuilder.Build()
                .UseSimpleInjector(_container);

            return host;
        }

        /// <summary>
        /// Seam provided for testing to allow container registrations to be overridden and the HostBuilder
        /// to be modified
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="container"></param>
        protected virtual void PreHostBuildActions(IHostBuilder hostBuilder, Container container) { }
        
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddRouting(options => { options.LowercaseUrls = true; });

            services.AddSimpleInjector(_container,
                options =>
                {
                    options.AddAspNetCore()
                        .AddControllerActivation();
                });
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = _applicationName,
                    Version = "v1"
                });
            });
        }

        private void Configure(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var env = serviceProvider.GetService<IWebHostEnvironment>();
            IHostApplicationLifetime hostApplicationLifetime = serviceProvider.GetService<IHostApplicationLifetime>();
            hostApplicationLifetime.ApplicationStopping.Register(OnShutdown);
            
            if (env.IsDevelopment())
            {
                // add development specific app.Use here
            }

            app.UseSimpleInjector(_container);
            
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocExpansion(DocExpansion.None);
                options.SwaggerEndpoint("/swagger/v1/swagger.json", _applicationName);
            });
            
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private void RegisterDomain()
        {
            _container.Register<AddBookTitleRequestUseCase>();
            _container.Register<ApproveBookOrderUseCase>();
            _container.Register<SendBookOrderUseCase>();
            _container.Register<GetAllBookOrdersUseCase>();
            _container.Register<DeleteBookOrdersUseCase>();
            _container.Register<SupplierBookOrderUpdateUseCase>();
        }
        
        protected virtual void RegisterPersistenceAdapter()
        {
            RegisterMySqlPersistenceAdapter();
        }

        private void RegisterMySqlPersistenceAdapter()
        {
            var persistenceAdapter = new Adapter.Persistence.MySql.PersistenceAdapter(
                new PersistenceAdapterSettings()
                {
                    ConnectionString = "server=127.0.0.1;" +
                                       "uid=bookorder_srv;" +
                                       "pwd=123;" +
                                       "database=bookorders"
                });

            persistenceAdapter.Initialize();
            persistenceAdapter.Register(_container);
        }
        
        protected virtual void RegisterNotificationAdapter()
        {
            var notificationAdapter = new Adapter.Notification.Email.NotificationAdapter(
                new NotificationAdapterSettings(
                    "localhost", 1025, bookSupplierEmail: "BookSupplierGateway@fakedomain.com"));
            notificationAdapter.Initialize();
            notificationAdapter.Register(_container);
        }
        
        private void OnShutdown()
        {
        }
    }
}