using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace app.Dtos.Comment
{
    public class CreateCommentRequestDto
    {
        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
    }
}