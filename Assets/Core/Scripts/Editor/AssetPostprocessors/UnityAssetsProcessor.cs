#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Core.Editor.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Core.Editor.AssetPostprocessors
{
    public class UnityAssetsProcessor : AssetPostprocessor
    {
        private static List<IAssetProcessor> _assetProcessors = new List<IAssetProcessor>();
        private static List<AssetChange> _changedAssets = new List<AssetChange>();
        private static List<AssetChange> _pendingAssets = new List<AssetChange>();


        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                AddChangedAsset(path, null, AssetChangeType.Imported);
            }

            foreach (string path in deletedAssets)
            {
                AddChangedAsset(path, null, AssetChangeType.Deleted);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                AddChangedAsset(movedAssets[i], movedFromAssetPaths[i], AssetChangeType.Moved);
            }

            _pendingAssets.Clear();
        }


        public static void AddProcessor(IAssetProcessor processor)
        {
            _assetProcessors.Add(processor);
        }


        private static AssetChange CreateAssetChange(string path, string oldPath, AssetChangeType changeType)
        {
            return new AssetChange(path, oldPath, changeType);
        }


        private static AssetChange FindAssetChange(List<AssetChange> assets, string path, AssetChangeType changeType)
        {
            return assets.Find(change => change.Path == path && change.ChangeType == changeType);
        }


        private static void AddChangedAsset(string path, string oldPath, AssetChangeType changeType)
        {
            if (!AssetDatabaseUtils.IsProjectAsset(path))
            {
                return;
            }

            if (FindAssetChange(_changedAssets, path, changeType) != null)
            {
                return;
            }

            AssetChange change = FindAssetChange(_pendingAssets, path, changeType);
            if (change == null)
            {
                change = CreateAssetChange(path, oldPath, changeType);
            }

            if (!ShouldProcessAssets(change))
            {
                return;
            }

            _changedAssets.Add(change);

            EditorApplication.delayCall -= ProcessAssets;
            EditorApplication.delayCall += ProcessAssets;
        }


        private static bool ShouldProcessAssets(AssetChange change)
        {
            foreach (IAssetProcessor processor in _assetProcessors)
            {
                if (processor.ShouldProcessAsset(change))
                {
                    return true;
                }
            }

            return false;
        }


        private static void ProcessAssets()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            bool isCancelled = false;

            while (_changedAssets.Count > 0 && !isCancelled)
            {
                AssetDatabase.StartAssetEditing();

                int assetsCount = _changedAssets.Count;
                for (int i = 0; i < assetsCount; i++)
                {
                    AssetChange change = _changedAssets[i];

                    UnityEngine.Debug.Log($"{change.ChangeType} asset at {change.Path}");

                    isCancelled = EditorUtility.DisplayCancelableProgressBar(
                        "Process asset",
                        $"{change.Path} : ({i}/{assetsCount})",
                        (float)i / assetsCount
                    );

                    if (isCancelled)
                    {
                        break;
                    }

                    foreach (IAssetProcessor processor in _assetProcessors)
                    {
                        try
                        {
                            if (processor.ShouldProcessAsset(change))
                            {
                                processor.ProcessAsset(change);
                            }
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError($"Error while processing asset in {processor.GetType()}");
                            UnityEngine.Debug.LogException(e);
                        }
                    }
                }

                EditorUtility.ClearProgressBar();

                foreach (IAssetProcessor processor in _assetProcessors)
                {
                    try
                    {
                        processor.Finish();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"Error while finishing asset processing in {processor.GetType()}");
                        UnityEngine.Debug.LogException(e);
                    }
                }

                _changedAssets.Clear();
                AssetDatabase.StopAssetEditing();
            }

            stopwatch.Stop();
            UnityEngine.Debug.Log($"Asset postprocessing took {stopwatch.ElapsedMilliseconds} ms.");
        }


        #region Inner types

        private class Postprocessor : AssetModificationProcessor
        {
            private static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
            {
                // Optimization: if asset is deleted from Unity we can determine asset type before deletion.
                _pendingAssets.Add(CreateAssetChange(assetName, null, AssetChangeType.Deleted));

                return AssetDeleteResult.DidNotDelete;
            }
        }

        public class AssetChange
        {
            public AssetChange(
                [NotNull] string path,
                [CanBeNull] string oldPath,
                AssetChangeType changeType)
            {
                Path = path;
                OldPath = oldPath;
                ChangeType = changeType;
                MainAssetType = AssetDatabase.GetMainAssetTypeAtPath(path);

                if (MainAssetType != null &&
                    MainAssetType.IsSubclassOf(typeof(ScriptableObject)))
                {
                    SubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                }
            }


            [NotNull]
            public string Path { get; }

            [CanBeNull]
            public string OldPath { get; }

            [CanBeNull]
            public Type MainAssetType { get; }

            [CanBeNull]
            public Object[] SubAssets { get; }

            public AssetChangeType ChangeType { get; }
        }

        public enum AssetChangeType
        {
            Imported,
            Deleted,
            Moved
        }

        #endregion
    }
}
