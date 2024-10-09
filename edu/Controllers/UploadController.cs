using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Kendo.Mvc.Examples.Controllers.Upload
{
    public partial class UploadController : Controller
    {
        private const string contentFolderRoot = "wwwroot/shared/";
        private const string folderName = "Images/";

        public ActionResult Validation()
        {
            return View();
        }

        public async Task<ActionResult> Validation_Save(IEnumerable<IFormFile> files)
        {
            var physicalPath = "";
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var userfile = User.FindFirst(ClaimTypes.Name)?.Value + "/";
                     physicalPath = Path.Combine(contentFolderRoot, "UserFiles", folderName + userfile+ fileName);
              
                    // The files are not actually saved in this demo
                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Validation_Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var userfile = User.FindFirst(ClaimTypes.Name)?.Value + "/";
                    var physicalPath = Path.Combine(contentFolderRoot, "UserFiles", folderName + userfile + fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                         System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }
    }
}