using System;
using System.Collections.Generic;
using System.Text;
using Word2vec.Tools;

namespace MRNeural.Tools
{
    public class VReader
    {
        public string Path { get; set; }
        public Exception Exception { get; set; }
        public Vocabulary Vocab { get; set; }

        public bool IsReady => Exception == null && Vocab != null;

        public VReader() { }
        public VReader(string path)
        {
            Path = path;
        }

        public bool UploadBinary(string path = null)
        {
            path = string.IsNullOrWhiteSpace(path) ? Path : path;

            if (string.IsNullOrWhiteSpace(Path))
                return false;

            try
            {
                Vocab = new Word2VecBinaryReader().Read(path);
            }
            catch(Exception ex)
            {
                Exception = ex;
                return false;
            }

            return true;
        }

        public bool UploadText(string path = null)
        {
            path = string.IsNullOrWhiteSpace(path) ? Path : path;

            if (string.IsNullOrWhiteSpace(Path))
                return false;

            try
            {
                Vocab = new Word2VecTextReader().Read(path);
            }
            catch (Exception ex)
            {
                Exception = ex;
                return false;
            }

            return true;
        }
    }
}
