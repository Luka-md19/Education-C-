using Server.Data;
using Server.Models.FacultyDtos;

namespace Server.Contract
{
    public interface IFacultyRepository : IGenericRepository<Faculty>
    {
        Task<FacultyDetailsDto> FacultyDetailsIdAsync(int FacultyId);
    }
}
