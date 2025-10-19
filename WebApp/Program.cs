var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run(async (HttpContext context) =>
{
    await context.Response.WriteAsync($"The method is: {context.Request.Method} \r \n");
    await context.Response.WriteAsync($"The url is: {context.Request.Path} \r \n");
    await context.Response.WriteAsync($"The query string is: {context.Request.QueryString} \r \n");
    await context.Response.WriteAsync($"The headers are: \r \n");
    foreach (var key in context.Request.Headers.Keys)
    {
        await context.Response.WriteAsync($"{key}: {context.Request.Headers[key]} \r \n");
    }

});

app.Run();
