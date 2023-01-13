using CustomerAPI.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace CustomerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;

        private string dataPath;

        private JsonSerializerOptions options;

        public CustomerController(ILogger<CustomerController> logger, IWebHostEnvironment webHostEnvironment)
        {
            dataPath = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "customers.json");
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _logger = logger;
        }

        private List<Customer> AllCustomers
        {
            get
            {
                if (!System.IO.File.Exists(dataPath))
                {
                    var customers = new List<Customer>();
                    AllCustomers = customers;
                    return customers;
                }

                var jsonData = System.IO.File.ReadAllText(dataPath, Encoding.UTF8);
                return JsonSerializer.Deserialize<List<Customer>>(jsonData, options);
            }
            set
            {
                string textData = JsonSerializer.Serialize(value, options);
                System.IO.File.WriteAllText(dataPath, textData, Encoding.UTF8);
            }
        }


        [HttpGet]
        [Route("customers")]
        public IEnumerable<Customer> Get()
        {
            return AllCustomers;
        }

        [HttpPost]
        [Route("customers")]
        public ActionResult Customers(Customer[] customers)
        {
            if (customers == null)
            {
                return ValidationProblem($"{nameof(Customer)} can't be null.");
            }

            if (customers.Length < 2)
            {
                return ValidationProblem($"{nameof(Customer)} should contain at least 2 items.");
            }

            if (customers.Any(customer => customer.Age <= 18))
            {
                return ValidationProblem($"age should be above 18.");
            }

            if (customers.Any(customer => AllCustomers.Exists(existing => existing.Id == customer.Id)))
            {
                return ValidationProblem($"Id's should be unique.");
            }

            if (customers.Any(customer => !Generator.IsValidCustomer(customer)))
            {
                return ValidationProblem($"Make sure all fields are supplied and Id's > 0");
            }

            foreach (var customer in customers)
            {
                AddCustomer(customer);
            }

            return Content("OK");
        }

        private void AddCustomer(Customer customer)
        {
            if (AllCustomers.Count == 0)
            {
                AllCustomers.Add(customer);
                return;
            }

            for (int i = 0; i < AllCustomers.Count; i++)
            {
                if (AllCustomers[i].Id < customer.Id)
                {
                    AllCustomers.Insert(i, customer);
                    return;
                }
            }

            AllCustomers.Add(customer);
        }
    }
}
