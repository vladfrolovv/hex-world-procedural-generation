#region

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using Object = UnityEngine.Object;

#endregion

namespace Core.Editor.AssetPostprocessors
{
    public abstract class BaseAssetProcessor<TAsset> : IAssetProcessor
        where TAsset : Object
    {


        protected virtual string AssetFolder => "Assets";

        protected abstract string AssetExtension { get; }
        public void ProcessAsset(UnityAssetsProcessor.AssetChange assetChange)
        {
            switch (assetChange.ChangeType)
            {
                case UnityAssetsProcessor.AssetChangeType.Imported:
                {
                    List<TAsset> changedAssets = LoadChangedAssets(assetChange);
                    if (changedAssets != null)
                    {
                        foreach (TAsset asset in changedAssets)
                        {
                            OnAssetImported(asset, assetChange.Path);
                        }
                    }

                    break;
                }

                case UnityAssetsProcessor.AssetChangeType.Deleted:
                {
                    OnAssetDeleted(assetChange.Path);
                    break;
                }

                case UnityAssetsProcessor.AssetChangeType.Moved:
                {
                    List<TAsset> changedAssets = LoadChangedAssets(assetChange);
                    if (changedAssets != null)
                    {
                        foreach (TAsset asset in changedAssets)
                        {
                            OnAssetMoved(asset, assetChange.Path, assetChange.OldPath);
                        }
                    }

                    break;
                }
            }
        }


        public bool ShouldProcessAsset(UnityAssetsProcessor.AssetChange assetChange)
        {
            string path = assetChange.Path;
            if (!path.StartsWith(AssetFolder) || !path.EndsWith(AssetExtension))
            {
                return false;
            }

            Type assetType = assetChange.MainAssetType;

            if (assetType == null)
            {
                return true;
            }

            Type processingType = typeof(TAsset);
            if (assetType == processingType || assetType.IsSubclassOf(processingType))
            {
                return true;
            }

            if (assetChange.SubAssets != null)
            {
                foreach (Object subAsset in assetChange.SubAssets)
                {
                    if (subAsset is TAsset)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public virtual void Finish()
        {
        }


        protected virtual void OnAssetImported([NotNull] TAsset asset, string assetPath)
        {
        }


        protected virtual void OnAssetDeleted(string assetPath)
        {
        }


        protected virtual void OnAssetMoved([NotNull] TAsset asset, string assetPath, string oldAssetPath)
        {
        }


        private List<TAsset> LoadChangedAssets(UnityAssetsProcessor.AssetChange assetChange)
        {
            Type processingType = typeof(TAsset);
            Type mainAssetType = assetChange.MainAssetType;

            if (mainAssetType == null)
            {
                return null;
            }

            var changedAssets = new List<TAsset>(1);

            if (mainAssetType == processingType || mainAssetType.IsSubclassOf(processingType))
            {
                changedAssets.Add((TAsset)AssetDatabase.LoadMainAssetAtPath(assetChange.Path));
            }

            if (assetChange.SubAssets != null)
            {
                foreach (Object subAsset in assetChange.SubAssets)
                {
                    if (subAsset is TAsset tAsset)
                    {
                        changedAssets.Add(tAsset);
                    }
                }
            }

            return changedAssets;
        }
    }
}
