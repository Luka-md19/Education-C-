using Server.Contract;
using Server.Data;
using System.Threading.Tasks;

namespace Server.Repositories
{
    public interface IUserDeviceRepository : IGenericRepository<UserDevice>
    {
        // Corrected method declaration without 'async' keyword
        Task<DeviceUpdateResult> AddOrUpdateUserDeviceAsync(string userId, string deviceIdentifier, string ipAddress);

        Task<int> GetUserDeviceCountAsync(string userId);
    }
}
