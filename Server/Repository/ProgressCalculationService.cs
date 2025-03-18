using Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class ProgressCalculationService : IProgressCalculationService
{
    private readonly ServerDbContext _context;

    public ProgressCalculationService(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<double> CalculateProgressPercentageAsync(Course course, string userId)
    {
        int totalContents = course.Chapters.Sum(ch => ch.Contents.Count);
        int completedContents = await CalculateCompletedContentsAsync(course, userId);
        return (totalContents > 0) ? (double)completedContents / totalContents * 100 : 0;
    }

    private async Task<int> CalculateCompletedContentsAsync(Course course, string userId)
    {
        return await _context.UserContentCompletions
                             .CountAsync(uc => uc.UserId == userId
                                               && uc.IsCompleted
                                               && course.Chapters.SelectMany(ch => ch.Contents)
                                                                 .Select(c => c.ContentId)
                                                                 .Contains(uc.ContentId));
    }
}
