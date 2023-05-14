using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace CityInfo.API.src.Controllers
{
    /// <summary>
    /// Controller for handling file operations.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/files")]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider fectp;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesController"/> class.
        /// </summary>
        /// <param name="fectpIn">The file extension content type provider.</param>
        public FilesController(FileExtensionContentTypeProvider fectpIn)
        {
            this.fectp = fectpIn
                ?? throw new System.ArgumentNullException(nameof(fectp));
        }

        /// <summary>
        /// Gets the file by its ID.
        /// </summary>
        /// <param name="fileId">The file ID.</param>
        /// <returns>The file.</returns>
        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var filePath = "test.csv";

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            if (!this.fectp.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, Path.GetFileName(filePath));
        }
    }
}
