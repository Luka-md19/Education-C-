using Server.Data;

public interface IProgressCalculationService
{
    Task<double> CalculateProgressPercentageAsync(Course course, string userId);
}