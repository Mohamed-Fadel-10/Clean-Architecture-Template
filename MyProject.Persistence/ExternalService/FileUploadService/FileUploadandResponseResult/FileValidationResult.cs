using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServices.FIleStorageService.FileUploadandResponseResult
{
    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public static FileValidationResult Valid()
        {
            return new FileValidationResult { IsValid = true };
        }

        public static FileValidationResult Invalid(string errorMessage)
        {
            return new FileValidationResult { IsValid = false, ErrorMessage = errorMessage };
        }
    }
}
