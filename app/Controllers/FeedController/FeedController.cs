using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using app.Common;
using app.Dtos.Post;
using app.Dtos.User;
using app.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers.FeedController
{
    [Route("api/feed")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly IPostRepository _postRepo;
        public FeedController(IPostRepository postRepo) => _postRepo = postRepo;

       private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (posts, total) = await _postRepo.GetFeedAsync(CurrentUserId, page, pageSize);
            var result = posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                LikesCount = p.Likes.Count,
                CommentsCount = p.Comments.Count,
                IsLikedByCurrentUser = p.Likes.Any(l => l.UserId == CurrentUserId),
                Author = new UserResponseDto { Id = p.User.Id, Username = p.User.Username, AvatarUrl = p.User.AvatarUrl, CreatedAt = p.User.CreatedAt },
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return Ok(PagedResponse<PostResponseDto>.Ok(result, page, pageSize, total));
        }
    }
}