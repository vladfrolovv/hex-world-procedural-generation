namespace Core.Runtime.Synchronization.Nodes
{
    public abstract class Node
    {
        private string _id;


        protected Node()
        {
        }


        protected Node(string id)
        {
            Id = id;
        }

        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                }
            }
        }

        public Node Parent { get; set; }
    }
}
