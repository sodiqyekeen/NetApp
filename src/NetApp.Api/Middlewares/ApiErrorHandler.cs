using System.Net;
using NetApp.Domain.Exceptions;
using NetApp.Models;

namespace NetApp.Api;
public class ApiErrorHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiErrorHandler> _logger;

    public ApiErrorHandler(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ApiErrorHandler>();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = new Response()
            {
                Succeeded = false,
                Message = error is ApiException ? error.Message : ""
            };


            switch (error)
            {
                case InvalidEmailAddressException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException e:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ApiException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                // case RequestValidationException e:
                //     responseModel.Messages.AddRange(e.Errors);
                //     break;
                default:
                    _logger.LogError(error.Message, error);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    if (string.IsNullOrWhiteSpace(responseModel.Message))
                    {
#if DEBUG
                        responseModel.Message = error.ToString();
#else
                            responseModel.Message= "An error occurred.";
#endif
                    }
                    break;

            }
            await response.WriteAsJsonAsync(responseModel);
        }
    }
}