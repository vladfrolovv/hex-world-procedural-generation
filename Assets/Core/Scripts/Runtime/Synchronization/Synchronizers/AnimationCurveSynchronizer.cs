#region

using Core.Runtime.Synchronization.Nodes;
using UnityEngine;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    public class AnimationCurveSynchronizer : ISynchronizer<AnimationCurve>
    {
        private const string _keyframeInTangentId = "inTangent";
        private const string _keyframeInWeightId = "inWeight";
        private const string _keyframeOutTangentId = "outTangent";
        private const string _keyframeOutWeightId = "outWeight";
        private const string _keyframesListId = "keyframes";
        private const string _keyframeTimeId = "time";
        private const string _keyframeValueId = "value";
        private const string _keyframeWeightedModeId = "weightedMode";

        public Node Serialize(AnimationCurve curve)
        {
            DictionaryNode dic = new DictionaryNode();
            {
                ListNode list = dic.SetList(_keyframesListId);
                foreach (Keyframe keyframe in curve.keys)
                {
                    DictionaryNode keyframeDic = list.AddDictionary();
                    {
                        keyframeDic.SetFloat(_keyframeTimeId, keyframe.time);
                        keyframeDic.SetFloat(_keyframeValueId, keyframe.value);
                        keyframeDic.SetFloat(_keyframeInTangentId, keyframe.inTangent);
                        keyframeDic.SetFloat(_keyframeOutTangentId, keyframe.outTangent);
                        keyframeDic.SetFloat(_keyframeInWeightId, keyframe.inWeight);
                        keyframeDic.SetFloat(_keyframeOutWeightId, keyframe.outWeight);
                        keyframeDic.SetInt(_keyframeWeightedModeId, (int)keyframe.weightedMode);
                    }
                }
            }

            return dic;
        }

        public AnimationCurve Deserialize(Node node)
        {
            DictionaryNode dic = (DictionaryNode)node;
            ListNode listNode = dic.GetList(_keyframesListId);

            var keyframes = new Keyframe[listNode.Count];
            for (int i = 0; i < listNode.Count; ++i)
            {
                DictionaryNode keyframeDic = listNode.GetDictionary(i);
                {
                    keyframes[i] = new Keyframe
                    {
                        time = keyframeDic.GetFloat(_keyframeTimeId),
                        value = keyframeDic.GetFloat(_keyframeValueId),
                        inTangent = keyframeDic.GetFloat(_keyframeInTangentId),
                        outTangent = keyframeDic.GetFloat(_keyframeOutTangentId),
                        inWeight = keyframeDic.GetFloat(_keyframeInWeightId),
                        outWeight = keyframeDic.GetFloat(_keyframeOutWeightId),
                        weightedMode = (WeightedMode)keyframeDic.GetInt(_keyframeWeightedModeId)
                    };
                }
            }

            return new AnimationCurve(keyframes);
        }
    }
}
