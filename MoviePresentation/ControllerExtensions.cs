using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public static class ResultMappingExtensions
{
    public static IActionResult MapResult<T>(this ControllerBase controller, ServiceResult<T> result)
    {
        if (result.Success)
            return controller.Ok(result.Data);

        return result.ErrorType switch
        {
            ErrorTypeEnum.NotFound =>
                controller.NotFound(result.Error),

            //should we do these types of responses insteaD=?
            //return Results.Problem(
            // detail: "The order ID provided is invalid.",
            // statusCode: StatusCodes.Status400BadRequest,
            // title: "Invalid Request"

            ErrorTypeEnum.Validation =>
                controller.BadRequest(result.Error),

            ErrorTypeEnum.Conflict =>
                controller.Conflict(result.Error),

            _ =>
                controller.StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
    public static IActionResult MapResult(this ControllerBase controller, ServiceResult result)
    {
        if (result.Success)
            return controller.NoContent();

        return result.ErrorType switch
        {
            ErrorTypeEnum.NotFound =>
                controller.NotFound(result.Error),

            ErrorTypeEnum.Validation =>
                controller.BadRequest(result.Error),

            ErrorTypeEnum.Conflict =>
                controller.Conflict(result.Error),
            _ =>
                controller.StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }

    public static IActionResult MapCreated<T>(
        this ControllerBase controller,
        ServiceResult<T> result,
        string actionName,
        object routeValues)
    {
        if (!result.Success)
            return controller.MapResult(result);

        return controller.CreatedAtAction(actionName, routeValues, result.Data);
    }
}