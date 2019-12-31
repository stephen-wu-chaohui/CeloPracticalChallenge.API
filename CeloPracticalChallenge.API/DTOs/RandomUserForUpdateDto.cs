using System;
using System.ComponentModel.DataAnnotations;

namespace CeloPracticalChallenge.API.DTOs
{
    public class RandomUserForUpdateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public NameDto Name { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
