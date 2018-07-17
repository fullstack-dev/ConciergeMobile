using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client.Windows
{
    public static class BinaryFileExt
    {
        public static byte[] GetBytes(this BinaryFile file)
        {
            ///TODO we may support file retrieval via url one day...
            return Convert.FromBase64String(file.FileData);
        }


        public static bool SaveToLocation(this BinaryFile file, string folderPath, string filename = null)
        {
            string nameToUse = filename ?? file.FileName;

            FileInfo info = new FileInfo(Path.Combine(folderPath, nameToUse));


            using (var osFile = info.OpenWrite())
            {
                var bytes = file.GetBytes();
                osFile.Write(bytes, 0, bytes.Length);
            }

            return true;
        }
    }
}
