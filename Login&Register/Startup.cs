using BusinessLayer.Interface;
using BusinessLayer.Service;
using LoginRegisterAPI.RabbitMQService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.EmailService;
using RepositoryLayer.Interface;
using RepositoryLayer.Repository;
using RepositoryLayer.Service;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace LoginRegisterAPI
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
            //register swagger           
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Fundoo Notes",
                    Description = "Implementation of Fundoo Notes",
                    TermsOfService = new Uri("https://www.bridgelabz.com/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mohammad Abid Shaikh",
                        Email = "shaikhabid332@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/theabidshaikh/"),
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = Assembly.GetExecutingAssembly().GetName().Name+".xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);               
                
            });
                       
            services.AddDbContext<scaffoldingDbContext>(optionsAction => optionsAction.UseSqlServer(Configuration["ConnectionStrings:SqlConnection"]));

            //log
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            });

           
            //jwt authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["jwt:Issuer"],
                        ValidAudience = Configuration["jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:Key"]))
                    };
                });

            //redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                
            });

            services.AddScoped<IRegisterationBL, RegistrationBL>();
            services.AddScoped<IRegisterationRL, RegistrationRL>();
            services.AddScoped<INotesBL, NotesBL>();
            services.AddScoped<INotesRL, NotesRL>();


            services.AddScoped<EmailService>();
            services.AddScoped<MessagePublish>();
                                                            

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
           
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
