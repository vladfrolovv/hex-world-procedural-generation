#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Runtime.Synchronization.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace Core.Runtime.Synchronization.Serializers
{
    public class JsonNodeSerializer : INodeSerializer
    {
        private Formatting _formatting;


        public JsonNodeSerializer(bool isFormatted)
        {
            _formatting = isFormatted ? Formatting.Indented : Formatting.None;
        }


        public JsonNodeSerializer()
        {
#if !OPTIMIZATION
            {
                _formatting = Formatting.Indented;
            }
#endif
        }


        public void Serialize(Stream stream, Node node)
        {
            JToken t = CreateToken(node);
            using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.Formatting = _formatting;
                    t.WriteTo(jsonWriter);
                }
            }
        }


        public Node Deserialize(Stream stream)
        {
            JToken t;

            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    t = JToken.ReadFrom(jsonReader);
                }
            }

            return CreateNode(t);
        }


        public Node Deserialize(string json)
        {
            return CreateNode(JToken.Parse(json));
        }


        public JToken CreateToken(Node node)
        {
            if (node is ValueNode<bool> boolNode)
            {
                return new JValue(boolNode.Value);
            }

            if (node is ValueNode<int> intNode)
            {
                return new JValue(intNode.Value);
            }

            if (node is ValueNode<long> longNode)
            {
                return new JValue(longNode.Value);
            }

            if (node is ValueNode<float> floatNode)
            {
                return new JValue(floatNode.Value);
            }

            if (node is ValueNode<string> stringNode)
            {
                return new JValue(stringNode.Value);
            }

            if (node is ValueNode<ulong> ulongNode)
            {
                return new JValue(ulongNode.Value);
            }

            if (node is NullNode)
            {
                return JValue.CreateNull();
            }

            if (node is DictionaryNode dictionary)
            {
                JObject obj = new JObject();
                foreach (Node n in dictionary)
                {
                    obj[n.Id] = CreateToken(n);
                }

                return obj;
            }

            if (node is ListNode list)
            {
                JArray obj = new JArray();
                foreach (Node n in list)
                {
                    obj.Add(CreateToken(n));
                }

                return obj;
            }

            throw new InvalidOperationException("Invalid node type.");
        }


        private Node CreateNode(JToken token)
        {
            JObject jobject = token as JObject;
            if (jobject != null)
            {
                DictionaryNode dictionary = new DictionaryNode();
                foreach (KeyValuePair<string, JToken> pair in jobject)
                {
                    AddTokenToDictionary(dictionary, pair.Key, pair.Value);
                }

                return dictionary;
            }

            JArray jarray = token as JArray;
            if (jarray != null)
            {
                ListNode list = new ListNode();
                foreach (JToken t in jarray)
                {
                    AddTokenToList(list, t);
                }

                return list;
            }

            JValue value = token as JValue;
            if (value != null)
            {
                switch (value.Type)
                {
                    case JTokenType.Object:
                        return CreateNode((JObject)value.Value);

                    case JTokenType.Array:
                        return CreateNode((JArray)value.Value);

                    case JTokenType.Integer:
                        return new ValueNode<int>((int)value.Value);

                    case JTokenType.Float:
                        return new ValueNode<float>((float)value.Value);

                    case JTokenType.String:
                        return new ValueNode<string>((string)value.Value);

                    case JTokenType.Boolean:
                        return new ValueNode<bool>((bool)value.Value);

                    case JTokenType.Null:
                        return new NullNode();
                }
            }

            return null;
        }


        private void AddTokenToList(ListNode parent, JToken token)
        {
            JValue value = token as JValue;
            if (value != null)
            {
                switch (value.Type)
                {
                    case JTokenType.Object:
                        AddTokenToList(parent, (JToken)value.Value);
                        return;

                    case JTokenType.Array:
                        AddTokenToList(parent, (JToken)value.Value);
                        break;

                    case JTokenType.Integer:
                        parent.AddInt((int)value);
                        break;

                    case JTokenType.Float:
                        parent.AddFloat((float)value);
                        break;

                    case JTokenType.String:
                        parent.AddString((string)value);
                        break;

                    case JTokenType.Boolean:
                        parent.AddBool((bool)value);
                        break;

                    case JTokenType.Null:
                        parent.AddNull();
                        break;
                }
            }

            JObject jobject = token as JObject;
            if (jobject != null)
            {
                DictionaryNode dictionary = parent.AddDictionary();
                foreach (KeyValuePair<string, JToken> pair in jobject)
                {
                    AddTokenToDictionary(dictionary, pair.Key, pair.Value);
                }

                return;
            }

            JArray jarray = token as JArray;
            if (jarray != null)
            {
                ListNode list = new ListNode();
                foreach (JToken t in jarray)
                {
                    AddTokenToList(list, t);
                }
            }
        }


        private void AddTokenToDictionary(DictionaryNode parent, string id, JToken token)
        {
            JValue value = token as JValue;
            if (value != null)
            {
                switch (value.Type)
                {
                    case JTokenType.Object:
                        AddTokenToDictionary(parent, id, (JToken)value.Value);
                        return;

                    case JTokenType.Array:
                        AddTokenToDictionary(parent, id, (JToken)value.Value);
                        break;

                    case JTokenType.Integer:
                        parent.SetInt(id, (int)value);
                        break;

                    case JTokenType.Float:
                        parent.SetFloat(id, (float)value);
                        break;

                    case JTokenType.String:
                        parent.SetString(id, (string)value);
                        break;

                    case JTokenType.Boolean:
                        parent.SetBool(id, (bool)value);
                        break;

                    case JTokenType.Null:
                        parent.SetNull(id);
                        break;
                }
            }

            JObject jobject = token as JObject;
            if (jobject != null)
            {
                DictionaryNode dictionary = parent.SetDictionary(id);
                foreach (KeyValuePair<string, JToken> pair in jobject)
                {
                    AddTokenToDictionary(dictionary, pair.Key, pair.Value);
                }

                return;
            }

            JArray jarray = token as JArray;
            if (jarray != null)
            {
                ListNode list = parent.SetList(id);
                foreach (JToken t in jarray)
                {
                    AddTokenToList(list, t);
                }
            }
        }
    }
}
