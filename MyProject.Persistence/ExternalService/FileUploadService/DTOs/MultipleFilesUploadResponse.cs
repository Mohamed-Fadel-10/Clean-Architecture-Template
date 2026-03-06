using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.FIleStorageService.DTOs
{
    public class MultipleFilesUploadResponse
    {
        public bool Success { get; set; }
        public List<FileUploadResponse> Files { get; set; } = new();
        public string Message { get; set; }
    }
}
