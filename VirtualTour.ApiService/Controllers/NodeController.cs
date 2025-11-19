using VirtualTour.BL.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Mvc;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodeController : ControllerBase
    {
        private readonly INodeService _nodeService;
        public NodeController(INodeService nodeService)
        {
            _nodeService = nodeService;
        }
        [HttpGet("getListUse")]
        public async Task<IActionResult> GetAllNodesAsync()
        {
            try
            {
                var nodes = await _nodeService.GetAllNodesAsync();
                return Ok(new BaseResponseModel { Success = true, Data = nodes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getLinks/{nodeId}")]
        public async Task<IActionResult> GetAllLinksAsync(int nodeId)
        {
            try
            {
                var links = await _nodeService.GetAllLinksAsync(nodeId);
                return Ok(new BaseResponseModel { Success = true, Data = links });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateNodeAsync([FromBody] NodeModel nodeModel)
        {
            if (nodeModel == null)
            {
                return BadRequest("Node model is null");
            }
            try
            {
                await _nodeService.CreateNodeAsync(nodeModel);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getMaxId")]
        public async Task<IActionResult> GetMaxIdAsync()
        {
            try
            {
                var maxId = await _nodeService.GetMaxIdAsync();
                if (maxId == null)
                {
                    maxId = 0;
                }
                return Ok(new BaseResponseModel { Success = true, Data = maxId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpPost("createLink/{startNodeId}")]
        public async Task<IActionResult> CreateNodeLinkAsync([FromBody] LinkedNodes linkedNode, int startNodeId)
        {
            if (linkedNode == null)
            {
                return BadRequest("Linked node model is null");
            }
            try
            {
                await _nodeService.CreateNodeLink(linkedNode, startNodeId);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getElementById/{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var node = await _nodeService.GetByIdAsync(id);
                if (node == null)
                {
                    return NotFound(new BaseResponseModel { Success = false, ErrorMessage = "Node not found" });
                }
                return Ok(new BaseResponseModel { Success = true, Data = node });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteNodeAsync(int id)
        {
            try
            {
                await _nodeService.DeleteNodeAsync(id);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateNodeAsync([FromBody] NodeModel nodeModel)
        {
            if (nodeModel == null)
            {
                return BadRequest("Node model is null");
            }
            try
            {
                await _nodeService.UpdateNodeAsync(nodeModel);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpPost("setStartNode/{currentNodeId}")]
        public async Task<IActionResult> SetStartNode(int currentNodeId, [FromBody] int previousNodeId)
        {
            try
            {
                await _nodeService.SetStartNode(currentNodeId, previousNodeId);
                return Ok(new BaseResponseModel { Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getStartId")]
        public async Task<IActionResult> GetStartIdAsync()
        {
            try
            {
                var startId = await _nodeService.GetStartIdAsync();
                return Ok(new BaseResponseModel { Success = true, Data = startId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getAllArea")]
        public async Task<IActionResult> GetAllAreaAsync()
        {
            try
            {
                var areas = await _nodeService.GetAllAreaAsync();
                return Ok(new BaseResponseModel { Success = true, Data = areas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getAllDept")]
        public async Task<IActionResult> GetAllDeptAsync()
        {
            try
            {
                var depts = await _nodeService.GetAllDeptAsync();
                return Ok(new BaseResponseModel { Success = true, Data = depts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpGet("getListPublic/{tenantId}")]
        public async Task<IActionResult> GetAllNodesPublicAsync(string tenantId)
        {
            try
            {
                var nodes = await _nodeService.GetAllNodesPublicAsync(tenantId);
                return Ok(new BaseResponseModel { Success = true, Data = nodes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
