#region

using System;
using System.IO;

#endregion

namespace Core.Runtime.FileFolders
{
    public interface IFileFolder : IDisposable
    {
        void PushPath(string path);
        void PopPath();

        bool Exists(string path);
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
        void Delete(string path);
    }
}
