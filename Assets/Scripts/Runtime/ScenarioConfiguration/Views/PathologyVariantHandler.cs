using System.Collections.Generic;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Common;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Accompanies a view that needs to be able to switch between pathology variants.
    /// Handles actions regarding the list of available pathology variants and forwards
    /// the appropriate actions to its associated view
    /// </summary>
    public class PathologyVariantHandler : MonoBehaviour
    {
        [SerializeField] protected PathologyVariantList _pathologyVariantList;
        [SerializeField] protected PathologyVariantManager _pathologyVariantManager;
        [SerializeField] protected View _pathologyVariantViewGameObject;
        protected IPathologyVariantView _pathologyVariantView;
        private PathologyVariant _loadedPathologyVariant;

        public void Initialize()
        {
            _pathologyVariantView = _pathologyVariantViewGameObject as IPathologyVariantView;
        }

        #region Events emitted by the PathologyVariantManager

        public virtual void OnActiveVariantChanged(PathologyVariant pathologyVariant)
        {
            _pathologyVariantList.SetActiveItem(pathologyVariant);
            _pathologyVariantView.LoadDataFromVariant(pathologyVariant);
            _loadedPathologyVariant = pathologyVariant;
        }

        public virtual void OnAvailableVariantsChanged(List<PathologyVariant> pathologyVariants)
        {
            _pathologyVariantList.ReplaceItems(pathologyVariants);
        }

        #endregion

        #region Events emitted by the UI Actions in the Variant List

        public void OnVariantSelectRequested(PathologyVariant pathologyVariant)
        {
            _pathologyVariantManager.SetActivePathologyVariant(pathologyVariant);
        }

        public void OnVariantRemoveRequested(PathologyVariant pathologyVariant)
        {
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.DeleteVariant,
                onConfirm: () => _pathologyVariantManager.RemovePathologyVariant(pathologyVariant));
        }

        public void OnVariantCreateRequested()
        {
            var newActiveVariant = _pathologyVariantManager.CreatePathologyVariant();
            _pathologyVariantManager.SetActivePathologyVariant(newActiveVariant);
        }

        #endregion

        public void WriteDataToActivePathologyVariant()
        {
            _pathologyVariantView.WriteUnhandledChangesToVariant(_loadedPathologyVariant);
        }
    }
}