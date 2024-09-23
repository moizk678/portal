using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseService _databaseService;

        public HomeController(ILogger<HomeController> logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        // Action to test database connection
        public IActionResult TestConnection()
        {
            bool isConnected = _databaseService.TestDatabaseConnection();
            if (isConnected)
            {
                return Content("Database connection successful!");
            }
            else
            {
                return Content("Database connection failed.");
            }
        }

        // Display the main page
        public IActionResult Index()
        {
            return View();
        }

        // Show the registration form
        public IActionResult Register()
        {
            return View();
        }

        // Handle form submission
        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Register the customer
                    bool success = _databaseService.RegisterCustomer(customer);
                    if (success)
                    {
                        _logger.LogInformation("Customer registered successfully.");

                        // Store a success message in TempData
                        TempData["SuccessMessage"] = "Registration successful! You can now log in.";

                        return RedirectToAction("Success");
                    }
                    else
                    {
                        _logger.LogError("Registration failed for the customer.");
                        ModelState.AddModelError("", "There was an issue registering the customer. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while registering the customer.");
                    ModelState.AddModelError("", "An unexpected error occurred. Please contact support.");
                }
            }

            // Return the form with validation errors if needed
            return View(customer);

        }



        // Update existing customer details
        [HttpPost]
        public JsonResult UpdateCustomer([FromBody] Customer customer)
        {
            try
            {
                if (customer.Id <= 0)
                {
                    _logger.LogWarning("Customer ID is missing or invalid.");
                    return Json(new { success = false, message = "Customer ID is missing or invalid." });
                }

                _logger.LogInformation("Attempting to update customer with ID: {Id}", customer.Id);

                bool success = _databaseService.UpdateCustomer(customer);

                if (success)
                {
                    _logger.LogInformation("Customer with ID {Id} updated successfully.", customer.Id);
                    return Json(new { success = true });
                }
                else
                {
                    _logger.LogWarning("No changes detected or customer not found for ID {Id}.", customer.Id);
                    return Json(new { success = false, message = "No changes detected or customer not found." });
                }
            }
            catch (Exception ex)
            {
                // Add more details to the log, including the customer ID and any other data.
                _logger.LogError(ex, "An error occurred while updating the customer with ID {Id}. Customer Data: {Customer}", customer.Id, customer);
                return Json(new { success = false, message = "An error occurred while updating the customer." });
            }
        }
        [HttpPost]
        public JsonResult DeleteCustomer([FromBody] Customer customer)
        {
            try
            {
                if (customer.Id <= 0)
                {
                    _logger.LogWarning("Customer ID is missing or invalid.");
                    return Json(new { success = false, message = "Customer ID is missing or invalid." });
                }

                _logger.LogInformation("Attempting to delete customer with ID: {Id}", customer.Id);

                bool success = _databaseService.DeleteCustomer(customer.Id);

                if (success)
                {
                    _logger.LogInformation("Customer with ID {Id} deleted successfully.", customer.Id);
                    return Json(new { success = true });
                }
                else
                {
                    _logger.LogWarning("Customer not found for ID {Id}.", customer.Id);
                    return Json(new { success = false, message = "Customer not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the customer with ID {Id}.", customer.Id);
                return Json(new { success = false, message = "An error occurred while deleting the customer." });
            }
        }





        // Display the success page after registration
        public IActionResult Success()
        {
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            return View();
        }

        // New action to fetch registered customers as JSON
        [HttpGet]
        public JsonResult GetRegisteredCustomers()
        {
            try
            {
                // Retrieve all customers with their details
                var customers = _databaseService.GetAllCustomers().Select(c => new
                {
                    c.Id,  // Include the ID field in the response
                    c.FullName,
                    c.DateOfBirth,
                    c.Gender,
                    c.MaritalStatus,
                    c.ResidentialAddress,
                    c.PhoneNumber,
                    c.EmailAddress,
                    c.NationalIdNumber,
                    c.AnnualIncome,
                    c.AccountType,
                    c.InitialDepositAmount
                }).ToList();

                return Json(customers); // Return the customers as JSON
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customers.");
                return Json(new { error = "An error occurred while fetching customers." });
            }
        }

        // Handle errors or unexpected issues
        public IActionResult Error()
        {
            return View();
        }
    }
}
