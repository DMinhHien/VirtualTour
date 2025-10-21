using Microsoft.AspNetCore.Mvc;
using VirtualTour.BL.Services;
using VirtualTour.Model;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/floor")]
    public class FloorController: ControllerBase
    {
        private readonly IFloorService _floorService;
        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAllFloors()
        {
            var floors = await _floorService.GetAllFloorsAsync();
            return Ok(new BaseResponseModel { Success = true, Data = floors });
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetFloorById(int id)
        {
            var floor = await _floorService.GetFloorByIdAsync(id);
            return Ok(new BaseResponseModel { Success = true, Data = floor });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateFloor([FromBody] FloorModel floor)
        {
            if (floor == null || string.IsNullOrEmpty(floor.FloorName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid floor data" });
            }
            await _floorService.CreateFloorAsync(floor);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateFloor([FromBody] FloorModel floor)
        {
            if (floor == null || string.IsNullOrEmpty(floor.FloorName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid floor data" });
            }
            await _floorService.UpdateFloorAsync(floor);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFloor(int id)
        {
            await _floorService.DeleteFloorAsync(id);
            return Ok(new BaseResponseModel { Success = true });
        }
    }
}
