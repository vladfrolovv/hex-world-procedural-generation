#region

using System;
using System.Globalization;
using Core.Runtime.Synchronization.Nodes;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    internal class DateTimeSynchronizer : ISynchronizer<DateTime>
    {
        public virtual Node Serialize(DateTime obj)
        {
            return new ValueNode<string>(obj.ToString(CultureInfo.InvariantCulture));
        }


        public virtual DateTime Deserialize(Node node)
        {
            string str = ((ValueNode<string>)node).Value;
            return DateTime.Parse(str, CultureInfo.InvariantCulture);
        }
    }
}
