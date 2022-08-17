using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QulixTest.Core.Domain;
using QulixTest.Core.IRepositories;

namespace QulixTest.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<ImageController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ImageController(IWebHostEnvironment webHostEnvironment,
        ILogger<ImageController> logger, IUnitOfWork unitOfWork)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> UploadFiles(List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            _logger.LogError($"Invalid Upload attemp in {nameof(UploadFiles)}");
            return BadRequest("Submitted data is invalid");
        }

        string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "NewFolder");

        List<ImageEntity> imageEntity = new();

        foreach (var file in files)
        {
            System.Guid guid = System.Guid.NewGuid();
            var fileName = file.FileName + guid.ToString();

            //var fileName = file.FileName + " " + Guid.NewGuid().ToString();
            string filePath = Path.Combine(directoryPath, fileName);

            imageEntity.Add(new ImageEntity()
            {
                imageUrl = filePath,
                OriginalSize = file.Length,
                CreatedDate = DateTime.Now
            });

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }
        _logger.LogError($"File upload is success {nameof(UploadFiles)}");
        
        // TODO: These lines are throwing null exception but adding to the database ???

        await _unitOfWork.Files.InsertRange(imageEntity);
        await _unitOfWork.SaveAsync();

        return Ok("Upload Success");

    }
} 
