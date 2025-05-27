using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Supabase;
using System.Text;
using Jose;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson();

// Configure authorization policies
// builder.Services.AddAuthorization(options =>
// {
// 	options.AddPolicy("AuthenticatedOnly", policy =>
// 		policy.RequireAuthenticatedUser());
// });
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)	
// 				.AddJwtBearer(options =>
// 				{
// 					var siginKey = new SymmetricSecurityKey(
// 						Encoding.UTF8.GetBytes("django-insecure-@7p$+s+@)&hm)5x37nd+7d^z-k&)&nm3^je4-^5*atg2cmld"));
					
// 					options.TokenValidationParameters = new TokenValidationParameters
// 					{
// 						ValidateIssuerSigningKey = true,
// 						ValidateIssuer = false,
// 						ValidateAudience = false,
// 						ValidateLifetime = true,
// 						IssuerSigningKey = siginKey,
// 						ValidAlgorithms = [SecurityAlgorithms.HmacSha256Signature],
// 						RequireSignedTokens = false,
// 						NameClaimType = "username",
// 						SignatureValidator = delegate (string token, TokenValidationParameters parameters)
// 						{
// 							var jwt = new JsonWebToken(token); // here was JwtSecurityToken
// 							if (parameters.ValidateIssuer && parameters.ValidIssuer != jwt.Issuer)
// 								return null;
// 							return jwt;
// 						},
// 					};
// 					options.Events = new JwtBearerEvents
// 					{
// 						OnAuthenticationFailed = context =>
// 						{
// 							Console.WriteLine("AUTH FAILED: " + context.Exception.Message);
// 							return Task.CompletedTask;
// 						},
// 						OnTokenValidated = context =>
// 						{
// 							Console.WriteLine("TOKEN VALIDATED for " + context.Principal.Identity.Name);
// 							return Task.CompletedTask;
// 						}
// 					};
// 				});

// Register Supabase client
builder.Services.AddScoped<Supabase.Client>(_ =>
	new Supabase.Client(
		builder.Configuration["SupabaseUrl"],
		builder.Configuration["SupabaseKey"],
		new SupabaseOptions
		{
			AutoRefreshToken = true,
			AutoConnectRealtime = true
		}));

// Register the DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o =>
{
	o.CommandTimeout(120); // 2 minutes
}));


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// app.UseAuthorization();
// app.UseAuthentication();

app.MapControllers();

app.Run();
