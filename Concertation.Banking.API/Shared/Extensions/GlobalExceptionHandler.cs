using Concertation.Banking.API.Shared.Dtos;
using Concertation.Banking.API.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Concertation.Banking.API.Shared.Extensions;

public static class GlobalExceptionHandler
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                IExceptionHandlerFeature? errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (errorFeature is null)
                    return;

                Exception exception = errorFeature.Error;
                int code = 500;

                if (exception is AppException appEx)
                    code = appEx.StatusCode;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = code;

                var response = new ApiResponseDto<object>(
                    Success: false,
                    Data: null,
                    Errors: [exception.Message]);

                await context.Response.WriteAsJsonAsync(response);
            });
        });
    }
}
