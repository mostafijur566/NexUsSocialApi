using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.Interfaces;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context) => _context = context;

        public async Task<Post?> GetByIdAsync(Guid id) =>
            await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<(List<Post>, int)> GetFeedAsync(Guid userId, int page, int pageSize)
        {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowingId).ToListAsync();

            var query = _context.Posts
                .Where(p => followingIds.Contains(p.UserId))
                .Include(p => p.User).Include(p => p.Likes).Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();
            var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (posts, total);
        }

        public async Task<(List<Post>, int)> SearchAsync(string query, int page, int pageSize)
        {
            var q = _context.Posts
                .Where(p => p.Content.Contains(query))
                .Include(p => p.User).Include(p => p.Likes).Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt);

            var total = await q.CountAsync();
            var posts = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (posts, total);
        }

        public async Task<Post> CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (post == null) return false;
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikeAsync(Guid postId, Guid userId)
        {
            if (await IsLikedByUserAsync(postId, userId)) return false;
            _context.Likes.Add(new Like { PostId = postId, UserId = userId });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlikeAsync(Guid postId, Guid userId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
            if (like == null) return false;
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsLikedByUserAsync(Guid postId, Guid userId) =>
            await _context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId);
    }
}