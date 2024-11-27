using Microsoft.EntityFrameworkCore;

namespace BuscoAPI.Helpers;

public static class HttpContextExtensions
{
    public async static Task InsertPageParameters<T>
        (
        this HttpContext httpContext,
        IQueryable<T> queryable,
        int numberRecordsPerPage
        )
    {
        double cant = await queryable.CountAsync();
        double cantPages = Math.Ceiling(cant / numberRecordsPerPage);
        httpContext.Response.Headers.Add("NumberOfPages", cantPages.ToString());
    }

    public async static Task InsertNumberOfRecords<T>
        (
        this HttpContext httpContext,
        IQueryable<T> queryable
        )
    {
        int countRegisters = await queryable.CountAsync();
        httpContext.Response.Headers.Append("NumberOfRecords", countRegisters.ToString());
    }
}

