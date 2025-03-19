using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StockManagement.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;
        private readonly ILogger<EncryptionService> _logger;

        public EncryptionService(IConfiguration configuration, ILogger<EncryptionService> logger)
        {
            _logger = logger;

            string keyString = configuration["Encryption:Key"];
            
            if (string.IsNullOrEmpty(keyString))
            {
                _logger.LogError("Encryption key is missing in appsettings.json!");
                throw new ArgumentNullException(nameof(keyString), "Encryption key is missing in configuration.");
            }

            _logger.LogInformation($"Encryption Key Length: {keyString.Length}"); // Debugging log

            if (keyString.Length != 16 && keyString.Length != 24 && keyString.Length != 32)
            {
                _logger.LogError("Invalid Encryption key length: {Length}", keyString.Length);
                throw new ArgumentException("Encryption key must be 16, 24, or 32 characters long.");
            }

            _key = Encoding.UTF8.GetBytes(keyString);
        }

        public string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = new byte[16];

                using (var encryptor = aes.CreateEncryptor())
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = new byte[16];

                using (var decryptor = aes.CreateDecryptor())
                {
                    var encryptedBytes = Convert.FromBase64String(encryptedText);
                    var plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
    }
}
