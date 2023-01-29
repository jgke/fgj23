#if ANDROID
using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Support
{
    public static class ResourceLoader
    {
        private static AssetManager _instance;
        public static void SetAssetInstance(AssetManager assets)
        {
            _instance = assets;
        }

        public static byte[] ReadResource(string path)
        {
            const int maxReadSize = 256 * 1024;
            byte[] content;
            using (BinaryReader br = new BinaryReader(_instance.Open(path)))
            {
                content = br.ReadBytes(maxReadSize);
            }

            return content;
        }
    }
}

#else

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Support
{
    static class ResourceLoader
    {
        public static byte[] ReadResource(string path)
        {
            string binaryPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string binDir = System.IO.Path.GetDirectoryName(binaryPath);
            return File.ReadAllBytes(Path.Join(binDir, path));
        }
    }
}

#endif
