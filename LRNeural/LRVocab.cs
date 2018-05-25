using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Word2vec.Tools;

namespace MRNeural
{
    public class LRVocab
    {
        public Vocabulary Vocabulary;

        public bool IsReady { get; set; } = false;
        public Exception Error { get; set; }

        protected string _path { get; set; }
        public string Path => _path;

        public LRVocab Create(string path, LRVocabReadProgress progress = null)
        {
            IsReady = false;
            Error = null;

            try
            {
                if(progress != null)
                {
                    progress.Invoke("Start uploading vocabulary");
                }

                _path = path;
                using (var stream = File.OpenRead(path))
                {
                    Vocabulary = new Word2VecBinaryReader().Read(stream);
                }

                if(progress != null)
                {
                    progress.Invoke("Vocabulary uploaded");
                }

                IsReady = true;
            }
            catch(Exception e)
            {
                Error = e;

                if(progress != null)
                {
                    progress.Invoke($"Vocabulary upload error: {e.Message}");
                }
            }
            
            return this;
        }
    }

    public delegate void LRVocabReadProgress(string log);

}
