using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class for encapsulaing binary files across the API interface. The file may be embedded within the structure, or it may be passed as a reference.
    /// 
    /// </summary>
    public class BinaryFile
    {
        public string FileName { get; set; }

        public string MimeType { get; set; }

        /// <summary>
        /// This may be the actual data of the file, or it may be a pointer to a resource where the file is located.
        /// You should not use this member directly, but use appropriate client-side utilities for accesing the file's contents.
        /// </summary>
        public string FileData { get; set; }
    }
}
