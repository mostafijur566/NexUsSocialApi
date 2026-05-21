using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using app.Common;
using app.Dtos.Comment;
using app.Dtos.User;
using app.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers.Comment
{
    [ApiController]
    [Route("api/posts/{postId}/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        public CommentsController(ICommentRepository commentRepo) => _commentRepo = commentRepo;

        private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetComments(Guid postId)
        {
            var comments = await _commentRepo.GetByPostIdAsync(postId);
            var result = comments.Select(MapComment).ToList();
            return Ok(ApiResponse<List<CommentResponseDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Guid postId, CreateCommentRequestDto dto)
        {
            var comment = new app.Models.Comment
            {
                Content = dto.Content,
                PostId = postId,
                UserId = CurrentUserId,
                ParentCommentId = dto.ParentCommentId
            };
            var created = await _commentRepo.CreateAsync(comment);
            var full = await _commentRepo.GetByIdAsync(created.Id);
            return Ok(ApiResponse<CommentResponseDto>.Ok(MapComment(full!), "Comment added."));
        }

        [HttpDelete("/api/comments/{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _commentRepo.DeleteAsync(id, CurrentUserId);
            if (!result) return NotFound(ApiResponse<string>.Fail("Comment not found or unauthorized."));
            return Ok(ApiResponse<string>.Ok("Deleted.", "Comment deleted."));
        }

        private CommentResponseDto MapComment(app.Models.Comment c) => new()
        {
            Id = c.Id,
            Content = c.Content,
            ParentCommentId = c.ParentCommentId,
            Author = new UserResponseDto { Id = c.User.Id, Username = c.User.Username, AvatarUrl = c.User.AvatarUrl, CreatedAt = c.User.CreatedAt },
            Replies = c.Replies.Select(r => new CommentResponseDto
            {
                Id = r.Id,
                Content = r.Content,
                ParentCommentId = r.ParentCommentId,
                Author = new UserResponseDto { Id = r.User.Id, Username = r.User.Username, AvatarUrl = r.User.AvatarUrl, CreatedAt = r.User.CreatedAt },
                CreatedAt = r.CreatedAt
            }).ToList(),
            CreatedAt = c.CreatedAt
        };
    }
}