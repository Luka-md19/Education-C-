using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Data.Configuartions;
using Server.Data.Configurations; // Contains CourseConfiguraion, CommunityFeedConfiguration, RoleConfiguration
using Server.Identity; // Contains ApiUser class

namespace Server.Data
{
    public class ServerDbContext : IdentityDbContext<ApiUser>
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CommunityFeed> CommunityFeeds { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CourseDepartment> CourseDepartments { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<UserContentCompletion> UserContentCompletions { get; set; }
        public DbSet<Answer> Answers { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin<string>>()
       .HasKey(login => new { login.LoginProvider, login.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(iur => new { iur.UserId, iur.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>()
    .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
            modelBuilder.ApplyConfiguration(new CourseConfiguraion());
            modelBuilder.ApplyConfiguration(new CommunityFeedConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new ChapterConfiguration());
            modelBuilder.ApplyConfiguration(new ContentConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new CourseDepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new DetailConfiguration());
            modelBuilder.ApplyConfiguration(new LecturerConfiguration());
     

        }
    }
}
