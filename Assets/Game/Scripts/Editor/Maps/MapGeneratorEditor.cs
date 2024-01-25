#region

using Game.Runtime.Maps;
using UnityEditor;
using UnityEngine;

#endregion

namespace Game.Editor.Maps
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        private MapGenerator _mapGenerator;

        protected void OnEnable()
        {
            _mapGenerator = target as MapGenerator;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // used to modify range
            // EditorGUILayout.PropertyField(MapSize);
            // PerlinScale.floatValue = Mathf.Min(MapSize.vector2IntValue.y, PerlinScale.floatValue);
            // EditorGUILayout.Slider(PerlinScale, 0.01f, MapSize.vector2IntValue.y);

            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate New"))
            {
                _mapGenerator.Generate();
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
