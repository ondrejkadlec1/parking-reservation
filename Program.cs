using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using ParkingReservation.Data;
using ParkingReservation.Security;
using ParkingReservation.Security.Handlers;
using ParkingReservation.Security.Requirements;
using ParkingReservation.Services;
using ParkingReservation.Services.ReservationService;
using ParkingReservation.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Bearer token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOrAdmin", policy =>
        policy.Requirements.Add(new OwnerOrAdminRequirement()));
    options.AddPolicy("Owner", policy =>
        policy.Requirements.Add(new OwnershipRequirement()));
});

builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IAuthorizationHandler, OwnershipHandler>();
builder.Services.AddScoped<IAuthorizationHandler, OwnerOrAdminHandler>();
builder.Services.AddScoped<IClaimsTransformation, RoleClaimTransformer>();

builder.Services.AddSingleton<GraphClientHelper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReservationWriteService, ReservationsWriteService>();
builder.Services.AddScoped<IReservationReadService, ReservationsReadService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();
app.UseStaticFiles();
app.UseExceptionHandler();

app.Run();
