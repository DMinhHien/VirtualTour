using VirtualTour.BL.Repositories;
using VirtualTour.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Services
{
    public interface IApiKeyService
    {
        Task<string> GenerateApiKeyAsync(int userId);
        Task<bool> ValidateApiKeyAsync(string rawKey);
        Task<string> GenerateRandomApiKey(int length = 32);
        Task<ApiKeyModel?> GetApiKeyByUserIdAsync(int userId);
        Task UpdateApiKeyAsync(ApiKeyModel apiKey);
        //Task<string> ComputeSha256Hash(string rawData);

    }
    public class ApiKeyService: IApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IPasswordService _passwordService;
        public ApiKeyService(IApiKeyRepository apiKeyRepository, IPasswordService passwordService)
        {
            _apiKeyRepository = apiKeyRepository;
            _passwordService = passwordService;
        }

        // Generates a new API key for a given user, stores the hashed key, and returns the raw key.
        public async Task<string> GenerateApiKeyAsync(int userId)
        {
            var rawKey = await GenerateRandomApiKey();
            var hashKey =_passwordService.EncryptString(rawKey);
            var apiKey = new ApiKeyModel
            {
                UserId = userId,
                HashKey = hashKey,
                ExpiredAt = DateTime.Now.AddDays(7),
            };
            await _apiKeyRepository.CreateApiKeyAsync(apiKey);
            return rawKey;
        }

        // Validates the incoming raw API key.
        public async Task<bool> ValidateApiKeyAsync(string rawKey)
        {
            var hashKey =_passwordService.EncryptString(rawKey);
            var apiKey = await _apiKeyRepository.GetApiKeyByHashAsync(hashKey);
            if (apiKey == null || apiKey.IsRevoked) return false;
            if (apiKey.ExpiredAt.HasValue && apiKey.ExpiredAt.Value < DateTime.UtcNow) return false;

            // Optionally update last used time.
            apiKey.LastUsedAt = DateTime.UtcNow;
            await _apiKeyRepository.UpdateApiKeyAsync(apiKey);
            return true;
        }

        public Task<string> GenerateRandomApiKey(int length = 50)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder sb = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);
                for (int i = 0; i < length; i++)
                {
                    int index = randomBytes[i] % allowedChars.Length;
                    sb.Append(allowedChars[index]);
                }
            }
            return Task.FromResult(sb.ToString());
        }
        public async Task<ApiKeyModel?> GetApiKeyByUserIdAsync(int userId)
        {
            var apiKey=await _apiKeyRepository.GetApiKeyByUserIdAsync(userId);
            apiKey.HashKey = _passwordService.DecryptString(apiKey.HashKey);
            return apiKey;
        }
        public async Task UpdateApiKeyAsync(ApiKeyModel apiKey)
        {
            apiKey.HashKey = _passwordService.EncryptString(apiKey.HashKey);
            await _apiKeyRepository.UpdateApiKeyAsync(apiKey);
        }
    }
}
