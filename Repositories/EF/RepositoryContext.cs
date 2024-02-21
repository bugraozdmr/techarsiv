using System.Reflection;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.Config;

namespace Repositories.EF;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options) : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfiguration(new IdentityRoleConfig());
        //modelBuilder.ApplyConfiguration(new CategoryConfig());

        // ikiside primary anahtarlar alırlar
        modelBuilder.Entity<SubjectDislike>()
            .HasKey(sl => new { sl.SubjectId, sl.UserId });

        modelBuilder.Entity<SubjectLike>()
            .HasKey(sl => new { sl.SubjectId, sl.UserId });
        
        
        modelBuilder.Entity<SubjectHeart>()
            .HasKey(sl => new { sl.SubjectId, sl.UserId });
        
        modelBuilder.Entity<Commentlike>()
            .HasKey(sl => new { sl.CommentId, sl.UserId });
        
        modelBuilder.Entity<CommentDislike>()
            .HasKey(sl => new { sl.CommentId, sl.UserId });
        

        
        // bulur
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<SubjectLike> SubjectLikes { get; set; }
    public DbSet<SubjectDislike> SubjectDislikes { get; set; }
    public DbSet<SubjectHeart> SubjectHearts { get; set; }
    
    public DbSet<Commentlike> Commentlikes { get; set; }
    public DbSet<CommentDislike> CommentDislikes { get; set; }
    
}