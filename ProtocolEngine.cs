using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLoadingTester
{
    public class ProtocolBuffer
    {
        private readonly List<Protocol_Preprocessor.BladderminProtocol> books;
        private readonly List<Tuple<string, Protocol_Preprocessor.BladderminProtocol>> library;
        public ProtocolBuffer()
        {
            books = FillLibrary();
            library = SortLibrary(books);
        }
        public List<Protocol_Preprocessor.BladderminProtocol> Books
        {
            get { return books; }
        }
        public List<Tuple<string, Protocol_Preprocessor.BladderminProtocol>> Library
        {
            get { return library; }
        }
        public static List<Protocol_Preprocessor.BladderminProtocol> FillLibrary()
        {
            var books = new List<Protocol_Preprocessor.BladderminProtocol>();
            var eng = new EnumsEngine();
            var ser = new SerializeData();
            var count = eng.F1les.Count;
            for (int i = 0; i < count; i++)
            {
                books.Add(ser.SerializeProtocol(eng.F1les.ToArray()[i]));
            }
            return books;
        }
        public static List<Tuple<string, Protocol_Preprocessor.BladderminProtocol>> SortLibrary(List<Protocol_Preprocessor.BladderminProtocol> books)
        {
            var library = new List<Tuple<string, Protocol_Preprocessor.BladderminProtocol>>();
            foreach (var book in books)
            {
                var name = from att in book.ProtocolMetaData
                           select att.Name;
                library.Add(Tuple.Create(name.FirstOrDefault(), book));
            }
            return library;
        }
    }
}
