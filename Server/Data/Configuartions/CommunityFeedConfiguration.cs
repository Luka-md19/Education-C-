using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Server.Data.Configurations
{
    public class CommunityFeedConfiguration : IEntityTypeConfiguration<CommunityFeed>
    {
        public void Configure(EntityTypeBuilder<CommunityFeed> builder)
        {
            builder.HasKey(cf => cf.PostId); // Primary Key

            builder.Property(cf => cf.Question)
                   .IsRequired(false)
                   .HasMaxLength(500); // Optional with max length

            builder.Property(cf => cf.PostedDate)
                   .IsRequired(); // Required

            builder.Property(cf => cf.Upvotes)
                   .IsRequired(); // Required

            // Establishing the relationship to Content
            builder.HasOne(cf => cf.Content) // Each CommunityFeed has one Content
                   .WithMany(content => content.CommunityFeeds) // Each Content has many CommunityFeed entries
                   .HasForeignKey(cf => cf.ContentId); // Foreign key in CommunityFeed

            // If you intend to have Answers navigation property configured
            builder.HasMany(cf => cf.Answers) // Each CommunityFeed (question) can have many answers
                   .WithOne(answer => answer.CommunityFeed) // Each Answer belongs to one CommunityFeed
                   .HasForeignKey(answer => answer.PostId); // Foreign key in Answer
        }
    }
}
