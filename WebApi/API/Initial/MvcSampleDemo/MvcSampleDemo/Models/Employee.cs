using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSampleDemo.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Display(Name ="First Name of the Employee")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")] 
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")] 
        public string EmailAddress { get; set; }
        public int Age { get; set; }

    }
}
