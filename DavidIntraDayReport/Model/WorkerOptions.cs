using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DavidIntraDayReport.Model
{
    public class WorkerOptions
    {
        public string ReportPath { get; set; }
        public string ReportFilename { get; set; }
        public string CSVSeparator { get; set; }
        public float IntervalMinutes { get; set; }
        public string LocalTimeHeaderName { get; set; }
        public string VolumeHeaderName { get; set; }
        
    }
}
