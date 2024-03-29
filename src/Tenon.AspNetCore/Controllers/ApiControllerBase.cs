﻿using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Tenon.AspNetCore.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    [NonAction]
    protected virtual ActionResult Problem(string error, int statusCode)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = error,
            Instance = Request.Path.ToString(),
            Status = statusCode,
            Title = null,
            Type = null
        };
        return Problem(problemDetails.Detail
            , problemDetails.Instance
            , problemDetails.Status
            , problemDetails.Title
            , problemDetails.Type);
    }

    [NonAction]
    protected virtual ActionResult Problem(string error, HttpStatusCode statusCode)
    {
        return Problem(error, (int)statusCode);
    }

    [NonAction]
    protected virtual ActionResult<T> Result<T>(T data)
    {
        if (data == null) return NoContent();
        return data;
    }

    [NonAction]
    protected virtual ActionResult<T> CreatedResult<T>(T data)
    {
        return Created(Request.Path, data);
    }
}