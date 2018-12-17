using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        
        public int UserId {get;set;}

            [Required(ErrorMessage = "First Name cannot be empty!!")]
            [MinLength(2, ErrorMessage="First Name cannot be less than 2 characters!!")]
        public string FirstName {get;set;}

            [Required(ErrorMessage=" Last Name cannot be Empty!!")]
            [MinLength(2,ErrorMessage="Last Name cannot be less than 2 characters!!")]
        public string LastName {get;set;}

            [Required(ErrorMessage="Email cannot be empty!!")]
            [EmailAddress]
        public string Email {get;set;}

            [Required]
            [DataType(DataType.Password)]
            [MinLength(2, ErrorMessage="Password cannot be less than 2 characters!!")]
        public string Password {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

            [NotMapped]
            [Compare("Password", ErrorMessage="Password and confirm password did not match!!")]
            [DataType(DataType.Password)]
        public string ConfirmPassword {get;set;}

        public List<WeddingSchedule> weddingSchedule {get;set;}
        public List<Guest> guest {get;set;}
    }
}