using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ProtocolLoadingTester
{
    //create protocol list and activate data after selecting protocol
    public class SerializeData
    {
        public readonly string path = $@"\\sdappvimg004\esapi$\hlowe\BladderMin\Protocols\";
        
        //public async Task<List<string>> LoadFileNames()
        //{
        //    var files = await Task.Run(() => ReadFolder());
        //    return files;
        //}
        public List<string> ReadFolder()
        {
            return Directory.GetFiles(path).ToList();
        }
        //public async Task<Protocol_Preprocessor.BladderminProtocol> LoadProtocol(string file)
        //{
        //    var xmlData = await Task.Run(() => SerializeProtocol(file));
        //    return xmlData;
        //}
        public Protocol_Preprocessor.BladderminProtocol SerializeProtocol(string file)
        {
            var xmlTools = new XmlTools();
            var serializer = xmlTools.Serializer;
            using (var stream = new StreamReader(file))
            {
                using (var reader = XmlReader.Create(stream, xmlTools.Settings))
                {
                    var deserializeFile = (Protocol_Preprocessor.BladderminProtocol)serializer.Deserialize(reader);
                    return deserializeFile;
                }
            }
        }
    }
}
