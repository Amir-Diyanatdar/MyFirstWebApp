using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run(static async (HttpContext context) =>
{
    if (context.Request.Method == "GET")
    {
        if (context.Request.Path.StartsWithSegments("/"))
        {
            await context.Response.WriteAsync($"The method is: {context.Request.Method} \r \n");
            await context.Response.WriteAsync($"The url is: {context.Request.Path} \r \n");
            await context.Response.WriteAsync($"The query string is: {context.Request.QueryString} \r \n");
            await context.Response.WriteAsync($"The headers are: \r \n");
            foreach (var key in context.Request.Headers.Keys)
            {
                await context.Response.WriteAsync($"{key}: {context.Request.Headers[key]} \r \n");
            }
        }
        else if (context.Request.Path.StartsWithSegments("/emploees"))
        {
            var emploees = EmploeeRepository.GetEmploees();
            foreach (var emploee in emploees)
            {
                await context.Response.WriteAsync($"Id: {emploee.Id}, Name: {emploee.Name}, Position: {emploee.Position}, Salary: {emploee.Salary} \r \n");
            }



        }
        //else if (context.Request.Path.StartsWithSegments("/emploees"))
        //{
        //    await context.Response.WriteAsync("employee list");
        //}

    }
    else if (context.Request.Method == "POST")
    {
        if (context.Request.Path.StartsWithSegments("/emploees"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var emploee = JsonSerializer.Deserialize<Emploee>(body);
            EmploeeRepository.AddEmploee(emploee);

        }
    }
    else if (context.Request.Method == "Put")
    {
        if (context.Request.Path.StartsWithSegments("/emploees"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var emploee = JsonSerializer.Deserialize<Emploee>(body);          
            var result = EmploeeRepository.UpdateEmploee(emploee);
            if (result)
            {
                await context.Response.WriteAsync("Employee updated successfully");
            }
            else
            {
                await context.Response.WriteAsync("Employee not found");
            }
        }
    }


});

app.Run();


public static class EmploeeRepository
{
    private static List<Emploee> emploees = new List<Emploee>
    {
        new Emploee(1,"JohnDoe", "Engineer", 60000),
        new Emploee(2,"Jane Smith", "Manager", 75000),
        new Emploee(3,"Sam Brown", "Technician", 5000),
    };
    public static List<Emploee> GetEmploees() => emploees;
    public static void AddEmploee(Emploee? emploee)
    {
        if (emploee != null)
        {
            emploees.Add(emploee);
        }
    }
    public static bool UpdateEmploee(Emploee? emploee)
    {
        if (emploee is not null)
        {
            var emp = emploees.FirstOrDefault(e => e.Id == emploee.Id);
            if (emp is not null)
            {
                emp.Name = emploee.Name;
                emp.Position = emploee.Position;
                emp.Salary = emploee.Salary;
                return true;
            }
        }
        return false;
    }

}






public class Emploee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public double Salary { get; set; }

    public Emploee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}
