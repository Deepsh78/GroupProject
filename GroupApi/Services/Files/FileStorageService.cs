using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GroupApi.Services.Files
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _uploadDirectory;
        private readonly long _maxFileSizeBytes;
        private readonly string[] _allowedExtensions;

        public FileStorageService(
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<FileStorageService> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uploadDirectory = _configuration["FileStorageSettings:UploadDirectory"] ?? "wwwroot/Uploads/book-images";
            _maxFileSizeBytes = _configuration.GetValue<long>("FileStorageSettings:MaxFileSizeBytes", 10485760);
            _allowedExtensions = _configuration.GetSection("FileStorageSettings:AllowedExtensions").Get<string[]>() ?? new[] { ".jpg", ".png", ".pdf", ".txt" };
            _logger.LogDebug("FileStorageService initialized with upload directory: {UploadDirectory}", _uploadDirectory);
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            try
            {
                _logger.LogInformation("Attempting to save file: {FileName}", file?.FileName);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Null or empty file provided.");
                    throw new ArgumentException("No file provided.");
                }

                if (file.Length > _maxFileSizeBytes)
                {
                    _logger.LogWarning("File size {FileSize} exceeds maximum limit of {MaxSize} bytes.", file.Length, _maxFileSizeBytes);
                    throw new ArgumentException($"File size exceeds the maximum limit of {_maxFileSizeBytes / 1024 / 1024} MB.");
                }

                var extension = Path.GetExtension(file.FileName)?.ToLower();
                if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                {
                    _logger.LogWarning("Invalid file extension {Extension}. Allowed: {AllowedExtensions}", extension, string.Join(", ", _allowedExtensions));
                    throw new ArgumentException($"File extension {extension} is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}.");
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var relativePath = Path.Combine("Uploads", "book-images", fileName).Replace("\\", "/");
                var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    _logger.LogInformation("Creating directory: {Directory}", directory);
                    Directory.CreateDirectory(directory);
                }

                // Verify writability
                var testFile = Path.Combine(directory, ".write-test");
                try
                {
                    await File.WriteAllTextAsync(testFile, "test");
                    File.Delete(testFile);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Directory {Directory} is not writable.", directory);
                    throw new IOException($"Directory {directory} is not writable: {ex.Message}");
                }

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("File saved successfully at: {FilePath}", relativePath);
                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save file: {FileName}", file?.FileName);
                throw new IOException($"Failed to save file: {ex.Message}", ex);
            }
        }

        public async Task<byte[]?> GetFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve file: {FilePath}", filePath);

                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogWarning("GetFileAsync called with null or empty file path.");
                    return null;
                }

                var fullPath = Path.Combine(_environment.WebRootPath, filePath);
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("File not found at: {FilePath}", fullPath);
                    return null;
                }

                var bytes = await File.ReadAllBytesAsync(fullPath);
                _logger.LogInformation("File retrieved successfully from: {FilePath}", filePath);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve file: {FilePath}", filePath);
                throw new IOException($"Failed to retrieve file: {ex.Message}", ex);
            }
        }

        public async Task DeleteFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Attempting to delete file: {FilePath}", filePath);

                if (string.IsNullOrEmpty(filePath))
                {
                    _logger.LogWarning("DeleteFileAsync called with null or empty file path.");
                    return;
                }

                var fullPath = Path.Combine(_environment.WebRootPath, filePath);
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                }
                else
                {
                    _logger.LogWarning("File not found for deletion: {FilePath}", fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {FilePath}", filePath);
                throw new IOException($"Failed to delete file: {ex.Message}", ex);
            }
        }
    }
}