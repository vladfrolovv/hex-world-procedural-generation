#region

using System.IO;

#endregion

namespace Core.Runtime.FileFolders
{
    public class FileFolder : BaseFileFolder
    {
        protected override bool ExistsImpl(string path)
        {
            return File.Exists(path);
        }


        protected override Stream OpenReadImpl(string path)
        {
            return File.OpenRead(path);
        }


        protected override Stream OpenWriteImpl(string path)
        {
            return File.OpenWrite(path);
        }


        protected override void DeleteImpl(string path)
        {
            File.Delete(path);
        }
    }
}
