using medicaltraining.assetstore.ScenarioConfiguration.Serialization;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Interface for views that are capable to write data / load data of a pathology variant.
    /// </summary>
    public interface IPathologyVariantView
    {
        public void LoadDataFromVariant(PathologyVariant pathologyVariant);
        
        public void WriteUnhandledChangesToVariant(PathologyVariant pathologyVariant);
    }
}