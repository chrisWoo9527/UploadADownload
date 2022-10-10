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
        [RequestSizeLimit(1024 * 1024 * 5)]
        [DisableRequestSizeLimit]
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
        [RequestSizeLimit(1024 * 1024 * 5)]
        [DisableRequestSizeLimit]
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
        [RequestSizeLimit(1024 * 1024 * 5)]
        [DisableRequestSizeLimit]
        [HttpGet]
        public async Task<ActionResult<FileStream>> Download(string fileName)
        {
            if (fileName == null)
            {
                return NotFound("文件名不允许为空~");
            }
            string? filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string path = Path.Combine(filePath, _configuration.GetSection("Ftp").Value, fileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound($"远程服务器没有找到名称为【{fileName}】 的文件~");
            }

            //  这个流还不能关闭 关闭了客户端读不到数据 死坑
            var memory = new MemoryStream();

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            await stream.CopyToAsync(memory);
            memory.Position = 0;
            return File(memory, FileContentType.GetContentType(path), fileName);
        }


        [HttpDelete]
        public ActionResult<ResultMessage> DeleteFile(string fileName)
        {
            if (fileName == null)
            {
                return new ResultMessage { Status = false, Message = "文件名不允许为空~" };
            }
            string? filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string path = Path.Combine(filePath, _configuration.GetSection("Ftp").Value, fileName);

            if (!System.IO.File.Exists(path))
            {
                return new ResultMessage { Status = false, Message = $"远程服务器没有找到名称为【{fileName}】 的文件~" };
            }

            System.IO.File.Delete(path);

            return new ResultMessage { Status = true, Message = $"" };

        }


        [HttpGet]
        public ActionResult<List<FileInformation>> SelectFiles()
        {
            return _uDloadService.GetFileInformation();
        }
    }
}
