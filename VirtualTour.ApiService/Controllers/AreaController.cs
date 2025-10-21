using Microsoft.AspNetCore.Mvc;
using VirtualTour.BL.Repositories;
using VirtualTour.Model;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/area")]
    public class AreaController: ControllerBase
    {
        private readonly IAreaRepository _areaRepository;
        public AreaController(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAllAreas()
        {
            var areas = await _areaRepository.GetAllAreasAsync();
            return Ok(new BaseResponseModel { Success = true, Data = areas });
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            var area = await _areaRepository.GetAreaByIdAsync(id);
            return Ok(new BaseResponseModel { Success = true, Data = area });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateArea([FromBody] AreaModel area)
        {
            if (area == null || string.IsNullOrEmpty(area.AreaName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid area data" });
            }
            await _areaRepository.CreateAreaAsync(area);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateArea([FromBody] AreaModel area)
        {
            if (area == null || string.IsNullOrEmpty(area.AreaName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid area data" });
            }
            await _areaRepository.UpdateAreaAsync(area);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            await _areaRepository.DeleteAreaAsync(id);
            return Ok(new BaseResponseModel { Success = true });
        }
    }
}
