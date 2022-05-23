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
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

      // services.AddControllers();
      services.AddControllers().AddJsonOptions(x =>
          x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "domarservice", Version = "v1" });
      });
      services.AddDbContext<DomarserviceContext>(options =>
          options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
      services.AddScoped<IRefereeRepository, RefereeRepository>();
      services.AddScoped<IScheduleRepository, ScheduleRepository>();
      services.AddScoped<ICompanyRepository, CompanyRepository>();
      services.AddScoped<ICompanyEventRepository, CompanyEventRepository>();
      var mapperConfig = new MapperConfiguration(mc =>
        {
          mc.AddProfile(new AutoMapperProfile());
        });
      // services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

      IMapper mapper = mapperConfig.CreateMapper();
      services.AddSingleton(mapper);

      services.AddIdentity<IdentityUser, IdentityRole>()
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
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
        };
      });
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
    }
  }
}
