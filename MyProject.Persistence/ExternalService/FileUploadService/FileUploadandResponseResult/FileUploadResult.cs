using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.FIleStorageService.FileUploadandResponseResult
{
    public class FileUploadResult
    {
        public bool IsSuccess { get; private set; }
        public string? RelativePath { get; private set; }
        public string? OriginalName { get; private set; }
        public string? StoredFileName { get; private set; }  
        public string? ContentType { get; private set; } 
        public long Size { get; private set; }
        public string? Folder { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public static FileUploadResult Success(
          string relativePath,
          string originalName,
          string storedFileName,   
          string contentType,      
          long size,
          string folderName) => new()
          {
              IsSuccess = true,
              RelativePath = relativePath,
              OriginalName = originalName,
              StoredFileName = storedFileName,
              ContentType = contentType,
              Size = size,
              Folder = folderName,
          };

        public static FileUploadResult Failure(string errorMessage)
        {
            return new FileUploadResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
