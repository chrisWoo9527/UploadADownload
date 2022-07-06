using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service.FileManager
{
    public class FileInformation
    {
        /// <summary>
        /// 文件名称(含后缀)
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// 文件Md5值
        /// </summary>
        public string? FileMd5 { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string? FileSize { get; set; }

        /// <summary>
        /// 最后修改日期
        /// </summary>
        public DateTime? LastModifyTime { get; set; }
    }
}
