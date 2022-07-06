using Common.Service;
using Common.Service.CommonDto;
using Common.Service.FileManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace UploadADownload.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UpDownController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;

        public UpDownController(IConfiguration configuration, IFileService fileService)
        {
            _configuration = configuration;
            _fileService = fileService;
        }

        /// <summary>
        /// 文件上传下载
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<FileUpLoadDto>> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new FileUpLoadDto { Status = false, Message = "stream is not null" };

            string? FtpPath = _configuration.GetSection("Ftp").Value;

            if (string.IsNullOrEmpty(FtpPath))
            {
                return new FileUpLoadDto { Status = false, Message = "Ftp 文件夹不存在" };
            }

            if (file.Length > (long)10 * 1024 * 1024 * 1024)
                return new FileUpLoadDto { Status = false, Message = "出于限制无法上传大于 1G 文件" };

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                string path = Path.Combine(FtpPath, file.FileName);

                string? DeskPath = Directory.GetFiles(path, "*", SearchOption.AllDirectories).FirstOrDefault(w => w == path);

                if (!string.IsNullOrEmpty(DeskPath))
                {
                    using var deskStream = new FileStream(DeskPath, FileMode.Open, FileAccess.Read);
                    if(deskStream.GetFileMD5()==file.OpenReadStream().GetFileMD5())
                    {
                        return new FileUpLoadDto { Status = false, Message = "MD5值相同，无需重复上传" };
                    }
                }

                var directoryPath = Path.GetDirectoryName(path);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                await file.OpenReadStream().CopyToFileAsync(path);
                stopwatch.Stop();

                return new FileUpLoadDto { Status = true, Message = "", Seconds = stopwatch.Elapsed };
            }
            catch (Exception ex)
            {
                return new FileUpLoadDto { Status = false, Message = ex.Message };
            }
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileName">文件名称(含后缀)</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Download(string fileName)
        {
            if (fileName == null)
            {
                return Content("filename not present");
            }

            var path = Path.Combine(_configuration.GetSection("Ftp").Value, fileName);

            var memory = new MemoryStream();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, FileContentType.GetContentType(path), fileName);
        }


        [HttpGet]
        public ActionResult<List<FileInformation>> SelectFiles()
        {
            List<FileInformation> list = new List<FileInformation>();
            var fileList = Directory.GetFiles(Path.Combine(_configuration.GetSection("Ftp").Value)).ToList();
            fileList.ForEach(file =>
            {
                list.Add(_fileService.GetFileInformation(file));
            });
            return list;
        }
    }
}
