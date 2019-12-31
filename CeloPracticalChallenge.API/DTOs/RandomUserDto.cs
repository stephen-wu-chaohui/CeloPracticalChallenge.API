
using System;
using System.ComponentModel.DataAnnotations;

namespace CeloPracticalChallenge.API.DTOs
{
    public class RandomUserDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        public NameDto Name { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Url]
        public string ProfileImage { get; set; }
    }
}