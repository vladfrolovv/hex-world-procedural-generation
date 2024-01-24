namespace Core.Editor.AssetPostprocessors
{
    public interface IAssetProcessor
    {
        bool ShouldProcessAsset(UnityAssetsProcessor.AssetChange assetChange);
        void ProcessAsset(UnityAssetsProcessor.AssetChange assetChange);

        void Finish();
    }
}
