using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run(async (HttpContext context) =>
{
    if (context.Request.Path.StartsWithSegments("/"))
    {
        context.Response.Headers["Content-Type"] = "text/html";
        await context.Response.WriteAsync($"The method is: {context.Request.Method} <br/>");
        await context.Response.WriteAsync($"The url is: {context.Request.Path}  <br/>");
        await context.Response.WriteAsync($"The query string is: {context.Request.QueryString}  <br/>");
        await context.Response.WriteAsync($"<b>Headers: <br/>");
        await context.Response.WriteAsync($"<ul>");
        foreach (var key in context.Request.Headers.Keys)
        {
            await context.Response.WriteAsync($"<li><b>{key}<b/>: {context.Request.Headers[key]} </li>");
        }
        await context.Response.WriteAsync($"</ul>");
    }
    else if (context.Request.Path.StartsWithSegments("/emploees"))
    {

        if (context.Request.Method == "GET")
        {
            var employees = EmployeeRepository.GetEmploees();
            foreach (var employee in employees)
            {
                await context.Response.WriteAsync($"Id: {employee.Id}, Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary} \r \n");
            }
        }
        context.Response.StatusCode = 200;

    }

    else if (context.Request.Method == "POST")
    {

        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        var employee = JsonSerializer.Deserialize<Employee>(body);
        EmployeeRepository.AddEmploee(employee);
        context.Response.StatusCode = 201;

    }
    else if (context.Request.Method == "Put")
    {

        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        var employee = JsonSerializer.Deserialize<Employee>(body);
        var result = EmployeeRepository.UpdateEmployee(employee);
        if (result)
        {
            await context.Response.WriteAsync("Employee updated successfully");
        }
        else
        {
            await context.Response.WriteAsync("Employee not found");
        }
        context.Response.StatusCode = 204;
        return;

    }

    else if (context.Request.Method == "Delete")
    {

        if (context.Request.Query.ContainsKey("id"))
        {
            ReadOnlySpan<byte> id = default;
            if (int.TryParse(id, out int employId))
            {
                var result = EmployeeRepository.DeleteEmployee(employId);
                if (result)
                {
                    await context.Response.WriteAsync("Employee deleted successfully");
                }
                else
                {
                    await context.Response.WriteAsync("Employee not found");
                }
            }
        }

    }

});



app.Run();


public static class EmployeeRepository
{
    private static List<Employee> employees = new List<Employee>
    {
        new Employee(1,"JohnDoe", "Engineer", 60000),
        new Employee(2,"Jane Smith", "Manager", 75000),
        new Employee(3,"Sam Brown", "Technician", 5000),
    };
    public static List<Employee> GetEmploees() => employees;
    public static void AddEmploee(Employee? emploee)
    {
        if (emploee != null)
        {
            employees.Add(emploee);
        }
    }
    public static bool UpdateEmployee(Employee? employee)
    {
        if (employee is not null)
        {
            var emp = employees.FirstOrDefault(e => e.Id == employee.Id);
            if (emp is not null)
            {
                emp.Name = employee.Name;
                emp.Position = employee.Position;
                emp.Salary = employee.Salary;
                return true;
            }
        }
        return false;
    }
    public static bool DeleteEmployee(int id)
    {
        var employee = employees.FirstOrDefault(e => e.Id == id);
        if (employee is not null)
        {
            employees.Remove(employee);
            return true;
        }
        return false;
    }

}






public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public double Salary { get; set; }

    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}
