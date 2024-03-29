﻿using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Domain.Interfaces;
using Chat.FileServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Chat.Domain.Entities;
using Chat.DbServer;

namespace Chat.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var projectDirectory = Directory.GetCurrentDirectory();
            var chatFileServerServiceConfiguration = 
                Configuration.GetSection(ChatFileServerServiceConfiguration.SECTION_NAME).Get<ChatFileServerServiceConfiguration>();
            
            services.AddScoped<IChatServerService<byte[], byte[]>, ChatFileServerService>(_ 
                => new ChatFileServerService(chatFileServerServiceConfiguration, projectDirectory));

            services.AddScoped<IChatServerService<Message, IEnumerable<Message>>, ChatDbServerService>(_
                => new ChatDbServerService(Configuration.GetConnectionString(ChatDbServerService.CONNECTION_STRING_NAME)));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
