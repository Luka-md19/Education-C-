using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Data;

namespace Server.Data.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.HasKey(c => c.ContentId); // Primary Key

            builder.Property(c => c.ContentType)
                   .IsRequired(); // Assuming ContentType is required

            builder.Property(c => c.ContentTitle)
                   .IsRequired() // Assuming ContentTitle is required
                   .HasMaxLength(255); // And has a max length

            builder.Property(c => c.ContentUrl)
                   .IsRequired(false); // ContentUrl might be optional

            builder.Property(c => c.ContentOrder)
                   .IsRequired(); // Assuming ContentOrder is required

            builder.HasIndex(ch => new { ch.ContentId, ch.ContentOrder })
              .IsUnique();

            // Relationship with Chapter is configured here if necessary
            builder.HasOne(c => c.Chapter) // Each Content belongs to one Chapter
                   .WithMany(chapter => chapter.Contents) // Each Chapter can have many Contents
                   .HasForeignKey(c => c.ChapterId); // ForeignKey in Content pointing to Chapter

            // Configuring the one-to-many relationship with CommunityFeed
            builder.HasMany(c => c.CommunityFeeds) // Each Content can have many CommunityFeeds
                   .WithOne(cf => cf.Content) // Each CommunityFeed is related to one Content
                   .HasForeignKey(cf => cf.ContentId); // ForeignKey in CommunityFeed pointing to Content
        }
    }
}
