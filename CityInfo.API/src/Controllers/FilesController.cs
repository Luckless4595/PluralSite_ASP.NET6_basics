using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.src.Controllers
{
    [ApiController]
    [Authorize]
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