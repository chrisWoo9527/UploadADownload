using Common.Service.CommonDto;
using Common.Service.FileManager;
using Microsoft.AspNetCore.Mvc;

namespace UploadADownload.Service
{
    public interface IUDloadService
    {
        Task<FileUpLoadDto> FileUpLoadAsync(IFormFile file);

        Task<List<FileUpLoadDto>> FileUpLoadAsync(List<IFormFile> files);

        Task<List<FileInformation>> GetFileInformationAsync();
    }
}
