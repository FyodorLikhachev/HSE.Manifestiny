using HSE.Manifestiny.Configuration;
using HSE.Manifestiny.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ApplicationContext>();

// Add services to the container.
builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidIssuer = AuthOptions.ISSUER,
			ValidateAudience = false,
			ValidAudience = AuthOptions.AUDIENCE,
			ValidateIssuerSigningKey = false,
			IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
			ValidateLifetime = false,
		};
	});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swagger =>
	{
		//This is to generate the Default UI of Swagger Documentation
		swagger.SwaggerDoc("v1", new OpenApiInfo
		{
			Version = "v1",
			Title = "ASP.NET 6 Web API",
			Description = "Authentication and Authorization in ASP.NET 6 with JWT and Swagger"
		});
		// To Enable authorization using Swagger (JWT)
		swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = SecuritySchemeType.ApiKey,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
		});
		swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
				new string[] {}
			}
		});
	});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(config => config.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET 6 Web API"));

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();