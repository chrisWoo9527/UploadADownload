using Common.Service;
using Common.Service.CommonDto;
using Common.Service.FileManager;
using Masuit.Tools.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using UploadADownload.Service;

namespace UploadADownload.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UpDownController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;
        private readonly IUDloadService _uDloadService;

        public UpDownController(IConfiguration configuration, IFileService fileService, IUDloadService uDloadService)
        {
            _configuration = configuration;
            _fileService = fileService;
            _uDloadService = uDloadService;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<FileUpLoadDto>> UploadFile(IFormFile file)
        {
            Log.Information("UploadFile:入参进入");
            return await _uDloadService.FileUpLoadAsync(file);
        }

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<List<FileUpLoadDto>>> UploadFileMore(List<IFormFile> files)
        {
            return await _uDloadService.FileUpLoadAsync(files);
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileName">文件名称(含后缀)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<FileStream>> Download(string fileName)
        {
            if (fileName == null)
            {
                return NotFound("filename not present");
            }
            string? filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var path = Path.Combine(filePath, _configuration.GetSection("Ftp").Value, fileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound("filename not present");
            }

            using (var memory = new MemoryStream())
            {
                using (var stream = new FileStream(path, FileMode.OpenOrCreate,FileAccess.ReadWrite))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, FileContentType.GetContentType(path), fileName);
            }
        }


        [HttpGet]
        public ActionResult<List<FileInformation>> SelectFiles()
        {
            return _uDloadService.GetFileInformation();
        }
    }
}
