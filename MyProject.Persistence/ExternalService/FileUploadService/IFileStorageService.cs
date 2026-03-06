using Microsoft.AspNetCore.Http;
using Persistence.ExternalServices.FIleStorageService.FileUploadandResponseResult;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.FileStorageService
{
    public interface IFileStorageService
    {
        Task<FileUploadResult> UploadFileAsync(
            IFormFile file,
            string folderName,
            CancellationToken cancellationToken = default);

        Task<List<FileUploadResult>> UploadMultipleFilesAsync(
            IFormFileCollection files,
            string folderName,
            CancellationToken cancellationToken = default);

        Task<FileDeleteResult> DeleteFileAsync(string filePath);

        Task<FileDeleteResult> DeleteFolderAsync(string folderName);
        Task<(byte[] FileBytes, string FileName, string ContentType)> GetFileAsync(string filePath);

        string GenerateFileUrl(string relativePath);
    }
}
