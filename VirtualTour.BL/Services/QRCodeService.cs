using VirtualTour.BL.Repositories;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Services
{
    

    public interface IQRCodeService
    {
        Task<List<QRCodeModel>> GetAllWifiNetworksAsync();
        Task<QRCodeModel> GetWifiNetworkByIdAsync(int id);
        Task<QRCodeModel> GetWifiNetworkBySsid(string Ssid);
        Task<QRCodeCreateRequest> CreateWifiAsync(QRCodeCreateRequest wifiNetwork);
        Task UpdateWifiAsync(QRCodeUpdateRequest wifiNetwork);
        Task DeleteWifiAsync(int id);
    }
    public class QRCodeService : IQRCodeService
    {
        private readonly IQRCodeRepository _wifiRepository;
        public QRCodeService(IQRCodeRepository wifiRepository)
        {
            _wifiRepository = wifiRepository;
        }
        public async Task<List<QRCodeModel>> GetAllWifiNetworksAsync()
        {
            return await _wifiRepository.GetAllWifiNetworksAsync();
        }
        public async Task<QRCodeModel> GetWifiNetworkByIdAsync(int id)
        {
            return await _wifiRepository.GetWifiNetworkByIdAsync(id);
        }
        public async Task<QRCodeCreateRequest> CreateWifiAsync(QRCodeCreateRequest wifiNetwork)
        {
            // Gọi repository insert và gán IDQR
            var newId = await _wifiRepository.CreateWifiAsync(wifiNetwork);
            wifiNetwork.IDQR = newId;
            return wifiNetwork;
        }
        public async Task UpdateWifiAsync(QRCodeUpdateRequest wifiNetwork)
        {
            await _wifiRepository.UpdateWifiAsync(wifiNetwork);
        }
        public async Task DeleteWifiAsync(int id)
        {
            await _wifiRepository.DeleteWifiAsync(id);
        }
        public async Task<QRCodeModel> GetWifiNetworkBySsid(string Ssid)
        {
            return await _wifiRepository.GetWifiNetworkBySsid(Ssid);
        }
    }
}
