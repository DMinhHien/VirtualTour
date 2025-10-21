using VirtualTour.BL.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualTour.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QRCodesController : ControllerBase
    {
        private readonly IQRCodeService _qRCodeService;
        public QRCodesController(IQRCodeService qRCodeService)
        {
            _qRCodeService = qRCodeService;
        }
        [Authorize(Policy = "Permission:QRCode.Manage")]
        [HttpGet]        
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var wifiNetworks = await _qRCodeService.GetAllWifiNetworksAsync();
                return Ok(new BaseResponseModel<IEnumerable<QRCodeModel>>
                {
                    Success = true,
                    Data = wifiNetworks
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel<IEnumerable<QRCodeModel>>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var wifiNetwork = await _qRCodeService.GetWifiNetworkByIdAsync(id);
                if (wifiNetwork == null)
                {
                    return NotFound(new BaseResponseModel<object>
                    {
                        Success = false,
                        ErrorMessage = "Wifi network not found."
                    });
                }

                return Ok(new BaseResponseModel<QRCodeModel>
                {
                    Success = true,
                    Data = wifiNetwork
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel<object>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }


        [Authorize(Policy = "Permission:QRCode.Create")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] QRCodeCreateRequest wifiNetwork)
        {
            try
            {
                if (wifiNetwork == null)
                {
                    return BadRequest(new BaseResponseModel<object>
                    {
                        Success = false,
                        ErrorMessage = "Invalid wifi network data."
                    });
                }

                var createdModel = await _qRCodeService.CreateWifiAsync(wifiNetwork);

                return Ok(new BaseResponseModel<QRCodeCreateRequest>
                {
                    Success = true,
                    Data = createdModel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel<object>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }


        [Authorize(Policy = "Permission:QRCode.Edit")]
        [HttpPut("update")]    
        public async Task<IActionResult> Update([FromBody] QRCodeUpdateRequest wifiNetwork)
        {
            try
            {
                if (wifiNetwork == null)
                {
                    return BadRequest(new BaseResponseModel<object>
                    {
                        Success = false,
                        ErrorMessage = "Invalid wifi network data."
                    });
                }

                await _qRCodeService.UpdateWifiAsync(wifiNetwork);
                return Ok(new BaseResponseModel<object>
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel<object>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }


        [Authorize(Policy = "Permission:QRCode.Delete")]

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _qRCodeService.DeleteWifiAsync(id);
                return Ok(new BaseResponseModel<object>
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel<object>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }
        [HttpGet("getBySsid/{ssid}")]
        public async Task<IActionResult> GetWifiNetworkBySsid(string ssid)
        {
            try
            {
                var wifiNetwork = await _qRCodeService.GetWifiNetworkBySsid(ssid);
                if (wifiNetwork == null)
                {
                    return NotFound(new BaseResponseModel<object>
                    {
                        Success = false,
                        ErrorMessage = "Wifi network not found."
                    });
                }

                return Ok(new BaseResponseModel<QRCodeModel>
                {
                    Success = true,
                    Data = wifiNetwork
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponseModel<object>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
