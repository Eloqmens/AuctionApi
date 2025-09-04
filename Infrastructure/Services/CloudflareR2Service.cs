using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

namespace Infrastructure.Services
{
    public interface ICloudflareR2Service
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
        Task<bool> DeleteImageAsync(string fileName);
        Task<string> GetImageUrlAsync(string fileName);
    }

    public class CloudflareR2Service : ICloudflareR2Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CloudflareR2Service> _logger;
        private readonly string _bucketName;
        private readonly string _publicUrl;

        public CloudflareR2Service(IConfiguration configuration, ILogger<CloudflareR2Service> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _bucketName = _configuration["CloudflareR2:Bucket"] ?? throw new ArgumentNullException("CloudflareR2:Bucket");
            _publicUrl = _configuration["CloudflareR2:PublicUrl"] ?? throw new ArgumentNullException("CloudflareR2:PublicUrl");

            var config = new AmazonS3Config
            {
                ServiceURL = _configuration["CloudflareR2:ServiceUrl"],
                ForcePathStyle = true,
                UseHttp = false
            };

            _s3Client = new AmazonS3Client(
                _configuration["CloudflareR2:AccessKey"],
                _configuration["CloudflareR2:SecretKey"],
                config
            );
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                // Sanitize filename to ASCII and safe chars
                var cleanFileName = SanitizeFileName(fileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{cleanFileName}";

                // Read stream to byte[] to compute signature
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Direct HTTP with SigV4 avoids SDK streaming/trailer issues
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                
                var serviceUrl = _configuration["CloudflareR2:ServiceUrl"];
                var accessKey = _configuration["CloudflareR2:AccessKey"];
                var secretKey = _configuration["CloudflareR2:SecretKey"];
                
                // URL-encode to keep key safe
                var encodedFileName = Uri.EscapeDataString(uniqueFileName);
                var url = $"{serviceUrl}/{_bucketName}/{encodedFileName}";
                
                // Build PUT request
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Content = new ByteArrayContent(fileBytes);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                
                // SigV4 headers
                var now = DateTime.UtcNow;
                var dateStamp = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                var amzDate = now.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
                
                // Canonical request
                var canonicalUri = $"/{_bucketName}/{encodedFileName}";
                var canonicalQueryString = "";
                var payloadHash = Convert.ToHexString(SHA256.HashData(fileBytes)).ToLower();
                var canonicalHeaders = $"host:{new Uri(serviceUrl).Host}\nx-amz-content-sha256:{payloadHash}\nx-amz-date:{amzDate}\n";
                var signedHeaders = "host;x-amz-content-sha256;x-amz-date";
                
                var canonicalRequest = $"PUT\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";
                
                // String to sign
                var algorithm = "AWS4-HMAC-SHA256";
                var credentialScope = $"{dateStamp}/auto/s3/aws4_request";
                var stringToSign = $"{algorithm}\n{amzDate}\n{credentialScope}\n{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalRequest))).ToLower()}";
                
                // Signature
                var signingKey = GetSignatureKey(secretKey, dateStamp, "auto", "s3");
                var signature = Convert.ToHexString(HMACSHA256.HashData(signingKey, Encoding.UTF8.GetBytes(stringToSign))).ToLower();
                
                // Final headers
                request.Headers.Add("x-amz-date", amzDate);
                request.Headers.Add("x-amz-content-sha256", payloadHash);
                request.Headers.TryAddWithoutValidation("Authorization", $"{algorithm} Credential={accessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}");

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var imageUrl = $"{_publicUrl}/{uniqueFileName}";
                    _logger.LogInformation("Image uploaded: {ImageUrl}", imageUrl);
                    return imageUrl;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Image upload failed {FileName}. HTTP: {StatusCode}, Error: {Error}", 
                    fileName, response.StatusCode, errorContent);
                throw new Exception($"Image upload failed. HTTP: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image upload {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string fileName)
        {
            try
            {
                // Extract object key from URL or raw key
                var key = ExtractKeyFromUrl(fileName);

                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var response = await _s3Client.DeleteObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("Image deleted: {FileName}", fileName);
                    return true;
                }

                _logger.LogWarning("Image delete failed {FileName}. HTTP: {StatusCode}", 
                    fileName, response.HttpStatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image delete {FileName}", fileName);
                return false;
            }
        }

        public Task<string> GetImageUrlAsync(string fileName)
        {
            try
            {
                // If already a full URL, return as-is
                if (fileName.StartsWith("http"))
                {
                    return Task.FromResult(fileName);
                }

                // Build URL using ServiceUrl and bucket
                var serviceUrl = _configuration["CloudflareR2:ServiceUrl"];
                var bucketName = _configuration["CloudflareR2:Bucket"];
                return Task.FromResult($"{serviceUrl}/{bucketName}/{fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing image URL {FileName}", fileName);
                throw;
            }
        }

        private string ExtractKeyFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            // If full public URL, strip host prefix
            if (url.StartsWith(_publicUrl))
            {
                return url.Substring(_publicUrl.Length + 1);
            }

            // Already a key
            return url;
        }

        public void Dispose()
        {
            _s3Client?.Dispose();
        }

        private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            var kDate = HMACSHA256.HashData(Encoding.UTF8.GetBytes("AWS4" + key), Encoding.UTF8.GetBytes(dateStamp));
            var kRegion = HMACSHA256.HashData(kDate, Encoding.UTF8.GetBytes(regionName));
            var kService = HMACSHA256.HashData(kRegion, Encoding.UTF8.GetBytes(serviceName));
            var kSigning = HMACSHA256.HashData(kService, Encoding.UTF8.GetBytes("aws4_request"));
            return kSigning;
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "image";

            // Split filename/extension
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            // Transliterate Cyrillic and replace special chars
            var sanitizedName = Transliterate(nameWithoutExtension);

            // Remove invalid filesystem chars
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                sanitizedName = sanitizedName.Replace(c, '_');
            }

            // Limit length
            if (sanitizedName.Length > 50)
            {
                sanitizedName = sanitizedName.Substring(0, 50);
            }

            // Fallback
            if (string.IsNullOrWhiteSpace(sanitizedName))
            {
                sanitizedName = "image";
            }

            return $"{sanitizedName}{extension}";
        }

        private static string Transliterate(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var transliterationMap = new Dictionary<char, string>
            {
                {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"}, {'е', "e"}, {'ё', "yo"},
                {'ж', "zh"}, {'з', "z"}, {'и', "i"}, {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"},
                {'н', "n"}, {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"},
                {'ф', "f"}, {'х', "h"}, {'ц', "ts"}, {'ч', "ch"}, {'ш', "sh"}, {'щ', "sch"},
                {'ъ', ""}, {'ы', "y"}, {'ь', ""}, {'э', "e"}, {'ю', "yu"}, {'я', "ya"},
                {'А', "A"}, {'Б', "B"}, {'В', "V"}, {'Г', "G"}, {'Д', "D"}, {'Е', "E"}, {'Ё', "Yo"},
                {'Ж', "Zh"}, {'З', "Z"}, {'И', "I"}, {'Й', "Y"}, {'К', "K"}, {'Л', "L"}, {'М', "M"},
                {'Н', "N"}, {'О', "O"}, {'П', "P"}, {'Р', "R"}, {'С', "S"}, {'Т', "T"}, {'У', "U"},
                {'Ф', "F"}, {'Х', "H"}, {'Ц', "Ts"}, {'Ч', "Ch"}, {'Ш', "Sh"}, {'Щ', "Sch"},
                {'Ъ', ""}, {'Ы', "Y"}, {'Ь', ""}, {'Э', "E"}, {'Ю', "Yu"}, {'Я', "Ya"}
            };

            var result = new StringBuilder();
            foreach (var c in text)
            {
                if (transliterationMap.TryGetValue(c, out var replacement))
                {
                    result.Append(replacement);
                }
                else if (char.IsLetterOrDigit(c) || c == '-' || c == '_')
                {
                    result.Append(c);
                }
                else
                {
                    result.Append('_');
                }
            }

            return result.ToString();
        }
    }
}
