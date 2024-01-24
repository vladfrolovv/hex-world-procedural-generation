#region

using System.Collections.Generic;
using System.IO;

#endregion

namespace Core.Runtime.FileFolders
{
    public abstract class BaseFileFolder : IFileFolder
    {
        private Stack<string> _pathes = new Stack<string>();


        public void PushPath(string path)
        {
            _pathes.Push(path);
        }


        public void PopPath()
        {
            _pathes.Pop();
        }


        public bool Exists(string path)
        {
            return ExistsImpl(GetFullPath(path));
        }


        public Stream OpenRead(string path)
        {
            return OpenReadImpl(GetFullPath(path));
        }


        public Stream OpenWrite(string path)
        {
            return OpenWriteImpl(GetFullPath(path));
        }


        public void Delete(string path)
        {
            DeleteImpl(GetFullPath(path));
        }


        public virtual void Dispose()
        {
        }


        protected abstract bool ExistsImpl(string path);
        protected abstract Stream OpenReadImpl(string path);
        protected abstract Stream OpenWriteImpl(string path);
        protected abstract void DeleteImpl(string path);


        protected string GetFullPath(string path)
        {
            if (_pathes.Count == 0)
            {
                return path;
            }

            return Path.Combine(_pathes.Peek(), path);
        }
    }
}
