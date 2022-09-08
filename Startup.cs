using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Domarservice.DAL;
using Domarservice.BLL;
using Domarservice.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace Domarservice.API
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
      // Proxy headers
      // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-6.0#configure-nginx
      services.Configure<ForwardedHeadersOptions>(options =>
      {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
      });
      services.AddControllers().AddJsonOptions(x =>
          x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "domarservice", Version = "v1" });
      });
      services.AddDbContext<DomarserviceContext>(options =>
          options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
      services.AddScoped<IRefereeRepository, RefereeRepository>();
      services.AddScoped<IRefereeScheduleRepository, RefereeScheduleRepository>();
      services.AddScoped<IBookingRequestRepository, BookingRequestRepository>();
      services.AddScoped<ICompanyRepository, CompanyRepository>();
      services.AddScoped<ICompanyEventRepository, CompanyEventRepository>();
      services.AddScoped<ISendMailHelper, SendMailHelper>();
      // BLL Administration
      services.AddScoped<IAdministrationService, AdministrationService>();
      var mapperConfig = new MapperConfiguration(mc =>
      {
        mc.AddProfile(new AutoMapperProfile());
      });
      // services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

      // Disable default validation responses, it is handled manually with ApiResponse in the data field.
      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.SuppressModelStateInvalidFilter = true;
      });

      IMapper mapper = mapperConfig.CreateMapper();
      services.AddSingleton(mapper);

      // We extend the IdentityUser with ApplicationUser to add more properties like refreshtoken.
      services.AddIdentity<ApplicationUser, IdentityRole>(config =>
      {
        config.SignIn.RequireConfirmedAccount = false;
        config.User.RequireUniqueEmail = true;
        config.Tokens.AuthenticatorIssuer = "JWT";
        config.SignIn.RequireConfirmedEmail = true;
      })
        .AddEntityFrameworkStores<DomarserviceContext>()
        .AddDefaultTokenProviders();

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidAudience = Configuration["JWT:ValidAudience"],
          ValidIssuer = Configuration["JWT:ValidIssuer"],
          ValidateLifetime = true,
          LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
          {
            return expires >= DateTime.UtcNow;
          },
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
        };
      });
      services.AddFluentValidation(options =>
      {
        // Validate child properties and root collection elements
        options.ImplicitlyValidateChildProperties = true;
        options.ImplicitlyValidateRootCollectionElements = true;
        // Automatic registration of validators in assembly
        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
      }).AddFluentValidationClientsideAdapters();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "domarservice v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      // app.UseCors(builder =>
      // {
      //   builder
      //     .AllowAnyOrigin()
      //     .AllowAnyMethod()
      //     .AllowAnyHeader();
      // });
    }
  }
}
