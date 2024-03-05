using HelloDoc.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class ViewUploadModel
    {
        public string? ConfirmationNumber { get; set; }

        public List<Requestwisefile> totalFiles { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int requestId { get; set; }

        public List<IFormFile> upload { get; set; }
    }
}
