using Azure.Core;
using BL.Constants;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddDbContext<PerspectiveContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("PerspectiveConnection"), sqlOptions => sqlOptions.UseNetTopologySuite())
	);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
	.AddEntityFrameworkStores<PerspectiveContext>()
	.AddDefaultTokenProviders();


//// Add services to the container.
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var supportedCultures = new[] { "en-US", "fr-FR", "pt-BR" };
	options.SetDefaultCulture(supportedCultures[0])
		.AddSupportedCultures(supportedCultures)
		.AddSupportedUICultures(supportedCultures);
});

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache(); // Add session state


//builder.Services.AddControllersWithViews()
//    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
//    .AddDataAnnotationsLocalization();

// Add services to the container
builder.Services.AddControllersWithViews()
	.AddRazorRuntimeCompilation()
	.AddViewLocalization()
	.AddDataAnnotationsLocalization();

builder.Services.AddControllers(); // Required for API controllers

builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
builder.Services.AddScoped(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "OAuthProvider";
})
.AddCookie()
.AddOAuth("OAuthProvider", options =>
{
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.CallbackPath = new PathString("/signin-oauth");
    options.AuthorizationEndpoint = "https://provider.com/oauth/authorize";
    options.TokenEndpoint = "https://provider.com/oauth/token";
    options.UserInformationEndpoint = "https://provider.com/oauth/userinfo";
    options.SaveTokens = true;

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var user = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            context.RunClaimActions(user.RootElement);
        }
    };
});

builder.Services.AddAuthorization(); // Optional, for role-based policies if needed

//builder.Services.AddAuthentication(options =>
//{
//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//	options.TokenValidationParameters = new TokenValidationParameters
//	{
//		ValidateIssuer = true,
//		ValidateAudience = true,
//		ValidateLifetime = false,
//		ValidateIssuerSigningKey = true,

//		ValidIssuer = builder.Configuration["JWTConfiguration:Issuer"],
//		ValidAudience = builder.Configuration["JWTConfiguration:Audience"],
//		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTConfiguration:Secret"]))
//	};
//	options.Events = new JwtBearerEvents
//	{
//		OnMessageReceived = context =>
//		{
//			var path = context.HttpContext.Request.Path;
//			if (path.StartsWithSegments("/api"))
//			{
//				var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
//				if (!string.IsNullOrEmpty(token))
//				{
//					context.Token = token;
//				}
//			}
//			return Task.CompletedTask;
//		},
//		OnChallenge = context =>
//		{
//			context.HandleResponse();
//			context.Response.StatusCode = 401;
//			context.Response.ContentType = "application/json";
//			return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { ErrorMessage = APIErrorMessage.TokenExpire, QryResult = "expired" }));
//		},
//		OnForbidden = context =>
//		{
//			context.Response.StatusCode = 403;
//			context.Response.ContentType = "application/json";
//			return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { ErrorMessage = APIErrorMessage.TokenExpire, QryResult = "expired" }));
//		}
//	};
//});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//	.AddCookie(options =>
//	{
//		options.LoginPath = "/Login";
//		options.AccessDeniedPath = "/Home/AccessDenied";
//	});


// Add services to the container.
builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", builder =>
	{
		builder
			.AllowAnyMethod()
			.AllowAnyHeader()
			.SetIsOriginAllowed(origin => true) // Allow any origin
			.AllowCredentials();
	});
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
	var db = scope.ServiceProvider.GetRequiredService<PerspectiveContext>();
	//db.Database.EnsureCreated();
	db.Database.Migrate();
	SeedService.SeedData(userManager, roleManager);

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Enable session middleware

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Login}/{action=Index}/{id?}");

// Map API routes
app.MapControllers();


app.Run();
