using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service.FileManager
{
    public interface IFileService
    {
        FileInformation GetFileInformation(string filePath);
    }
}
