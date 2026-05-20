using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using app.Common;
using app.Dtos.Post;
using app.Dtos.User;
using app.Interfaces;
using app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers.Post
{
    [Route("api/post")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepo;
        private readonly FileService _fileService;

        public PostsController(IPostRepository postRepo, FileService fileService)
        {
            _postRepo = postRepo;
            _fileService = fileService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            var post = await _postRepo.GetByIdAsync(id);
            if (post == null) return NotFound(ApiResponse<string>.Fail("Post not found."));

            return Ok(ApiResponse<PostResponseDto>.Ok(MapPost(post)));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequestDto dto)
        {
            string? imageUrl = null;

            if (dto.Image != null)
            {
                imageUrl = await _fileService.SaveImageAsync(dto.Image);
                if (imageUrl == null)
                    return BadRequest(ApiResponse<string>.Fail("Invalid image format. Allowed: jpg, jpeg, png, webp."));
            }

            var post = new app.Models.Post
            {
                Content = dto.Content,
                ImageUrl = imageUrl,
                UserId = CurrentUserId
            };

            var created = await _postRepo.CreateAsync(post);
            var full = await _postRepo.GetByIdAsync(created.Id);
            return CreatedAtAction(nameof(GetPost), new { id = created.Id },
                ApiResponse<PostResponseDto>.Ok(MapPost(full!), "Post created."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, UpdatePostRequestDto dto)
        {
            var post = await _postRepo.GetByIdAsync(id);
            if (post == null) return NotFound(ApiResponse<string>.Fail("Post not found."));
            if (post.UserId != CurrentUserId) return Forbid();

            post.Content = dto.Content;
            post.ImageUrl = dto.ImageUrl ?? post.ImageUrl;
            post.UpdatedAt = DateTime.UtcNow;

            var updated = await _postRepo.UpdateAsync(post);
            return Ok(ApiResponse<PostResponseDto>.Ok(MapPost(updated), "Post updated."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _postRepo.DeleteAsync(id, CurrentUserId);
            if (!result) return NotFound(ApiResponse<string>.Fail("Post not found or unauthorized."));
            return Ok(ApiResponse<string>.Ok("Deleted.", "Post deleted."));
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> Like(Guid id)
        {
            var result = await _postRepo.LikeAsync(id, CurrentUserId);
            if (!result) return BadRequest(ApiResponse<string>.Fail("Already liked."));
            return Ok(ApiResponse<string>.Ok("Liked.", "Post liked."));
        }

        [HttpDelete("{id}/like")]
        public async Task<IActionResult> Unlike(Guid id)
        {
            var result = await _postRepo.UnlikeAsync(id, CurrentUserId);
            if (!result) return NotFound(ApiResponse<string>.Fail("Like not found."));
            return Ok(ApiResponse<string>.Ok("Unliked.", "Post unliked."));
        }

        private PostResponseDto MapPost(app.Models.Post post) => new()
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            LikesCount = post.Likes.Count,
            CommentsCount = post.Comments.Count,
            IsLikedByCurrentUser = post.Likes.Any(l => l.UserId == CurrentUserId),
            Author = new UserResponseDto
            {
                Id = post.User.Id,
                Username = post.User.Username,
                AvatarUrl = post.User.AvatarUrl,
                CreatedAt = post.User.CreatedAt
            },
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}