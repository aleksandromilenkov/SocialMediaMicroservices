using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;
        public PostRepository(IDbContextFactory<DatabaseContext> factory)
        {
            _factory = factory;
        }
        public async Task CreateAsync(PostEntity post)
        {
            await using var context = _factory.CreateDbContext();
            context.Posts.Add(post);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId)
        {
            await using var context = _factory.CreateDbContext();
            var postToRemove = await context.Posts.FindAsync(postId);
            if (postToRemove == null) throw new Exception("Post does not exists");
            context.Posts.Remove(postToRemove);
            await context.SaveChangesAsync();
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            await using var context = _factory.CreateDbContext();
            return await context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<List<PostEntity>> ListAllAsync()
        {
            await using var context = _factory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().ToListAsync();
        }

        public async Task<List<PostEntity>> ListByAuthorAsync(string author)
        {
            await using var context = _factory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().Where(p => p.Author.Contains(author)).ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithCommentsAsync()
        {
            await using var context = _factory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().Where(p => p.Comments != null && p.Comments.Any()).ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
        {
            await using var context = _factory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).AsNoTracking().Where(p => p.Likes >= numberOfLikes).ToListAsync();
        }

        public async Task UpdateAsync(PostEntity post)
        {
            await using var context = _factory.CreateDbContext();
            context.Posts.Update(post);
            await context.SaveChangesAsync();
        }
    }
}
