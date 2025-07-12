using System.Collections.Generic;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Common;
using Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views.Anamnesis
{
    /// <summary>
    /// A PathologyVariantHandler which implements additional
    /// functionality to use a unified anamnesis data object for
    /// all pathology variants
    /// </summary>
    public class AnamnesisPathologyVariantHandler : PathologyVariantHandler
    {
        [SerializeField] public ToggleButton UseUnifiedAnamnesisDataToggle;
        [SerializeField] public Button CopyDataFromFirstVariantButton;
        [SerializeField] private GameObject[] _activeOnMultipleVariants;

        #region Events emitted by the PathologyVariantManager

        public override void OnActiveVariantChanged(PathologyVariant pathologyVariant)
        {
            // Load Unified 
            PathologyVariant variantToLoad = _pathologyVariantManager.UseUnifiedAnamnesisData
                ? _pathologyVariantManager.GetUnifiedAnamnesisDataPathologyVariant()
                : pathologyVariant;
            base.OnActiveVariantChanged(variantToLoad);

            //Only Display "Copy from default variant" button if more than one variant exists.
            bool displayCopyButton = !_pathologyVariantManager.IsUnifiedAnamnesisDataPathologyVariant(pathologyVariant);
            CopyDataFromFirstVariantButton.gameObject.SetActive(displayCopyButton);
        }

        public override void OnAvailableVariantsChanged(List<PathologyVariant> pathologyVariants)
        {
            base.OnAvailableVariantsChanged(pathologyVariants);

            bool displayUseUnifiedAnamnesisDataToggle = pathologyVariants.Count > 1;
            UseUnifiedAnamnesisDataToggle.gameObject.SetActive(displayUseUnifiedAnamnesisDataToggle);

            // TODO should there be a references to these ui elements here?
            foreach (var g in _activeOnMultipleVariants)
            {
                g.SetActive(displayUseUnifiedAnamnesisDataToggle);
            }
        }

        public void OnUseUnifiedAnamnesisDataChanged(bool useUnifiedAnamnesisData)
        {
            var variantToLoad = useUnifiedAnamnesisData
                ? _pathologyVariantManager.GetUnifiedAnamnesisDataPathologyVariant()
                : _pathologyVariantManager.ActivePathologyVariant;
            base.OnActiveVariantChanged(variantToLoad);

            if (UseUnifiedAnamnesisDataToggle == null) return;
            UseUnifiedAnamnesisDataToggle.SetToggleState(!useUnifiedAnamnesisData);
        }

        #endregion

        public void OnSetUseUnifiedAnamnesisDataRequested(bool useUnifiedAnamnesisData)
        {
            _pathologyVariantManager.SetUseUnifiedAnamnesisData(useUnifiedAnamnesisData);
        }

        public void OnCopyAnamnesisDataFromDefaultPathologyVariantRequested()
        {
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.CopyAnamnesisData, onConfirm: () =>
                _pathologyVariantView.LoadDataFromVariant(
                    _pathologyVariantManager.GetUnifiedAnamnesisDataPathologyVariant()));
        }
    }
}