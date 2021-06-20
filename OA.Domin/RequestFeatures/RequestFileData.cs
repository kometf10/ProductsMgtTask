using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.RequestFeatures
{
    public class RequestFileData
    {
        public byte[] Data { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }

    }
}
