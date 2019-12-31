using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CeloPracticalChallenge.API.DTOs
{
    public class NameDto
    {
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        
        [Required]
        [MaxLength(256)]
        public string First { get; set; }

        [Required]
        [MaxLength(256)]
        public string Last { get; set; }
    }
}