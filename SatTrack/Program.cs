
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SatTrack.Configs;
using SatTrack.Contracts.Consumers;
using SatTrack.DAL;
using SatTrack.Services;
using SatTrack.Services.Interfaces;
using SatTrack.Util;
using System.Text;

namespace SatTrack
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            appSettingsConfiguration.Initialize(builder.Configuration);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourdomain.com",
                    ValidAudience = "yourdomain.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettingsConfiguration.GetSetting("SecretKey")))
                };
            });
            builder.Services.AddDbContext<ElderveilContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("ElderveilConnectionString"));
            });

            builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
            builder.Services.Configure<CelestrakSettings>(builder.Configuration.GetSection("CelestrakApi"));
            builder.Services.AddHttpClient<CelestrakClientService>();

            builder.Services.AddScoped<PasswordService>();
            builder.Services.AddScoped<IRoleService,RoleService>();
            builder.Services.AddScoped<IUserService,UserService>();
            builder.Services.AddScoped<ISatGroupService,SatGroupService>();
            builder.Services.AddScoped<ISatService,SatService>();
            builder.Services.AddAuthorization(options =>
            {
                foreach(string r in Roles.RoleList)
                {
                    options.AddPolicy(r, policy => policy.RequireRole(r));
                }
            });

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<GroupMessageConsumer>();
                x.AddConsumer<SatMessageConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQSettings = context.GetRequiredService<IConfiguration>().GetSection("RabbitMQ").Get<RabbitMQSettings>();
                    cfg.Host(rabbitMQSettings.Hostname, h =>
                    {
                        h.Username(rabbitMQSettings.Username);
                        h.Password(rabbitMQSettings.Password);
                    });
                    cfg.ConfigureEndpoints(context);
                    cfg.ReceiveEndpoint("GroupUpdate", e =>
                    {
                        e.ConfigureConsumer<GroupMessageConsumer>(context);
                    });
                    cfg.ReceiveEndpoint("SatelliteQueue", e =>
                    {
                        e.PrefetchCount = 200;
                        e.ConcurrentMessageLimit = 100;
                        e.ConfigureConsumer<SatMessageConsumer>(context);
                    });
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter Bearer [space] and then your token in the text input below."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });

            var app = builder.Build();
            app.UseAuthentication();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
