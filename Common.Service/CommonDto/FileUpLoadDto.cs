using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service.CommonDto
{
    public class FileUpLoadDto
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public TimeSpan? Seconds { get; set; }
    }
}
