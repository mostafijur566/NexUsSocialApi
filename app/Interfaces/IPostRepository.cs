using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Models;

namespace app.Interfaces
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        Task<(List<Post> Posts, int TotalCount)> GetFeedAsync(Guid userId, int page, int pageSize);
        Task<(List<Post> Posts, int TotalCount)> SearchAsync(string query, int page, int pageSize);
        Task<Post> CreateAsync(Post post);
        Task<Post> UpdateAsync(Post post);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<bool> LikeAsync(Guid postId, Guid userId);
        Task<bool> UnlikeAsync(Guid postId, Guid userId);
        Task<bool> IsLikedByUserAsync(Guid postId, Guid userId);
    }
}