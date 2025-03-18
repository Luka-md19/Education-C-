using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.API.Exceptions;
using Server.Contract;
using Server.Data;
using Server.Models.UserDeviceDtos;
using Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDevicesController : ControllerBase
    {
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserDevicesController> _logger;

        public UserDevicesController(IUserDeviceRepository userDeviceRepository, IMapper mapper, ILogger<UserDevicesController> logger)
        {
            _userDeviceRepository = userDeviceRepository;
            _mapper = mapper;
            _logger = logger;
        }
        // GET: api/UserDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDeviceDto>>> GetUserDevices()
        {
            var userDevices = await _userDeviceRepository.GetAllAsync<GetUserDeviceDto>();
            return Ok(userDevices);
        }

        // GET: api/UserDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDeviceDto>> GetUserDevice(int id)
        {
            var userDeviceDto = await _userDeviceRepository.GetAsync<GetUserDeviceDto>(id);
            if (userDeviceDto == null)
            {
                return NotFound();
            }

            return Ok(userDeviceDto);
        }

        // PUT: api/UserDevices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDevice(int id, UpdateUserDevice userDeviceDto)
        {
            if (id != userDeviceDto.Id)
            {
                return BadRequest();
            }

            try
            {
                await _userDeviceRepository.UpdateAsync<UpdateUserDevice>(id, userDeviceDto);
            }
            catch (NotFoundException)
            {
                if (!await _userDeviceRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        // POST: api/UserDevices/AddOrUpdate
        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateUserDevice(CreateUserDeviceDto createUserDeviceDto)
        {
            // Map DTO to UserDevice entity
            var userDevice = _mapper.Map<UserDevice>(createUserDeviceDto);

            // Extract IP address from CreateUserDeviceDto or from HttpContext
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            // Attempt to add or update the user device
            var result = await _userDeviceRepository.AddOrUpdateUserDeviceAsync(userDevice.UserId, userDevice.DeviceIdentifier, ipAddress);

            switch (result)
            {
                case DeviceUpdateResult.Success:
                    return Ok("User device added or updated successfully.");

                case DeviceUpdateResult.LimitReached:
                    return BadRequest("You cannot register more than two devices. Please manage your devices.");

                case DeviceUpdateResult.Error:
                    // Handle any other kind of failure that might occur
                    return StatusCode(StatusCodes.Status500InternalServerError);

                default:
                    // In case there are other enum values that are not handled above
                    _logger.LogError("Unexpected DeviceUpdateResult value: {Result}", result);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        // GET: api/UserDevices/Count/{userId}
        [HttpGet("Count/{userId}")]
        public async Task<ActionResult<int>> GetUserDeviceCount(string userId)
        {
            int count = await _userDeviceRepository.GetUserDeviceCountAsync(userId);
            return Ok(count);
        }

        // POST: api/UserDevices
        [HttpPost]
        public async Task<ActionResult<UserDevice>> PostUserDevice(CreateUserDeviceDto createUserDeviceDto)
        {
            var userDeviceDto = await _userDeviceRepository.AddAsync<CreateUserDeviceDto, UserDeviceDto>(createUserDeviceDto);
            return CreatedAtAction("GetUserDevice", new { id = userDeviceDto.Id }, userDeviceDto);
        }

        // DELETE: api/UserDevices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDevice(int id)
        {
            await _userDeviceRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
