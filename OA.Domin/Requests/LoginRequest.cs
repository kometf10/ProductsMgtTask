using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Domain.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Required field")]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required field")]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
