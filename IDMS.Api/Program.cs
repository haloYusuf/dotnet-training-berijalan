using System.Text.Json;
using IDMS.Api.Middleware;
using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Services;
using IDMS.Modules.Api.Master.Services.Impl;
using IDMS.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMstBrandService, MstBrandService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
