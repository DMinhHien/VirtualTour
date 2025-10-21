using Dapper;
using VirtualTour.BL.Services;
using VirtualTour.DataAccess;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Repositories
{
  
    public interface IQRCodeRepository
    {
        Task<List<QRCodeModel>> GetAllWifiNetworksAsync();
        Task<QRCodeModel> GetWifiNetworkByIdAsync(int id);
        Task<QRCodeModel> GetWifiNetworkBySsid(string Ssid);
        Task<int> CreateWifiAsync(QRCodeCreateRequest wifiNetwork);
        Task UpdateWifiAsync(QRCodeUpdateRequest wifiNetwork);
        Task DeleteWifiAsync(int id);

    }
    public class QRCodeRepository : IQRCodeRepository
    {
        private readonly IDbContext _dbContext;
        IPasswordService _passwordService;
        public QRCodeRepository(IDbContext dbContext, IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }
        public async Task<List<QRCodeModel>> GetAllWifiNetworksAsync()
        {
            var storedProcedure = "[dbo].[sp_QRCodeConfigs_GetAll]";
            using (var connection = _dbContext.CreateConnection())
            {
                List<QRCodeModel> wifi_list = (await connection.QueryAsync<QRCodeModel>(storedProcedure, commandType: CommandType.StoredProcedure)).ToList();
                foreach (var wifi in wifi_list)
                {
                    if (wifi.WifiPassHash != null && wifi.WifiPassHash != "")
                    {
                        wifi.WifiPassHash = _passwordService.DecryptString(wifi.WifiPassHash);
                    }
                }
                return wifi_list;
            }
        }
        public async Task<QRCodeModel> GetWifiNetworkByIdAsync(int Id)
        {
            var storedProcedure = "sp_QRCodeConfigs_GetById";
            var parameters = new DynamicParameters();
            parameters.Add("@IDQR", Id);
            using (var connection = _dbContext.CreateConnection())
            {
                var wifi = await connection.QueryFirstOrDefaultAsync<QRCodeModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                wifi.WifiPassHash = _passwordService.DecryptString(wifi.WifiPassHash);
                return wifi;
            }
        }
        public async Task<int> CreateWifiAsync(QRCodeCreateRequest wifiNetwork)
        {
            int isChange = 1;
            if (string.IsNullOrWhiteSpace(wifiNetwork.ChangeOnDays) || wifiNetwork.ChangeOnDays == "None")
            {
                isChange = 0;
                wifiNetwork.ChangeOnDays = null;
            }

            var storedProcedure = "sp_QRCodeConfigs_Insert";
            var parameters = new DynamicParameters();
            parameters.Add("@QRName", wifiNetwork.QRName);
            parameters.Add("@QRType", wifiNetwork.QRType);
            parameters.Add("@QRContent", wifiNetwork.QRContent);
            parameters.Add("@SSIDName", wifiNetwork.SSIDName);
            parameters.Add("@WifiPassHash", _passwordService.EncryptString(wifiNetwork.WifiPassHash));
            parameters.Add("@SecurityType", wifiNetwork.SecurityType);
            parameters.Add("@IsChange", isChange);
            parameters.Add("@ChangeOnDays", wifiNetwork.ChangeOnDays);

            using var connection = _dbContext.CreateConnection();

            var newId = await connection.ExecuteScalarAsync<int>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);

            return newId;
        }

        public async Task UpdateWifiAsync(QRCodeUpdateRequest wifiNetwork)
        {
            int IsChange = 1;
            if (wifiNetwork.ChangeOnDays == "None" || wifiNetwork.ChangeOnDays == "" || wifiNetwork.ChangeOnDays == null)
            {
                IsChange = 0;
                wifiNetwork.ChangeOnDays = null;
            }
            var storedProcedure = "sp_QRCodeConfigs_Update";
            var parameters = new DynamicParameters();
            parameters.Add("@IDQR", wifiNetwork.IDQR);
            parameters.Add("@QRName", wifiNetwork.QRName);
            parameters.Add("@QRType", wifiNetwork.QRType);
            parameters.Add("@QRContent", wifiNetwork.QRContent);
            parameters.Add("@SSIDName", wifiNetwork.SSIDName);
            parameters.Add("@WifiPassHash", _passwordService.EncryptString(wifiNetwork.WifiPassHash));
            parameters.Add("@SecurityType", wifiNetwork.SecurityType);            
            parameters.Add("@IsChange", IsChange); 
            parameters.Add("@ChangeOnDays", wifiNetwork.ChangeOnDays);

            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task DeleteWifiAsync(int id)
        {
            var storedProcedure = "sp_QRCodeConfigs_Delete";
            var parameters = new DynamicParameters();
            parameters.Add("@IDQR", id);
            using (var connection = _dbContext.CreateConnection())
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<QRCodeModel> GetWifiNetworkBySsid(string Ssid)
        {
            var storedProcedure = "sp_QRCodeConfigs_Get_SSID";
            var parameters = new DynamicParameters();
            parameters.Add("@SSIDName", Ssid);
            using (var connection = _dbContext.CreateConnection())
            {
                var res = await connection.QueryFirstOrDefaultAsync<QRCodeModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                res.WifiPassHash = _passwordService.DecryptString(res.WifiPassHash);
                return res;
            }
        }

    }
}
