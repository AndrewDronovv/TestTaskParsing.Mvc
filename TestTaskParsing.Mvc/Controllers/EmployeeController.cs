using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TestTaskParsing.Domain;

public class EmployeeController : Controller
{
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Employee/Index
    public IActionResult Index(string searchString, DateTime? startDate, DateTime? endDate)
    {
        var employees = from e in _context.Employees
                        select e;

        //Search by parameter
        if (!string.IsNullOrEmpty(searchString))
        {
            employees = employees.Where(e => e.PayrollNumber.Contains(searchString) ||
            e.ForeName.Contains(searchString) ||
            e.Surname.Contains(searchString) ||
            e.Telephone.Contains(searchString) ||
            e.Mobile.Contains(searchString) ||
            e.Address.Contains(searchString) ||
            e.City.Contains(searchString) ||
            e.PostCode.Contains(searchString) ||
            e.EMailHome.Contains(searchString));
        }
        //Search by startDate and endDate
        if (startDate.HasValue && endDate.HasValue)
        {
            employees = employees.Where(e => e.DateOfBirth >= startDate.Value.Date &&
                                             e.DateOfBirth <= endDate.Value.Date);
        }
        else if (startDate.HasValue)
        {
            employees = employees.Where(e => e.DateOfBirth >= startDate.Value.Date);
        }
        else if (endDate.HasValue)
        {
            employees = employees.Where(e => e.DateOfBirth <= endDate.Value.Date);
        }

        //Sort employees by Surname
        employees = employees.OrderBy(e => e.Surname);


        return View(employees.ToList());
    }

    [HttpPost]
    public IActionResult Import(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using (var reader = new StreamReader(file.OpenReadStream())) //Creates a StreamReader to read the contents of the file.
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) // Creates an instance of CSVReader for parsing CSV data.
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                HeaderValidated = null,
            }))
            {
                csv.Context.RegisterClassMap<EmployeeMap>(); // Registers ClassMap
                var records = csv.GetRecords<Employee>().ToList();
                _context.Employees.AddRange(records);
                _context.SaveChangesAsync();
                TempData["Message"] = $"{records.Count} records imported successfully!";
            }
        }
        else
        {
            TempData["Message"] = "File not selected or empty!";
        }
        return RedirectToAction(nameof(Index));
    }
    //GET - EDIT
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var employee = _context.Employees.Find(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: Employee/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(employee);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(employee);
    }

    //GET - DELETE
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var obj = _context.Employees.Find(id);
        if (obj == null)
        {
            return NotFound();
        }
        return View(obj);
    }


    //POST - DELETE
    [HttpPost]
    [ValidateAntiForgeryToken] //Use for protection against an attack, create a unique token
    public IActionResult DeletePost(int? id)
    {
        var obj = _context.Employees.Find(id);
        if (obj == null)
        {
            return NotFound();
        }
        _context.Employees.Remove(obj);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}