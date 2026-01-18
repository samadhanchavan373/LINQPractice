// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
Task.Run(() => { });

Task.Run(async () => { await Task.FromResult(1); });

var employees = new List<Employee>
{
    new Employee { Id = 1, Name = "John",     DepartmentId = 1, Salary = 5000,  Age = 28 },
    new Employee { Id = 2, Name = "Sara",     DepartmentId = 2, Salary = 7000,  Age = 32 },
    new Employee { Id = 3, Name = "Mike",     DepartmentId = 1, Salary = 6500,  Age = 45 },
    new Employee { Id = 4, Name = "Linda",    DepartmentId = 3, Salary = 8000,  Age = 38 },
    new Employee { Id = 5, Name = "David",    DepartmentId = 2, Salary = 5500,  Age = 29 },
    new Employee { Id = 6, Name = "Sophia",   DepartmentId = 3, Salary = 9000,  Age = 41 },
    new Employee { Id = 7, Name = "Andrew",   DepartmentId = 1, Salary = 4000,  Age = 24 },
};

var departments = new List<Department>
{
    new Department { Id = 1, Name = "IT" },
    new Department { Id = 2, Name = "HR" },
    new Department { Id = 3, Name = "Finance" }
};

var result1 = employees.Where(e =>
{
    Debug.WriteLine("Evaluating " + e.Id);
    return e.Id > 4;
});

result1.ToList();


#region Moderate
//Get all employees whose salary is above the average salary.
var op1 = employees.Where(e => e.Salary > employees.Select(e => e.Salary).Average()).Select(e => new { e.Id, e.Name, e.Salary });
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op1));
var ooop= op1.First();

var op99 = employees.Where(e => e.Salary > employees.Select(e => e.Salary).Average());

var op100 = op99.Select(e => new { e.Id, e.Name, e.Salary });
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op99));

// Find the highest salary employee in each department.
var op2 = employees.GroupBy(e => e.DepartmentId).
    Select(e => e.OrderByDescending(e => e.Salary).FirstOrDefault())
    .Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new { dName = d.Name,eName = e.Name, e.Salary});
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op2));

// Get the list of employees sorted by Salary descending, then by Name ascending.
var erkjfo4r = employees.Where(e => e.Id > 4);

var op3 = employees.OrderByDescending(e => e.Salary).ThenBy(e => e.Name);
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op3));

// Group employees by DepartmentName and show the total salary per department.
var op4 = employees.GroupBy(e => e.DepartmentId).Select(g => new { DeptId = g.Key, TotalSalary = g.Sum(e => e.Salary) })
    .Join(departments, a => a.DeptId, d => d.Id, (a, d) => new { d.Name, a.TotalSalary });

Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op4));
#endregion


//Find the second highest salary in the company.
#region Advance
var op5 = employees.OrderByDescending(e => e.Salary).FirstOrDefault(e => e.Salary < employees.Max(e => e.Salary));
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op5));

// List employees older than 30 and belonging to IT or Finance departments.
var op6 = employees.Where(e => e.Age > 30).Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new { e, d.Name }).Where(a => a.Name == "IT" || a.Name == "Finance").Select(a => a.e);
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op6));

//Get department-wise employee count including departments with zero employees.

// add one new Department
 departments.Add(
    new Department { Id = 4, Name = "Marketing" });


var op7 = departments.SelectMany(dept => (employees.GroupBy(e => e.DepartmentId).Select(g => new { DeptId = g.Key, NoOfEmps = g.Count() }))
                           .Where(a => a.DeptId == dept.Id).DefaultIfEmpty(),
                           (d, a) => new {
                               DepName = d.Name,
                               EmployeeCount = a?.NoOfEmps ?? 0,
                           });

Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op7));

// From each department, pick the employee with the lowest salary.

var op8 = employees.GroupBy(e => e.DepartmentId).Select(g => new { DeptId = g.Key, MinSal = g.Min(g => g.Salary) })
    .Join(departments, a => a.DeptId, d => d.Id, (a, d) => new { d.Name , a.MinSal});
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op8));
#endregion


#region Complex
// Create a result showing: DepartmentName and a comma-separated list of employee names.

var op9 = employees.GroupBy(e => e.DepartmentId).Select(g => new { DeptId = g.Key, EmpNames = string.Join(',', g.Select(e => e.Name)) })
    .Join(departments, a => a.DeptId, d => d.Id, (a, d) => new { d.Name, a.EmpNames });
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op9));


// Return all employees who earn more than every employee in HR department.
var op10 = employees.Where(e => e.Salary > 
    employees.Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new { DeptName = d.Name, Employee = e })
   .GroupBy(a => a.DeptName)
   .FirstOrDefault(g => g.Key == "HR").Max(a => a.Employee.Salary));

//var hrDeptId = departments.First(d => d.Name == "HR").Id;

//var hrMaxSalary = employees
//    .Where(e => e.DepartmentId == departments.First(d => d.Name == "HR").Id)
//    .Max(e => e.Salary);

var result = employees
    .Where(e => e.Salary > employees
                          .Where(e => e.DepartmentId == departments.First(d => d.Name == "HR").Id)
                          .Max(e => e.Salary))
    .ToList();


Console.WriteLine();
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op10));

// Display employees grouped by Age Range: range, count , names
// Age 20–29
// Age 30–39
// Age 40–49
Console.WriteLine();
var op11 = employees.GroupBy(e => e.Age >= 20 && e.Salary <= 29 ? "20-29" :
                       e.Age >= 30 && e.Age <= 39 ? "30-39" :
                       e.Age >= 40 && e.Age <= 49 ? "40-49" : "other")
    .Select(g => new
    {
        AgeRange = g.Key,
        EmployeeCount = g.Count(),
        Names = string.Join(',', g.Select(e => e.Name))
    });
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op11));

// Find employees whose salary is greater than the average salary of their own department.

var op12 = employees.Where( e => e.Salary > employees.GroupBy(e => e.DepartmentId).
                                 Select(g => new { g.Key, AvgSal = g.Average(e => e.Salary) }).FirstOrDefault(a => a.Key == e.DepartmentId).AvgSal);
Console.WriteLine();
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(op12));



var sentences = new[]
{
    "Hello my",
    "LINQ practice"
};

var chars = sentences.SelectMany(s => s);


Console.WriteLine(default(string));

Console.WriteLine(default(String));



List<int> ls = new List<int> { 1, 2, 3, 4, 5 };


var num = ls.FirstOrDefault(n => n > 10);

Console.WriteLine(num);  // 0 not null


#endregion







