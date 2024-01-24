#region

using System.IO;
using Core.Runtime.FileFolders;
using Core.Runtime.Synchronization.Nodes;
using Core.Runtime.Synchronization.Serializers;
using Core.Runtime.Synchronization.States;

#endregion

namespace Core.Runtime.Saves
{
    public class SerializationHelper
    {
        private IFileFolder _folder;
        private INodeSerializer _serializer;


        public SerializationHelper(IFileFolder folder, INodeSerializer serializer)
        {
            _folder = folder;
            _serializer = serializer;
        }


        public IFileFolder Folder => _folder;


        public DeserializationState LoadStateFromFile(string path)
        {
            DictionaryNode root = LoadRootNodeFromFile(path);
            return root == null ? null : new DeserializationState(root);
        }


        public DictionaryNode LoadRootNodeFromFile(string path)
        {
            DictionaryNode result = null;
            if (_folder.Exists(path))
            {
                using (Stream stream = _folder.OpenRead(path))
                {
                    result = (DictionaryNode)_serializer.Deserialize(stream);
                }
            }

            return result;
        }


        public void SaveStateToFile(SerializationState state, string path)
        {
            SaveRootNodeToFile(state.Root, path);
        }


        public void SaveRootNodeToFile(DictionaryNode root, string path)
        {
            using (Stream stream = _folder.OpenWrite(path))
            {
                _serializer.Serialize(stream, root);
            }
        }
    }
}
