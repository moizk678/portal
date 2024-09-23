using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public class Customer
    {
        [Key]
        [Column("ID")]  // Map C# property Id to SQL column ID
        public int Id { get; set; }  // Keep this as Id in your code for consistency
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Marital status is required")]
        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }

        [Required(ErrorMessage = "Residential address is required")]
        [StringLength(200, ErrorMessage = "Residential address cannot exceed 200 characters")]
        [Display(Name = "Residential Address")]
        public string ResidentialAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "National ID number is required")]
        [StringLength(20, ErrorMessage = "National ID number cannot exceed 20 characters")]
        [Display(Name = "National ID Number")]
        public string NationalIdNumber { get; set; }

        [Required(ErrorMessage = "Annual income is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid income amount")]
        [DataType(DataType.Currency)]
        [Display(Name = "Annual Income")]
        public decimal AnnualIncome { get; set; }

        [Required(ErrorMessage = "Account type is required")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Required(ErrorMessage = "Initial deposit amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid deposit amount greater than zero")]
        [DataType(DataType.Currency)]
        [Display(Name = "Initial Deposit Amount")]
        public decimal InitialDepositAmount { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        [Display(Name = "Username")]
        public string Username { get; set; } // Remove 'required' keyword if not always required

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be at least 8 characters long", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one letter and one number")]
        [Display(Name = "Password")]
        public string Password { get; set; } // Remove 'required' keyword if not always required
    }
}


