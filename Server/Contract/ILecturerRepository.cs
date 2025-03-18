using Server.Data;
using Server.Models.LecturerDtos;

namespace Server.Contract
{
    public interface ILecturerRepository : IGenericRepository<Lecturer>
    {

        Task<List<CourselecturerDto>> GetlecturerByIdAsync(int lecturerId);
    }
}
