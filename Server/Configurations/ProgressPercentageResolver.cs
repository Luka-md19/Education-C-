using AutoMapper;
using Server.Data;
using Server.Models.CourseDtos;

public class ProgressPercentageResolver : IMemberValueResolver<Course, CourseProgressDto, string, double>
{
    private readonly IProgressCalculationService _progressCalculationService;

    public ProgressPercentageResolver(IProgressCalculationService progressCalculationService)
    {
        _progressCalculationService = progressCalculationService;
    }

    public double Resolve(Course source, CourseProgressDto destination, string sourceMember, double destMember, ResolutionContext context)
    {
        // Assuming sourceMember is the CourseId in string format
        var courseId = int.Parse(sourceMember);
        var userId = context.Items["userId"] as string;
        return _progressCalculationService.CalculateProgressPercentageAsync(source, userId).Result;
    }
}
