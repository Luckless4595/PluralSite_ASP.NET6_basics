using Microsoft.AspNetCore.StaticFiles;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase 
    {
        private readonly FileExtensionContentTypeProvider fectp;    
        
        public FilesController( FileExtensionContentTypeProvider fectpIn){
            this.fectp = fectpIn
            ?? throw new System.ArgumentNullException(nameof(fectp));
        }
        
        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId){
            var filePath = "test.csv";

            if (! System.IO.File.Exists(filePath)){
                return NotFound();
            } 

            if (! this.fectp.TryGetContentType(filePath, out var contentType)){
                contentType = "application/octet-stream";
            }
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes , contentType, Path.GetFileName(filePath));
            
        }
    }
}