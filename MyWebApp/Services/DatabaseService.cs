using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; // For List<>
using System.Data; // For DataTable
using BCrypt.Net;  // For password hashing
using Microsoft.Extensions.Configuration;
using MyWebApp.Models; // Assuming the Customer model is in this namespace

namespace MyWebApp.Services
{
    public class DatabaseService
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly string _connectionString;

        public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Method to test database connection
        public bool TestDatabaseConnection()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    _logger.LogInformation("Database connection successful.");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database connection failed.");
                    return false;
                }
            }
        }

        // Method to register a customer
        public bool RegisterCustomer(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"INSERT INTO Customers2 (FullName, DateOfBirth, Gender, MaritalStatus, ResidentialAddress, 
                                     PhoneNumber, EmailAddress, NationalID, AnnualIncome, AccountType, InitialDeposit, 
                                     Username, Password)
                                     VALUES (@FullName, @DateOfBirth, @Gender, @MaritalStatus, @ResidentialAddress, @PhoneNumber, 
                                     @EmailAddress, @NationalIdNumber, @AnnualIncome, @AccountType, @InitialDepositAmount, 
                                     @Username, @Password)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(customer.Password);

                        cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = customer.FullName;
                        cmd.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value = customer.DateOfBirth;
                        cmd.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = customer.Gender;
                        cmd.Parameters.Add("@MaritalStatus", SqlDbType.NVarChar).Value = customer.MaritalStatus;
                        cmd.Parameters.Add("@ResidentialAddress", SqlDbType.NVarChar).Value = customer.ResidentialAddress;
                        cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = customer.PhoneNumber;
                        cmd.Parameters.Add("@EmailAddress", SqlDbType.NVarChar).Value = customer.EmailAddress;
                        cmd.Parameters.Add("@NationalIdNumber", SqlDbType.NVarChar).Value = customer.NationalIdNumber;
                        cmd.Parameters.Add("@AnnualIncome", SqlDbType.Decimal).Value = customer.AnnualIncome;
                        cmd.Parameters.Add("@AccountType", SqlDbType.NVarChar).Value = customer.AccountType;
                        cmd.Parameters.Add("@InitialDepositAmount", SqlDbType.Decimal).Value = customer.InitialDepositAmount;
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = customer.Username;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = hashedPassword;

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            _logger.LogInformation("Customer {CustomerFullName} registered successfully.", customer.FullName);
                            return true;
                        }
                        else
                        {
                            _logger.LogWarning("Customer registration did not affect any rows in the database.");
                            return false;
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    _logger.LogError(sqlEx, "Database error occurred while registering customer {CustomerFullName}.", customer.FullName);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while registering customer {CustomerFullName}.", customer.FullName);
                    return false;
                }
            }
        }



        public bool UpdateCustomer(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // Log the ID to ensure the correct customer is being fetched
                    _logger.LogInformation("Updating customer with ID: {ID}", customer.Id);

                    // Fetch existing customer data
                    string selectQuery = "SELECT * FROM Customers2 WHERE ID = @ID";
                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, connection))
                    {
                        selectCmd.Parameters.AddWithValue("@ID", customer.Id);

                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Building dynamic update query
                                string updateQuery = "UPDATE Customers2 SET ";
                                List<SqlParameter> parameters = new List<SqlParameter>();
                                bool hasChanges = false;

                                // Check for changes in each field and add to query if different
                                if (reader["FullName"].ToString() != customer.FullName)
                                {
                                    updateQuery += "FullName = @FullName, ";
                                    parameters.Add(new SqlParameter("@FullName", customer.FullName));
                                    hasChanges = true;
                                }

                                if (Convert.ToDateTime(reader["DateOfBirth"]) != customer.DateOfBirth)
                                {
                                    updateQuery += "DateOfBirth = @DateOfBirth, ";
                                    parameters.Add(new SqlParameter("@DateOfBirth", customer.DateOfBirth));
                                    hasChanges = true;
                                }

                                if (reader["Gender"].ToString() != customer.Gender)
                                {
                                    updateQuery += "Gender = @Gender, ";
                                    parameters.Add(new SqlParameter("@Gender", customer.Gender));
                                    hasChanges = true;
                                }

                                if (reader["MaritalStatus"].ToString() != customer.MaritalStatus)
                                {
                                    updateQuery += "MaritalStatus = @MaritalStatus, ";
                                    parameters.Add(new SqlParameter("@MaritalStatus", customer.MaritalStatus));
                                    hasChanges = true;
                                }

                                if (reader["ResidentialAddress"].ToString() != customer.ResidentialAddress)
                                {
                                    updateQuery += "ResidentialAddress = @ResidentialAddress, ";
                                    parameters.Add(new SqlParameter("@ResidentialAddress", customer.ResidentialAddress));
                                    hasChanges = true;
                                }

                                if (reader["PhoneNumber"].ToString() != customer.PhoneNumber)
                                {
                                    updateQuery += "PhoneNumber = @PhoneNumber, ";
                                    parameters.Add(new SqlParameter("@PhoneNumber", customer.PhoneNumber));
                                    hasChanges = true;
                                }

                                if (reader["EmailAddress"].ToString() != customer.EmailAddress)
                                {
                                    updateQuery += "EmailAddress = @EmailAddress, ";
                                    parameters.Add(new SqlParameter("@EmailAddress", customer.EmailAddress));
                                    hasChanges = true;
                                }

                                if (reader["NationalID"].ToString() != customer.NationalIdNumber)
                                {
                                    updateQuery += "NationalID = @NationalIdNumber, ";
                                    parameters.Add(new SqlParameter("@NationalIdNumber", customer.NationalIdNumber));
                                    hasChanges = true;
                                }

                                if (Convert.ToDecimal(reader["AnnualIncome"]) != customer.AnnualIncome)
                                {
                                    updateQuery += "AnnualIncome = @AnnualIncome, ";
                                    parameters.Add(new SqlParameter("@AnnualIncome", customer.AnnualIncome));
                                    hasChanges = true;
                                }

                                if (reader["AccountType"].ToString() != customer.AccountType)
                                {
                                    updateQuery += "AccountType = @AccountType, ";
                                    parameters.Add(new SqlParameter("@AccountType", customer.AccountType));
                                    hasChanges = true;
                                }

                                if (Convert.ToDecimal(reader["InitialDeposit"]) != customer.InitialDepositAmount)
                                {
                                    updateQuery += "InitialDeposit = @InitialDepositAmount, ";
                                    parameters.Add(new SqlParameter("@InitialDepositAmount", customer.InitialDepositAmount));
                                    hasChanges = true;
                                }

                                // If any field has changes, execute the update query
                                if (hasChanges)
                                {
                                    // Remove last comma and space, then add WHERE clause
                                    updateQuery = updateQuery.TrimEnd(',', ' ') + " WHERE ID = @ID";
                                    parameters.Add(new SqlParameter("@ID", customer.Id));

                                    _logger.LogInformation("Executing Update Query: {Query}", updateQuery);

                                    // Close the reader before executing the update
                                    reader.Close();

                                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                                    {
                                        updateCmd.Parameters.AddRange(parameters.ToArray());
                                        int rowsAffected = updateCmd.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            _logger.LogInformation("Customer with ID {ID} updated successfully.", customer.Id);
                                            return true;
                                        }
                                        else
                                        {
                                            _logger.LogWarning("No rows affected. Query executed: {Query}", updateQuery);
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("No changes detected for customer with ID {ID}.", customer.Id);
                                    return false;
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Customer with ID {ID} not found.", customer.Id);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the customer with ID {ID}.", customer.Id);
                    return false;
                }
            }
        }



        public bool DeleteCustomer(int customerId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM Customers2 WHERE ID = @Id"; // Make sure "ID" matches the database column name
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", customerId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the customer with ID {Id}.", customerId);
                    return false;
                }
            }
        }






        // New method to retrieve all registered customers from the database
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

string query = @"SELECT 
                    ID,  -- Include the ID column to fetch the primary key
                    FullName, 
                    DateOfBirth, 
                    Gender, 
                    MaritalStatus, 
                    ResidentialAddress, 
                    PhoneNumber, 
                    EmailAddress, 
                    NationalID, 
                    AnnualIncome, 
                    AccountType, 
                    InitialDeposit, 
                    Username, 
                    Password 
                FROM Customers2";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var customer = new Customer
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    FullName = reader["FullName"].ToString(),
                                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                    Gender = reader["Gender"].ToString(),
                                    MaritalStatus = reader["MaritalStatus"].ToString(),
                                    ResidentialAddress = reader["ResidentialAddress"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    EmailAddress = reader["EmailAddress"].ToString(),
                                    NationalIdNumber = reader["NationalID"].ToString(),
                                    AnnualIncome = reader["AnnualIncome"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AnnualIncome"]),
                                    AccountType = reader["AccountType"].ToString(),
                                    InitialDepositAmount = reader["InitialDeposit"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["InitialDeposit"]),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString()
                                };
                                customers.Add(customer);
                            }
                        }
                    }
                    _logger.LogInformation("Retrieved {CustomerCount} customers from the database.", customers.Count);
                }
                catch (SqlException sqlEx)
                {
                    _logger.LogError(sqlEx, "Database error occurred while retrieving customers.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving customers.");
                }
            }

            return customers;
        }

    }
}
