using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.FIleStorageService.DTOs
{
    public class FileDeleteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
