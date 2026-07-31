using System.Text.Json;
using IDMS.Api.Middleware;
using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Services;
using IDMS.Modules.Api.Master.Services.Impl;
using IDMS.Shared.Common;
using IDMS.Shared.Exceptions;
using IDMS.Shared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter JWT"
    });

    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter Api Key"
    });

    options.AddSecurityRequirement(doc =>
    {
        var requirement = new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            {new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", doc, null), []},
            {new Microsoft.OpenApi.OpenApiSecuritySchemeReference("ApiKey", doc, null), []},
        };

        return requirement;
    });
});

builder.Services.AddScoped<IMstBrandService, MstBrandService>();
builder.Services.AddScoped<IMstTypeService, MstTypeService>();
builder.Services.AddScoped<IMstModelService, MstModelService>();
builder.Services.AddScoped<IMstCustomerService, MstCustomerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserServices, CurrentUserServices>();

builder.Services.AddScoped<IMstDealerService, MstDealerService>();
builder.Services.AddScoped<IMstInsuranceService, MstInsuranceService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
        .Where(e => e.Value?.Errors.Count > 0)
        .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage))
        .ToList();

        var response = new ApiResponse<object>
        {
            ReqId = context.HttpContext.TraceIdentifier,
            Status = "Error",
            Message = string.Join("; ", errors),
            Data = null
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    };
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnChallenge = context =>
        {
            throw new UnauthorizedException("Unauthorized");
        },
        OnForbidden = context =>
        {
            throw new ForbiddenException("Forbidden");
        }
    };
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage))
                .ToList();

            var response = new ApiResponse<object>
            {
                ReqId = context.HttpContext.TraceIdentifier,
                Status = "error",
                Message = string.Join("; ", errors),
                Data = null
            };

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
