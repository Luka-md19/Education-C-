using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Repositories;
using Server.Contract; // Ensure you have this using directive to access the DeviceUpdateResult enum
using Server.Repository;

public class UserDeviceRepository : GenericRepository<UserDevice>, IUserDeviceRepository
{
    private readonly ServerDbContext _context;
    private readonly IMapper _mapper;

    public UserDeviceRepository(ServerDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DeviceUpdateResult> AddOrUpdateUserDeviceAsync(string userId, string deviceIdentifier, string ipAddress)
    {
        var userDevices = await _context.UserDevices.Where(ud => ud.UserId == userId).ToListAsync();
        var existingDevice = userDevices.FirstOrDefault(ud => ud.DeviceIdentifier == deviceIdentifier);

        if (existingDevice != null)
        {
            existingDevice.LastAccessTime = DateTime.UtcNow;
            existingDevice.IPAddress = ipAddress;
            _context.UserDevices.Update(existingDevice);
        }
        else if (userDevices.Count < 2)
        {
            var userDevice = new UserDevice
            {
                UserId = userId,
                DeviceIdentifier = deviceIdentifier,
                IPAddress = ipAddress,
                LastAccessTime = DateTime.UtcNow
            };
            await _context.UserDevices.AddAsync(userDevice);
        }
        else
        {
            // User has reached the limit of registered devices
            return DeviceUpdateResult.LimitReached;
        }

        await _context.SaveChangesAsync();
        return DeviceUpdateResult.Success;
    }

    public async Task<int> GetUserDeviceCountAsync(string userId)
    {
        return await _context.UserDevices.CountAsync(ud => ud.UserId == userId);
    }
}
