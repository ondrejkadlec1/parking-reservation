using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace ParkingReservation.Services
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.ContentType = Text.Plain;
            if (exception is BadHttpRequestException badRequestException) 
            {
                httpContext.Response.StatusCode = badRequestException.StatusCode;
                await httpContext.Response.WriteAsync(badRequestException.Message);
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync("Objevila se neočekávaná chyba.");
            }
            return true;
        }
    }
}
