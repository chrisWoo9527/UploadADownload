using Common.Service;
using Common.Service.CommonDto;
using Common.Service.FileManager;
using Masuit.Tools.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
