using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.ExternalServicesPersistence.FIleStorageService.DTOs
{
    public class MultipleFilesUploadRequest
    {
        [Required]
        [MinLength(1)]
        public IFormFileCollection Files { get; set; }

        [Required]
        public string Folder { get; set; }


    }
}
