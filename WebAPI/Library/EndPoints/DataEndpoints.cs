using Microsoft.EntityFrameworkCore;
using WebAPI.Library.Contexts;
using WebAPI.Library.Models;

namespace WebAPI.Library.EndPoints;

public static class DataEndpoints
{
    public static void AddDataEndpoints(this IEndpointRouteBuilder app)
    {
        string url = "/api/URL"; // Common URL Path for all Endpoints

        static async Task<List<DataModel>> GetData(DatabaseContext context) => 
            await context.Log.ToListAsync();

        app.MapGet(url + "", (DatabaseContext context) => 
        { 
            var item = GetData(context);
            return (item == null) ? Results.NotFound("Log is Empty") :
                                    Results.Ok(item); 
        }).WithName("ReadAllLogEntries").WithOpenApi();

        app.MapGet(url + "/{Id}", async (DatabaseContext context, int Id) =>
        {
            var item = await context.Log.FindAsync(Id);
            return (item == null) ? Results.NotFound("Log Entry Not Found") : 
                                    Results.Ok(item); 
        }).WithName("ReadLogById").WithOpenApi();

        app.MapPost(url + "", async (DatabaseContext context, DataModel item) =>
        {
            await context.Log.AddAsync(item);
            await context.SaveChangesAsync();
            return Results.Ok(await GetData(context));
        }).WithName("InsertLogToDatabase").WithOpenApi();

        app.MapPut(url + "/{Id}", async (DatabaseContext context, DataModel? item, int Id) =>
        {
            var logItem = await context.Log.FindAsync(Id);
            if (logItem == null) 
                return Results.NotFound($"Unable To Update Log Entry with ID {Id} was Not Found");
            logItem.Level = item.Level;
            logItem.Message = item.Message;
            logItem.Exception = item.Exception;
            logItem.Properties = item.Properties;
            context.Log.Update(logItem);
            await context.SaveChangesAsync();
            return Results.Ok(await GetData(context));
         }).WithName("UpdateLogById").WithOpenApi();

        app.MapDelete(url + "/{Id}", async (DatabaseContext context, int Id) =>
        {
            var item = await context.Log.FindAsync(Id);
            if (item == null)
                return Results.NotFound($"Unable To Delete Log Entry with ID {Id} was Not Found");
            context.Remove(item);
            await context.SaveChangesAsync();
            return (await context.Log.FindAsync(Id) != null) ?
                Results.Problem("Item Could not ve deleted") :
                Results.Ok(await GetData(context));
        }).WithName("DeleteLogById").WithOpenApi();
    }
}