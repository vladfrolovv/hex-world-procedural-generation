#region

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Core.Runtime.Synchronization.Nodes
{
    public class ListNode : Node
    {
        private List<Node> _nodes = new List<Node>();

        public int Count => _nodes.Count;


        public bool Contains(int index)
        {
            return index >= 0 && index < _nodes.Count;
        }


        public IEnumerator<Node> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }


        public bool GetBool(int index)
        {
            return GetValue<bool>(index);
        }


        public void SetBool(int index, bool value)
        {
            SetValue(index, value);
        }


        public int GetInt(int index)
        {
            return GetValue<int>(index);
        }


        public void SetInt(int index, int value)
        {
            SetValue(index, value);
        }


        public float GetFloat(int index)
        {
            return GetValue<float>(index);
        }


        public void SetFloat(int index, float value)
        {
            SetValue(index, value);
        }


        public string GetString(int index)
        {
            return GetValue<string>(index);
        }


        public void SetString(int index, string value)
        {
            SetValue(index, value);
        }


        public bool IsNull(int index)
        {
            Node node = GetNode<Node>(index);
            return node is NullNode;
        }


        public void SetNull(int index)
        {
            Node node = GetNode<Node>(index);
            if (!(node is NullNode))
            {
                SetNode(index, new NullNode());
            }
        }


        public DictionaryNode GetDictionary(int index)
        {
            return GetNode<DictionaryNode>(index);
        }


        public DictionaryNode SetDictionary(int index)
        {
            DictionaryNode node = new DictionaryNode();
            SetNode(index, node);
            return node;
        }


        public ListNode GetList(int index)
        {
            return GetNode<ListNode>(index);
        }


        public ListNode SetList(int index)
        {
            ListNode node = new ListNode();
            SetNode(index, node);
            return node;
        }


        public ListNode AddList()
        {
            ListNode node = new ListNode();
            AddNode(node);
            return node;
        }


        public DictionaryNode AddDictionary()
        {
            DictionaryNode node = new DictionaryNode();
            AddNode(node);
            return node;
        }


        public void AddBool(bool value)
        {
            AddValue(value);
        }


        public void AddInt(int value)
        {
            AddValue(value);
        }


        public void AddFloat(float value)
        {
            AddValue(value);
        }


        public void AddString(string value)
        {
            AddValue(value);
        }


        public void AddValue<T>(T value)
        {
            AddNode(new ValueNode<T>(value));
        }


        public void AddNull()
        {
            AddNode(new NullNode());
        }


        public T GetValue<T>(int index)
        {
            ValueNode<T>.CheckType(this);
            return GetNode<ValueNode<T>>(index).Value;
        }


        public void SetValue<T>(int index, T value)
        {
            ValueNode<T>.CheckType(this);

            if (GetNode<Node>(index) is ValueNode<T> node)
            {
                node.Value = value;
            }
            else
            {
                SetNode(index, new ValueNode<T>(value));
            }
        }


        public Node GetNode(int index)
        {
            return GetNode<Node>(index);
        }


        public T GetNode<T>(int index) where T : Node
        {
            if (!Contains(index))
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range.");
            }

            Node node = _nodes[index];
            if (!(node is T))
            {
                throw new InvalidOperationException(
                    $"Invalid node type at {index}. The requiested type is '{typeof(T).Name}' but the node is '{node.GetType().Name}'.");
            }

            return (T)node;
        }


        public void AddNode([NotNull] Node node)
        {
            if (node == null)
            {
                throw new NullReferenceException("Trying to add a null node.");
            }

            _nodes.Add(node);
            OnNodeAttached(node, _nodes.Count - 1);
        }


        public void RemoveNode(int index)
        {
            OnNodeDetaching(_nodes[index]);
            _nodes.RemoveAt(index);
        }


        private void OnNodeAttached(Node node, int index)
        {
            if (node.Parent != null)
            {
                Debug.Log("Node {node.Id} already has a parent.");
            }

            node.Parent = this;
        }


        private void OnNodeDetaching(Node node)
        {
            node.Parent = null;
        }


        private void SetNode(int index, [NotNull] Node node)
        {
            if (!Contains(index))
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range.");
            }

            if (node == null)
            {
                throw new NullReferenceException("Trying to add a null node.");
            }

            OnNodeDetaching(_nodes[index]);
            _nodes[index] = node;
            OnNodeAttached(node, index);
        }
    }
}
