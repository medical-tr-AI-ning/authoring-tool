using System.Collections.Generic;
using System.IO;
using System.Linq;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.ScenarioConfiguration.Views;
using Runtime.ScenarioConfiguration.Views.Agent;
using Runtime.ScenarioConfiguration.Views.Anamnesis;
using Runtime.ScenarioConfiguration.Views.Pathologies;
using UnityEngine;

namespace Runtime.ScenarioConfiguration
{
    /// <summary>
    /// Manages creation, switching and changes to pathology variants which are used by
    /// the anamnesis and pathologies view. Also handles whether all pathology variants should use
    /// the same (unified) anamnesis data or each pathology variant has its own custom anamnesis data.
    public class PathologyVariantManager : MonoBehaviour
    {
        public PathologyVariant ActivePathologyVariant { get; private set; }
        private List<PathologyVariant> _pathologyVariants = new List<PathologyVariant>();
        private List<PathologyVariantHandler> _variantHandlers;
        public bool UseUnifiedAnamnesisData { get; private set; }

        [SerializeField] public bool UseUnifiedAnamnesisDataDefaultValue;
        [SerializeField] private AgentView _agentView;

        private void Awake()
        {
            _variantHandlers = getVariantHandlers();

            // TODO selected variant cant be changed after current agent is changed pls fix!
            _agentView.SelectedAgentChanged += () =>
            {
                _pathologyVariants.Clear();
                CreatePathologyVariant();
                SetActivePathologyVariant(_pathologyVariants.First());
            };
        }

        public void LoadEmptyConfiguration()
        {
            CreatePathologyVariant();
            SetActivePathologyVariant(_pathologyVariants.First());
            SetUseUnifiedAnamnesisData(UseUnifiedAnamnesisDataDefaultValue);
        }

        public AgentData GetCurrentAgentData() => _agentView.GetCurrentAgentData();

        public bool IsUnifiedAnamnesisDataPathologyVariant(PathologyVariant pathologyVariant)
        {
            return pathologyVariant == GetUnifiedAnamnesisDataPathologyVariant();
        }

        public PathologyVariant GetUnifiedAnamnesisDataPathologyVariant() => _pathologyVariants.First();

        public void SetUseUnifiedAnamnesisData(bool useUnifiedAnamnesisData)
        {
            saveActivePathologyVariantUserData();
            UseUnifiedAnamnesisData = useUnifiedAnamnesisData;
            notifyUseUnifiedAnamnesisDataChanged();
        }

        public void SetActivePathologyVariant(PathologyVariant pathologyVariant)
        {
            if (pathologyVariant == ActivePathologyVariant)
            {
                Debug.Log("Pathology variant was already selected!");
                return;
            }

            // Save user input before switching to different pathology variant
            if (ActivePathologyVariant != null)
                saveActivePathologyVariantUserData();

            if (!_pathologyVariants.Contains(pathologyVariant))
            {
                Debug.LogError("Trying to select an unknown pathology variant!");
                return;
            }

            ActivePathologyVariant = pathologyVariant;
            foreach (PathologyVariantHandler variantHandler in _variantHandlers)
            {
                variantHandler.OnActiveVariantChanged(ActivePathologyVariant);
            }
        }

        public PathologyVariant CreatePathologyVariant()
        {
            var pathologyVariant = new PathologyVariant();
            _pathologyVariants.Add(pathologyVariant);
            notifyPathologyVariantListChanged();
            return pathologyVariant;
        }

        public void LoadConfiguration(List<SerializablePathologyVariant> serializablePathologyVariants,
            string resourcesFolder)
        {
            if (serializablePathologyVariants.Count == 0)
            {
                Debug.LogError("Configuration must contain at least one pathology variant!");
                return;
            }

            _pathologyVariants = new List<PathologyVariant>();
            int variant_index = 0;
            foreach (var serializablePathologyVariant in serializablePathologyVariants)
            {
                string variantPath = Path.Combine(resourcesFolder, FolderStructure.PathologyVariant(variant_index));
                PathologyVariant pathologyVariant = serializablePathologyVariant.Deserialize(variantPath);
                _pathologyVariants.Add(pathologyVariant);
                variant_index++;
            }

            notifyPathologyVariantListChanged();
            SetActivePathologyVariant(_pathologyVariants.First());
        }

        public void RemovePathologyVariant(PathologyVariant pathologyVariant)
        {
            if (_pathologyVariants.Count <= 1)
            {
                Debug.LogError("Cannot remove last remaining pathology variant!");
                return;
            }

            _pathologyVariants.Remove(pathologyVariant);
            notifyPathologyVariantListChanged();
            if (pathologyVariant == ActivePathologyVariant)
            {
                ActivePathologyVariant = _pathologyVariants.First();
            }

            notifyActivePathologyVariantChanged();
        }

        private void saveActivePathologyVariantUserData()
        {
            foreach (PathologyVariantHandler variantHandler in _variantHandlers)
            {
                variantHandler.WriteDataToActivePathologyVariant();
            }
        }

        private void notifyPathologyVariantListChanged()
        {
            foreach (PathologyVariantHandler variantHandler in _variantHandlers)
                variantHandler.OnAvailableVariantsChanged(_pathologyVariants);
        }

        private void notifyActivePathologyVariantChanged()
        {
            foreach (PathologyVariantHandler variantHandler in _variantHandlers)
                variantHandler.OnActiveVariantChanged(ActivePathologyVariant);
        }

        private void notifyUseUnifiedAnamnesisDataChanged()
        {
            foreach (PathologyVariantHandler variantHandler in _variantHandlers)
                if (variantHandler is AnamnesisPathologyVariantHandler anamnesisPathologyVariantHandler)
                    anamnesisPathologyVariantHandler.OnUseUnifiedAnamnesisDataChanged(UseUnifiedAnamnesisData);
        }

        private List<PathologyVariantHandler> getVariantHandlers()
        {
            return FindObjectsOfType<PathologyVariantHandler>(includeInactive: true).ToList();
        }

        public List<PathologyVariant> GetPathologyVariants()
        {
            saveActivePathologyVariantUserData();
            return _pathologyVariants;
        }
    }
}