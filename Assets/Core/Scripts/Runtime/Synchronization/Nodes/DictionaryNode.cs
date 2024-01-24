#region

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Core.Runtime.Synchronization.Nodes
{
    public class DictionaryNode : Node
    {
        private Dictionary<string, Node> _nodes = new Dictionary<string, Node>();


        public DictionaryNode()
        {
        }


        public DictionaryNode(string id) : base(id)
        {
        }


        public int Count => _nodes.Count;


        public IEnumerator<Node> GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }


        public bool GetBool([NotNull] string id)
        {
            return GetValue<bool>(id);
        }


        public bool GetBool([NotNull] string id, bool defaultValue)
        {
            return GetValue(id, defaultValue);
        }


        public void SetBool([NotNull] string id, bool value)
        {
            SetValue(id, value);
        }


        public int GetInt([NotNull] string id)
        {
            return GetValue<int>(id);
        }


        public int GetInt([NotNull] string id, int defaultValue)
        {
            return GetValue(id, defaultValue);
        }


        public void SetInt([NotNull] string id, int value)
        {
            SetValue(id, value);
        }


        public long GetLong([NotNull] string id)
        {
            return GetValue<long>(id);
        }


        public long GetLong([NotNull] string id, long defaultValue)
        {
            return GetValue(id, defaultValue);
        }


        public void SetLong([NotNull] string id, long value)
        {
            SetValue(id, value);
        }


        public float GetFloat([NotNull] string id)
        {
            return GetValue<float>(id);
        }


        public float GetFloat([NotNull] string id, float defaultValue)
        {
            return GetValue(id, defaultValue);
        }


        public void SetFloat([NotNull] string id, float value)
        {
            SetValue(id, value);
        }


        public string GetString([NotNull] string id)
        {
            return GetValue<string>(id);
        }


        public string GetString([NotNull] string id, string defaultValue)
        {
            return GetValue(id, defaultValue);
        }


        public void SetString([NotNull] string id, string value)
        {
            SetValue(id, value);
        }


        public T GetValue<T>([NotNull] string id)
        {
            ValueNode<T>.CheckType(this);
            return GetNode<ValueNode<T>>(id).Value;
        }


        public T GetValue<T>([NotNull] string id, T defaultValue)
        {
            ValueNode<T>.CheckType(this);

            var valueNode = FindNode<ValueNode<T>>(id);
            if (valueNode != null)
            {
                return valueNode.Value;
            }

            return defaultValue;
        }


        public void SetValue<T>([NotNull] string id, T value)
        {
            ValueNode<T>.CheckType(this);

            if (_nodes.TryGetValue(id, out Node node) &&
                node is ValueNode<T> valueNode)
            {
                valueNode.Value = value;
            }
            else
            {
                SetNode(id, new ValueNode<T>(value));
            }
        }


        public bool IsNull([NotNull] string id)
        {
            Node node = GetNode<Node>(id);
            return node is NullNode;
        }


        public void SetNull([NotNull] string id)
        {
            SetNode(id, new NullNode());
        }


        public DictionaryNode GetDictionary([NotNull] string id)
        {
            return GetNode<DictionaryNode>(id);
        }


        public DictionaryNode SetDictionary([NotNull] string id)
        {
            DictionaryNode node = new DictionaryNode();
            SetNode(id, node);
            return node;
        }


        public ListNode GetList([NotNull] string id)
        {
            return GetNode<ListNode>(id);
        }


        public ListNode SetList([NotNull] string id)
        {
            ListNode node = new ListNode();
            SetNode(id, node);
            return node;
        }


        public bool Contains([NotNull] string id)
        {
            return _nodes.ContainsKey(id);
        }


        public Node GetNode([NotNull] string id)
        {
            return GetNode<Node>(id);
        }


        public void SetNode([NotNull] string id, [NotNull] Node node)
        {
            if (node == null)
            {
                throw new NullReferenceException("Trying to add a null node.");
            }

            if (_nodes.TryGetValue(id, out Node oldNode))
            {
                OnNodeDetaching(oldNode);
            }

            _nodes[id] = node;
            OnNodeAttached(node, id);

            node.Id = id;
        }


        public void RemoveNode([NotNull] string id)
        {
            if (_nodes.TryGetValue(id, out Node oldNode))
            {
                OnNodeDetaching(oldNode);
            }

            _nodes.Remove(id);
        }


        public T FindNode<T>([NotNull] string id) where T : Node
        {
            if (_nodes.TryGetValue(id, out Node node))
            {
                if (node is T tNode)
                {
                    return tNode;
                }

                throw new InvalidOperationException($"The node with id '{id}' is '{node.GetType().Name}' but accessed as '{typeof(T).Name}'.");
            }

            return null;
        }


        public T GetNode<T>([NotNull] string id) where T : Node
        {
            if (_nodes.TryGetValue(id, out Node node))
            {
                if (node is T tNode)
                {
                    return tNode;
                }

                throw new InvalidOperationException($"The node with id '{id}' is '{node.GetType().Name}' but accessed as '{typeof(T).Name}'.");
            }

            throw new KeyNotFoundException($"The value with id '{id}' doesn't exist.");
        }


        private void OnNodeAttached(Node node, string id)
        {
            if (node.Parent != null)
            {
                Debug.Log($"Node {node.Id} already has a parent.");
            }

            node.Parent = this;
        }


        private void OnNodeDetaching(Node node)
        {
            node.Parent = null;
            node.Id = null;
        }
    }
}
