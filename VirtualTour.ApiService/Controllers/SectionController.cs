using Microsoft.AspNetCore.Mvc;
using VirtualTour.BL.Services;
using VirtualTour.Model;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/section")]
    public class SectionController: ControllerBase
    {
        private readonly ISectionService _sectionService;
        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAllSections()
        {
            var sections = await _sectionService.GetAllSectionsAsync();
            return Ok(new BaseResponseModel { Success = true, Data = sections });
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetSectionById(int id)
        {
            var section = await _sectionService.GetSectionByIdAsync(id);
            return Ok(new BaseResponseModel { Success = true, Data = section });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateSection([FromBody] SectionModel section)
        {
            if (section == null || string.IsNullOrEmpty(section.SectName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid section data" });
            }
            await _sectionService.CreateSectionAsync(section);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateSection([FromBody] SectionModel section)
        {
            if (section == null || string.IsNullOrEmpty(section.SectName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid section data" });
            }
            await _sectionService.UpdateSectionAsync(section);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            await _sectionService.DeleteSectionAsync(id);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpGet("getListPublic/{tenantId}")]
        public async Task<IActionResult> GetAllSectionsPublic(string tenantId)
        {
            var sections = await _sectionService.GetAllSectionsPublicAsync(tenantId);
            return Ok(new BaseResponseModel { Success = true, Data = sections });
        }

    }
}
