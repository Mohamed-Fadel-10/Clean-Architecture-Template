using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.FIleStorageService.DTOs
{
    public class FileUploadResponse
    {
        public bool Success { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public string Folder { get; set; }
        public string Message { get; set; }
        
    }

}
