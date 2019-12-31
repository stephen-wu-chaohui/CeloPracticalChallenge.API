using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CeloPracticalChallenge.API.Entities
{
    public class RandomUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        [MaxLength(256)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(256)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Url]
        public string ThrumbnailURL { get; set; }

        [Required]
        [Url]
        public string LargeImageURL { get; set; }
    }
}
