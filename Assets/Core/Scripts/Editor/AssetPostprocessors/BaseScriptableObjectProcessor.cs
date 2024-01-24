#region

using Core.Runtime;
using Core.Runtime.Base;
using UnityEditor;

#endregion

namespace Core.Editor.AssetPostprocessors
{
    [InitializeOnLoad]
    public class BaseScriptableObjectProcessor : BaseAssetProcessor<BaseScriptableObject>
    {
        static BaseScriptableObjectProcessor()
        {
            UnityAssetsProcessor.AddProcessor(new BaseScriptableObjectProcessor());
        }


        protected override string AssetFolder => $"{GameFolderConstant.GameFolder}Data/Runtime";

        protected override string AssetExtension => ".asset";


        protected override void OnAssetImported(BaseScriptableObject asset, string assetPath)
        {
            base.OnAssetImported(asset, assetPath);

            string actualGUID = AssetDatabase.AssetPathToGUID(assetPath);
            BaseScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<BaseScriptableObject>(assetPath);
            SerializedObject serialized = new SerializedObject(scriptableObject);
            SerializedProperty guidProperty = serialized.FindProperty("_guid");
            if (guidProperty.stringValue != actualGUID)
            {
                guidProperty.stringValue = actualGUID;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(scriptableObject);
            }
        }
    }
}
