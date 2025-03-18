using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Server.Data.Configurations
{
    public class UserCourseConfiguration : IEntityTypeConfiguration<UserCourse>
    {
        public void Configure(EntityTypeBuilder<UserCourse> builder)
        {
            builder.HasData(
                new UserCourse { UserCourseId = 1, UserId = "480df7a4-cd08-4c0c-96f4-f847981e1a88", CourseId = 17 },
                new UserCourse { UserCourseId = 2, UserId = "480df7a4-cd08-4c0c-96f4-f847981e1a88", CourseId = 18 },
                new UserCourse { UserCourseId = 3, UserId = "480df7a4-cd08-4c0c-96f4-f847981e1a88", CourseId = 19 },
                new UserCourse { UserCourseId = 4, UserId = "480df7a4-cd08-4c0c-96f4-f847981e1a88", CourseId = 20 }
              
            );
        }
    }
}
