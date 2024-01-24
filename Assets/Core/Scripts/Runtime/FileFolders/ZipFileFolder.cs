#region

using System;
using System.IO;
using System.IO.Compression;

#endregion

namespace Core.Runtime.FileFolders
{
    public class ZipFileFolder : BaseFileFolder
    {
        private ZipArchive _archive;
        private MemoryStream _stream;


        public ZipFileFolder(ZipArchiveMode mode, byte[] buffer = null)
        {
            _stream = buffer == null ? new MemoryStream() : new MemoryStream(buffer, mode != ZipArchiveMode.Read);

            _archive = new ZipArchive(_stream, mode);
        }


        public override void Dispose()
        {
            base.Dispose();

            DisposeArchive();

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }


        public void Dispose(out byte[] bytes)
        {
            bytes = null;

            // Archive must be disposed before accessing the buffer.
            DisposeArchive();

            ArraySegment<byte> segment;
            if (_stream.TryGetBuffer(out segment))
            {
                bytes = segment.Array;
                if (segment.Offset != 0 || segment.Count != bytes.Length)
                {
                    bytes = null;
                }
            }

            bytes = bytes ?? _stream.ToArray();

            Dispose();
        }


        protected override bool ExistsImpl(string path)
        {
            return _archive.GetEntry(path) != null;
        }


        protected override Stream OpenReadImpl(string path)
        {
            return _archive.GetEntry(path).Open();
        }


        protected override Stream OpenWriteImpl(string path)
        {
            return _archive.CreateEntry(path).Open();
        }


        protected override void DeleteImpl(string path)
        {
            _archive.GetEntry(path).Delete();
        }


        private void DisposeArchive()
        {
            if (_archive != null)
            {
                _archive.Dispose();
                _archive = null;
            }
        }
    }
}
