using System;
using System.Security.Claims;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SasTokenLib.SasTokenLib;
using User_lib;

namespace SasTokenLib
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class SasTokenController : ControllerBase
    {
        private readonly SasTokenRepository _sasTokenRepository;
        private readonly string _storageAccountName;
        private readonly string _storageAccountKey;
        private readonly string _containerName;

        public SasTokenController(IConfiguration configuration)
        {
            _sasTokenRepository = new SasTokenRepository();
            _storageAccountName = configuration["AzureStorage:AccountName"];
            _storageAccountKey = configuration["AzureStorage:AccountKey"];
            _containerName = configuration["AzureStorage:ContainerName"];
        }

        [HttpPost("generate")]
        public IActionResult GenerateSasToken([FromBody] SasTokenRequest request)
        {
            try
            {
               
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User authentication failed.");
                }

                if (string.IsNullOrEmpty(_storageAccountName) || string.IsNullOrEmpty(_storageAccountKey))
                    return BadRequest("Azure Storage configuration is missing.");

                const int MaxSasTokenExpiryMinutes = 30;

                if (request.ExpiryMinutes <= 0 || request.ExpiryMinutes > MaxSasTokenExpiryMinutes)
                {
                    return BadRequest($"Expiration time must be between 1 and {MaxSasTokenExpiryMinutes} minutes.");
                }

                string sasToken = _sasTokenRepository.GenerateSas(_storageAccountName, _storageAccountKey, _containerName, request);

                return Ok(new SasTokenResponse
                {
                    SasUrl = sasToken,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(request.ExpiryMinutes)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
