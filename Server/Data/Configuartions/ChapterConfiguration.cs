using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Server.Data;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasKey(ch => ch.ChapterId); 

        builder.Property(ch => ch.ChapterName)
               .IsRequired()
               .HasMaxLength(100); 

       
        builder.Property(ch => ch.ChapterOrder).IsRequired();

        builder.HasOne(ch => ch.Course)
               .WithMany(c => c.Chapters) 
               .HasForeignKey(ch => ch.CourseId)
               .IsRequired(); 

        builder.HasIndex(ch => new { ch.CourseId, ch.ChapterOrder })
               .IsUnique(); 


        builder.HasMany(ch => ch.Contents)
               .WithOne(c => c.Chapter)
               .HasForeignKey(c => c.ChapterId);
    }
}
