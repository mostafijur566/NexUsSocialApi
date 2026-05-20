using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Dtos.Post
{
    public class UpdatePostRequestDto
    {
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}