using Common.Service;
using Common.Service.AutoFacManager;
using Common.Service.CommonDto;
using Common.Service.FileManager;
using Masuit.Tools.Files;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace UploadADownload.Service
{
    public class UDloadService : IUDloadService, ISingletonService
    {
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;

        public UDloadService(IConfiguration configuration, IFileService fileService)
        {
            _configuration = configuration;
            _fileService = fileService;
        }

        public async Task<FileUpLoadDto> FileUpLoadAsync(IFormFile file)
        {

            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string? FtpPath = Path.Combine(filePath, _configuration.GetSection("Ftp").Value);

            if (!Directory.Exists(FtpPath))
                Directory.CreateDirectory(FtpPath);

            #region 数据判断
            if (file == null || file.Length == 0)
                return (new FileUpLoadDto { Status = false, Message = "stream is not null" });

            if (string.IsNullOrEmpty(FtpPath))
            {
                return new FileUpLoadDto { Status = false, Message = "Ftp 文件夹不存在" };
            }

            if (file.Length > (long)100 * 1024 * 1024)
                return new FileUpLoadDto { Status = false, Message = "出于限制无法上传大于 500MB 文件" };

            string path = Path.Combine(FtpPath, file.FileName);

            string? DeskPath = Directory.GetFiles(FtpPath, "*", SearchOption.AllDirectories)?.FirstOrDefault(w => w == path);

            if (!string.IsNullOrEmpty(DeskPath))
            {
                using (var deskStream = new FileStream(DeskPath, FileMode.Open, FileAccess.Read))
                {
                    if (deskStream.GetFileMD5() == file.OpenReadStream().GetFileMD5())
                    {
                        return new FileUpLoadDto { Status = false, Message = "MD5值相同，无需重复上传" };
                    }
                }

            }
            #endregion
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();

                var directoryPath = Path.GetDirectoryName(path);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                await file.OpenReadStream().CopyToFileAsync(path);
                stopwatch.Stop();

                return new FileUpLoadDto { Status = true, Message = "", Seconds = stopwatch.Elapsed.Seconds, fileInfo = _fileService.GetFileInformation(path) };
            }
            catch (Exception ex)
            {
                return new FileUpLoadDto { Status = false, Message = ex.Message };
            }
        }

        public async Task<List<FileUpLoadDto>> FileUpLoadAsync(List<IFormFile> files)
        {

            string? filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string? FtpPath = Path.Combine(filePath, _configuration.GetSection("Ftp").Value);

            if (!Directory.Exists(FtpPath))
                Directory.CreateDirectory(FtpPath);

            #region 数据判断
            if (files == null)
                return new List<FileUpLoadDto> { new FileUpLoadDto { Status = false, Message = "stream is not null" } };

            if (string.IsNullOrEmpty(FtpPath))
            {
                return new List<FileUpLoadDto> { new FileUpLoadDto { Status = false, Message = "Ftp 文件夹不存在" } };
            }

            long fileSize = 0;
            files.ForEach(w => fileSize = fileSize + w.Length);

            if (files.GetFilesLength() > (long)1024 * 1024 * 1024)
                return new List<FileUpLoadDto> { new FileUpLoadDto { Status = false, Message = "出于限制无法上传大于 1G 文件" } };

            #endregion

            List<FileUpLoadDto> list = new List<FileUpLoadDto>();


            files.ForEach(w =>
             {
                 string path = Path.Combine(FtpPath, w.FileName);

                 string? DeskPath = Directory.GetFiles(FtpPath, "*", SearchOption.AllDirectories)?.FirstOrDefault(w => w == path);

                 if (!string.IsNullOrEmpty(DeskPath))
                 {
                     using var deskStream = new FileStream(DeskPath, FileMode.Open, FileAccess.Read);
                     if (deskStream.GetFileMD5() == w.OpenReadStream().GetFileMD5())
                     {
                         list.Add(new FileUpLoadDto { Status = false, Message = "MD5值相同，无需重复上传" });
                     }
                 }
                 else
                 {
                     try
                     {
                         Stopwatch stopwatch = Stopwatch.StartNew();
                         stopwatch.Start();

                         var directoryPath = Path.GetDirectoryName(path);

                         if (!Directory.Exists(directoryPath))
                             Directory.CreateDirectory(directoryPath);

                         w.OpenReadStream().CopyToFile(path);
                         stopwatch.Stop();

                         list.Add(new FileUpLoadDto { Status = true, Message = "", Seconds = stopwatch.Elapsed.Seconds, fileInfo = _fileService.GetFileInformation(path) });
                     }
                     catch (Exception ex)
                     {
                         list.Add(new FileUpLoadDto { Status = false, Message = ex.Message });
                     }
                 }
             });

            return await Task.FromResult(list);
        }

        public List<FileInformation> GetFileInformation()
        {
            List<FileInformation> list = new List<FileInformation>();
            string? filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            var fileList = Directory.GetFiles(Path.Combine(filePath, _configuration.GetSection("Ftp").Value)).ToList();
            fileList.ForEach(file =>
            {
                list.Add(_fileService.GetFileInformation(file));
            });

            var fileInformations = (from a in list
                                    orderby a.LastModifyTime descending
                                    select a).ToList();

            return fileInformations;
        }

    }
}
