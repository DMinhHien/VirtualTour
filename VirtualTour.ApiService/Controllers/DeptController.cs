using Microsoft.AspNetCore.Mvc;
using VirtualTour.BL.Services;
using VirtualTour.Model;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/dept")]
    public class DeptController: ControllerBase
    {
        private readonly IDeptService _deptService;
        public DeptController(IDeptService deptService)
        {
            _deptService = deptService;
        }
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAllDepts()
        {
            var depts = await _deptService.GetAllDeptsAsync();
            return Ok(new BaseResponseModel { Success = true, Data = depts });
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetDeptById(int id)
        {
            var dept = await _deptService.GetDeptByIdAsync(id);
            return Ok(new BaseResponseModel { Success = true, Data = dept });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateDept([FromBody] DeptModel dept)
        {
            if (dept == null || string.IsNullOrEmpty(dept.DeptName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid dept data" });
            }
            await _deptService.CreateDeptAsync(dept);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDept([FromBody] DeptModel dept)
        {
            if (dept == null || string.IsNullOrEmpty(dept.DeptName))
            {
                return BadRequest(new BaseResponseModel { Success = false, ErrorMessage = "Invalid dept data" });
            }
            await _deptService.UpdateDeptAsync(dept);
            return Ok(new BaseResponseModel { Success = true });
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDept(int id)
        {
            await _deptService.DeleteDeptAsync(id);
            return Ok(new BaseResponseModel { Success = true });
        }
    }
}
