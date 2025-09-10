using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectDemoWebApi.Data;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.Extensions;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services;
using ProjectDemoWebApi.Services.Interface;
using ProjectDemoWebApi.Validation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// Cache for OTP storage
builder.Services.AddDistributedMemoryCache();

// Email configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailService>();

// DbContext configuration
// DbContext configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookConnection")));

builder.Services.AddAuthorization();

// AutoMapper configuration
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(typeof(Program).Assembly);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity configuration
builder.Services.AddIdentity<Users, Roles>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register all repositories and services using extension method
builder.Services.AddApplicationServices();

builder.Services.AddHostedService<UnconfirmedUserCleanupService>();


builder.Services.AddHttpContextAccessor();

// Jwt 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    // Custom error handling
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = ApiResponse<string>.Fail("Unauthorized - Token không hợp lệ hoặc chưa đăng nhập", null, 401);
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var result = ApiResponse<string>.Fail("Forbidden - Không có quyền truy cập", null, 403);
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
        }
    };
});

// Controller configuration with global authorization
builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

// OpenAPI configuration
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IRoleSeederService>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Seed Admin + Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<Users>>();
    var roleManager = services.GetRequiredService<RoleManager<Roles>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new Roles { Name = "Admin" });

    var adminEmail = "sinhndhcmr@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var admin = new Users
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Sinh",
            LastName = "Nguyen",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, "Sinhnd@89");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    var seeder = services.GetService<IRoleSeederService>();
    if (seeder != null)
        await seeder.SeedAsync();
}
app.Run();
