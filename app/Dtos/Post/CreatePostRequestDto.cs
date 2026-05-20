using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace app.Dtos.Post
{
    public class CreatePostRequestDto
    {
        [Required]
        [MaxLength(500, ErrorMessage = "Content must be between 1 and 500 characters.")]
        [MinLength(1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }
}