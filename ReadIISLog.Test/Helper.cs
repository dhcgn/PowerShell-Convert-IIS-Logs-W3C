using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReadIISLog.Test
{
    public class Helper
    {
        public static string GetValue(Assembly executingAssembly, string fileName)
        {
            var name = executingAssembly.GetManifestResourceNames().Single(x => x.EndsWith(fileName));
            var filepath = Path.GetDirectoryName(executingAssembly.Location) + @"\" + fileName;


            using (Stream stream = executingAssembly.GetManifestResourceStream(name))
            using (var file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(file);
            }

            return filepath;
        }
    }
}
