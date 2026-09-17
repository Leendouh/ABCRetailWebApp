using ABCRetailWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailWebApp.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileStorageService fileStorageService, ILogger<FileController> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        // GET: File
        public async Task<IActionResult> Index()
        {
            try
            {
                var files = await _fileStorageService.ListFilesAsync();
                return View(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading files");
                TempData["Error"] = "Error loading files from Azure File Storage";
                return View(Enumerable.Empty<string>());
            }
        }

        // POST: File/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _fileStorageService.UploadFileAsync(file);
                _logger.LogInformation($"File {file.FileName} uploaded successfully");
                TempData["Success"] = $"File '{file.FileName}' uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                TempData["Error"] = $"Error uploading file: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: File/Download
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                TempData["Error"] = "File name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var fileStream = await _fileStorageService.DownloadFileAsync(fileName);
                if (fileStream == null)
                {
                    TempData["Error"] = $"File '{fileName}' not found.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation($"File {fileName} downloaded successfully");
                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                TempData["Error"] = $"Error downloading file: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: File/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                TempData["Error"] = "File name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _fileStorageService.DeleteFileAsync(fileName);
                _logger.LogInformation($"File {fileName} deleted successfully");
                TempData["Success"] = $"File '{fileName}' deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                TempData["Error"] = $"Error deleting file: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}