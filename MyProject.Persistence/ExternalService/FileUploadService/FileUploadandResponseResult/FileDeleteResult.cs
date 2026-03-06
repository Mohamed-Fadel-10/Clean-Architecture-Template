using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.FIleStorageService.FileUploadandResponseResult
{
    public class FileDeleteResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public static FileDeleteResult Success()
        {
            return new FileDeleteResult { IsSuccess = true };
        }

        public static FileDeleteResult Failure(string errorMessage)
        {
            return new FileDeleteResult { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
