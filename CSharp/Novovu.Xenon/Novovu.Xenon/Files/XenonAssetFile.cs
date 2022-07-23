using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.IO.Compression;
namespace Novovu.Xenon.Files
{
    public class XenonAssetFile
    {
        public XenonAssetFile()
        {
        }

        public string Name;
        public Stream FileStream;
        public string Hash;
        public string FileId;

        public void SaveToFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XenonAssetFile));
            Stream manifest = new MemoryStream();
            XmlWriter writer = new XmlTextWriter(manifest, Encoding.Unicode);
            XenonAssetFile tempObj = new XenonAssetFile();
            tempObj.FileId = FileId;
            tempObj.Name = Name;
            tempObj.Hash = Hash;
            serializer.Serialize(writer, tempObj);
            using (MemoryStream mem = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(mem))
                {
                    ZipArchiveEntry m = archive.CreateEntry("manifest.xml");
                    manifest.CopyTo(m.Open());
                    ZipArchiveEntry file = archive.CreateEntry("asset.a");
                    FileStream.CopyTo(file.Open());
                }
                using (Stream filestream = File.Create(filename))
                {
                    mem.CopyTo(filestream);
                }
            }
        }
    }
}
