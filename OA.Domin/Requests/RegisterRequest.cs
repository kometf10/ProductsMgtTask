using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Domain.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Required field")]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Phone(ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password Confirmation Missmatch")]
        public string ConfirmPassword { get; set; }

    }
}
