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

            GUILayout.Space(25);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate New"))
            {
                _mapGenerator.Generate();
            }

            GUILayout.EndHorizontal();
        }
    }
}
