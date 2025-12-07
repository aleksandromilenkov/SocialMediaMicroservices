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
    public class CommentRepository : ICommentRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _factory;
        public CommentRepository(IDbContextFactory<DatabaseContext> factory)
        {
            _factory = factory;
        }
        public async Task CreateAsync(CommentEntity comment)
        {
            await using var context = _factory.CreateDbContext();
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid commentId)
        {
            await using var context = _factory.CreateDbContext();
            var commentToRemove = await GetByIdAsync(commentId);
            if (commentToRemove == null) return;
            context.Comments.Remove(commentToRemove);
            await context.SaveChangesAsync();
        }

        public async Task<CommentEntity> GetByIdAsync(Guid commentId)
        {
            await using var context = _factory.CreateDbContext();
            return await context.Comments.FindAsync(commentId);
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            await using var context = _factory.CreateDbContext();
            context.Comments.Update(comment);
            await context.SaveChangesAsync();
        }
    }
}
