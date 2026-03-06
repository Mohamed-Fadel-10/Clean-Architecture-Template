using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Persistence.ExternalServices.FileStorageService;
using Persistence.ExternalServices.FIleStorageService.FileUploadandResponseResult;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly HashSet<string> _allowedImageExtensions = new()
        { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg" };

    private readonly HashSet<string> _allowedDocumentExtensions = new()
        { ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".zip", ".rar" };

    private const long MaxImageSize = 10 * 1024 * 1024;       // 10MB
    private const long MaxDocumentSize = 20 * 1024 * 1024;  // 20MB

    public FileStorageService(
        IWebHostEnvironment env,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Upload

    public async Task<FileUploadResult> UploadFileAsync(
        IFormFile file,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return FileUploadResult.Failure("File is empty");

        if (string.IsNullOrWhiteSpace(folderName))
            return FileUploadResult.Failure("Folder name is required");

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        HashSet<string> allowedExtensions;
        long maxSize;

        if (_allowedImageExtensions.Contains(fileExtension))
        {
            allowedExtensions = _allowedImageExtensions;
            maxSize = MaxImageSize;
        }
        else if (_allowedDocumentExtensions.Contains(fileExtension))
        {
            allowedExtensions = _allowedDocumentExtensions;
            maxSize = MaxDocumentSize;
        }
        else
        {
            var allAllowed = _allowedImageExtensions.Concat(_allowedDocumentExtensions);
            return FileUploadResult.Failure(
                $"File type not allowed. Allowed types: {string.Join(", ", allAllowed)}");
        }

        var validationResult = ValidateFile(file, allowedExtensions, maxSize);
        if (!validationResult.IsValid)
            return FileUploadResult.Failure(validationResult.ErrorMessage);

        return await SaveFileAsync(file, folderName, cancellationToken, fileExtension);
    }

    public async Task<List<FileUploadResult>> UploadMultipleFilesAsync(
        IFormFileCollection files,
        string folderName,
        CancellationToken cancellationToken = default)
    {
        if (files == null || files.Count == 0)
            return new List<FileUploadResult>();

        var tasks = files.Select(file =>
            UploadFileAsync(file, folderName, cancellationToken));

        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    #endregion

    #region Save

    private string GetUploadsFolder(string folderName)
    {
        if (string.IsNullOrWhiteSpace(_env.WebRootPath))
            throw new InvalidOperationException(
                "WebRootPath is not configured. Ensure wwwroot folder exists.");

        folderName = folderName.Trim().ToLower();

        var uploadsFolder = Path.Combine(
            _env.WebRootPath,
            "uploads",
            folderName);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        return uploadsFolder;
    }

    private async Task<FileUploadResult> SaveFileAsync(
    IFormFile file,
    string folderName,
    CancellationToken cancellationToken,
    string fileExtension)
    {
        try
        {
            var uploadsFolder = GetUploadsFolder(folderName);
            var prefix = _allowedImageExtensions.Contains(fileExtension) ? "img_" : "file_";
            var uniqueFileName = $"{prefix}{Guid.NewGuid():N}{fileExtension}";
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var stream = new FileStream(
                fullPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                useAsync: true);

            await file.CopyToAsync(stream, cancellationToken);

            var relativePath = Path.Combine("uploads", folderName, uniqueFileName)
                                   .Replace("\\", "/");

            return FileUploadResult.Success(
                relativePath: relativePath,
                originalName: file.FileName,
                storedFileName: uniqueFileName,  
                contentType: file.ContentType,
                size: file.Length,
                folderName: folderName);
        }
        catch (Exception ex)
        {
            return FileUploadResult.Failure($"Error saving file: {ex.Message}");
        }
    }

    #endregion

    #region Delete

    public Task<FileDeleteResult> DeleteFileAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.FromResult(
                    FileDeleteResult.Failure("File path is empty"));

            if (string.IsNullOrWhiteSpace(_env.WebRootPath))
                throw new InvalidOperationException(
                    "WebRootPath is not configured.");

            var fullPath = Path.Combine(
                _env.WebRootPath,
                filePath.TrimStart('/'));

            if (!File.Exists(fullPath))
                return Task.FromResult(
                    FileDeleteResult.Failure("File not found"));

            File.Delete(fullPath);
            return Task.FromResult(FileDeleteResult.Success());
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                FileDeleteResult.Failure($"Error deleting file: {ex.Message}"));
        }
    }

    public Task<FileDeleteResult> DeleteFolderAsync(string folderName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return Task.FromResult(
                    FileDeleteResult.Failure("Folder name is empty"));

            if (string.IsNullOrWhiteSpace(_env.WebRootPath))
                throw new InvalidOperationException(
                    "WebRootPath is not configured.");

            var folderPath = Path.Combine(
                _env.WebRootPath,
                "uploads",
                folderName.Trim().ToLower());

            if (!Directory.Exists(folderPath))
                return Task.FromResult(FileDeleteResult.Success());

            Directory.Delete(folderPath, recursive: true);
            return Task.FromResult(FileDeleteResult.Success());
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                FileDeleteResult.Failure($"Error deleting folder: {ex.Message}"));
        }
    }
    public async Task<(byte[] FileBytes, string FileName, string ContentType)> GetFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is empty", nameof(filePath));

        if (string.IsNullOrWhiteSpace(_env.WebRootPath))
            throw new InvalidOperationException("WebRootPath is not configured.");

        var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found", fullPath);

        var fileBytes = await File.ReadAllBytesAsync(fullPath);
        var fileName = Path.GetFileName(fullPath);
        var contentType = GetContentType(fullPath);

        return (fileBytes, fileName, contentType);
    }

   


    #endregion

    #region Helpers

    public string GenerateFileUrl(string relativePath)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return relativePath;

        relativePath = relativePath.TrimStart('/');
        return $"{request.Scheme}://{request.Host}/{relativePath}";
    }

    private FileValidationResult ValidateFile(
        IFormFile file,
        HashSet<string> allowedExtensions,
        long maxSize)
    {
        if (file == null || file.Length == 0)
            return FileValidationResult.Invalid("File is empty");

        if (file.Length > maxSize)
            return FileValidationResult.Invalid(
                $"File size exceeds maximum allowed size of {maxSize / (1024 * 1024)}MB");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return FileValidationResult.Invalid(
                $"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
        }

        return FileValidationResult.Valid();
    }
    private string GetContentType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };
    }

    #endregion
}
