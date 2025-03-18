using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Data;

namespace Server.Data.Configurations
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(a => a.AnswerId); 

            builder.Property(a => a.Content)
                   .IsRequired(); 

            builder.Property(a => a.PostedDate)
                   .IsRequired(); 

            builder.HasOne(a => a.CommunityFeed) 
                   .WithMany() 
                   .HasForeignKey(a => a.PostId); 

            builder.Property(a => a.Upvotes)
                   .IsRequired(); 

           
        }
    }
}
