using System.Collections.Generic;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Common;
using Runtime.Common.Prompts;
using Runtime.ScenarioConfiguration.Views.Agent;
using Runtime.UI;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    
    /// <summary>
    /// /// <summary>
    /// Handles the "Pathologien" view UI and logic.
    /// </summary>
    /// </summary>
    public class PathologiesView : PresetConfigurationView<SerializablePathologyVariant.SerializablePathologyData>,
        IPathologyVariantView
    {
        [SerializeField] private Display3D _display3D;
        [SerializeField] private RawImage previewImageRaw;
        [SerializeField] private Material OverlayMaterial;
        [SerializeField] private GameObject PlacingHint;
        [SerializeField] private MelanomaUIList _melanomaUIList;
        [SerializeField] private MelanomaConfigurator _melanomaConfigurator;
        [SerializeField] private NaeviConfigurator _naeviConfigurator;
        [SerializeField] private Toggle _naeviEnabledToggle;
        [SerializeField] private Toggle _melanomaEnabledToggle;
        [SerializeField] private PathologyVariantList _pathologyVariantList;
        [SerializeField] private PathologyVariantManager _pathologyVariantManager;
        [SerializeField] private PathologyVariantHandler _pathologyVariantHandler;
        [SerializeField] private AgentCameraController _camera;
        [SerializeField] private GetPointOnRendertexture _3DMarker;

        private LesionManager _lesionManager;
        private PathologyDataConfigurator _pathologyDataConfigurator;
        private SetCursor _cursor;
        private bool _placing = false;

        public override void Initialize()
        {
            _cursor = FindObjectOfType<SetCursor>();
            _melanomaUIList = GetComponentInChildren<MelanomaUIList>(includeInactive: true);
            _naeviConfigurator.NaeviElevationUpdate.AddListener(OnNaeviUpdate);
            _pathologyDataConfigurator = new PathologyDataConfigurator();
            _lesionManager = new LesionManager(OverlayMaterial, _melanomaConfigurator, _pathologyDataConfigurator);
            _naeviEnabledToggle.onValueChanged.AddListener(OnNaeviEnabledChanged);
            _melanomaEnabledToggle.onValueChanged.AddListener(OnMelanomaEnabledChanged);
            _pathologyVariantHandler.Initialize();

            _3DMarker.gameObject.SetActive(false);
            _3DMarker.Pickup.AddListener(() => { SetPlacing(true); _3DMarker.gameObject.SetActive(false); });
            _3DMarker.Delete.AddListener(() => { RequestDeleteMelanoma(_melanomaConfigurator.Selected); _3DMarker.gameObject.SetActive(false); });
            _3DMarker.Edit.AddListener(() => { SelectMelanoma(_melanomaConfigurator.Selected); _3DMarker.gameObject.SetActive(false);});
        }

        private void Update()
        {
            if (_placing) UpdateMelanomaPlacement();
            if (!_placing) 
            {
                // reset Cursor
                _cursor?.Cursor1();

                SelectMelanoma();
            }
        }

        private void UpdateMelanomaPlacement()
        {
            bool placing = _placing;
            _cursor?.Cursor2();
            if (!_display3D.GetRoomWindow.isInside) return;

            Ray mousePointerRay = _display3D.GetViewRenderCamera().ViewportPointToRay(_display3D.GetMouseCoords(out _));
            int layer = LayerMask.NameToLayer("Melanoma");
            if (Physics.Raycast(mousePointerRay, out RaycastHit hit, 50, layer))
            {
                string descriptor = GetComponent<GetBodySegmentation>().GetDescriptor(hit.textureCoord);
                _lesionManager.HandleMelanomaPlacement(hit, ref placing, descriptor);
                SetPlacing(placing);
            }
        }

        private void SelectMelanoma()
        {
            if (!_display3D.GetRoomWindow.isInside) return;

            if(!Input.GetMouseButtonDown(0) || Input.GetMouseButton(1)) return;

            Ray mousePointerRay = _display3D.GetViewRenderCamera().ViewportPointToRay(_display3D.GetMouseCoords(out _));
            int layer = LayerMask.NameToLayer("Melanoma");
            if (Physics.Raycast(mousePointerRay, out RaycastHit hit, 50))
            {
                if(hit.collider.gameObject.GetComponent<MelanomaCollider>() != null)
                {
                    _3DMarker.HighlightedObject = hit.collider.transform;
                    _3DMarker.gameObject.SetActive(true);
                    _melanomaConfigurator.Selected = hit.collider.GetComponent<MelanomaCollider>().GetMelanoma();

                    if(_melanomaConfigurator.isActiveAndEnabled)
                    {
                        SelectMelanoma(_melanomaConfigurator.Selected);
                    }

                    Debug.Log("Melanoma Hit");
                }
            }
        }

        private void OnDestroy()
        {
            _pathologyDataConfigurator.RestoreOriginalTextures();
            SetPlacing(false);
            _3DMarker.gameObject.SetActive(false);
        }

        public void SetPlacing(bool placing)
        {
            _placing = placing;
            PlacingHint.SetActive(placing);
        }

        public override SerializablePathologyVariant.SerializablePathologyData GetConfiguration(string resourcesFolder)
        {
            return new SerializablePathologyVariant.SerializablePathologyData(
                _pathologyVariantManager.ActivePathologyVariant.Pathology, resourcesFolder, new SimpleFileWriter());
        }

        public override void LoadConfiguration(SerializablePathologyVariant.SerializablePathologyData configuration,
            string resourcesFolder)
        {
            //Cast to PathologyData
            PathologyData pathologyData = configuration.Deserialize(resourcesFolder);

            //Write loaded pathology data to active variant
            _pathologyVariantManager.ActivePathologyVariant.Pathology = pathologyData;

            //Update UI elements and textures
            LoadDataFromVariant(_pathologyVariantManager.ActivePathologyVariant);
        }

        private new void OnEnable()
        {
            base.OnEnable();
            _pathologyVariantManager.GetCurrentAgentData().SetNaked(true);
            _camera.Reset2();
            SetPlacing(false);
            _3DMarker.gameObject.SetActive(false);
        }

        public void Drop()
        {
            if (previewImageRaw.texture is RenderTexture previewImage)
            {
                if (previewImage != null && _melanomaConfigurator.Selected != null)
                {
                    _melanomaConfigurator.SetMelanoma(previewImage, _melanomaConfigurator.Selected.Size, Color.white);
                }
                else
                {
                    Debug.LogError("PreviewImage or selected melanoma is null.");
                }
            }
        }

        public void SelectMelanoma(Melanoma melanoma)
        {
            _lesionManager.SelectMelanoma(melanoma);
            _melanomaConfigurator.gameObject.SetActive(true);
            _melanomaConfigurator.SetUI(melanoma);
            //Debug.Log($"Selected Melanoma {melanoma.name}");
        }

        public void AddMelanoma()
        {
            _melanomaConfigurator.SetMelanoma();
            Drop();
        }

        public void RequestDeleteMelanoma(Melanoma melanoma)
        {
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.DeleteMelanoma, onConfirm: () => DeleteMelanoma(melanoma));
        }

        public void DeleteMelanoma(Melanoma melanoma)
        {
            _lesionManager.RemoveMelanoma(melanoma);
            CloseConfigurator();
        }

        public void CloseConfigurator()
        {
            _melanomaConfigurator.gameObject.SetActive(false);
            var melanomas = _pathologyVariantManager.ActivePathologyVariant.Pathology.Melanoma;
            _melanomaUIList.ReplaceItems(melanomas);
        }

        public void RestoreUIState(PathologyData pathologyData)
        {
            //Restore Naevi Section
            _naeviEnabledToggle.isOn = pathologyData.NaeviEnabled;
            _naeviEnabledToggle.onValueChanged.Invoke(_naeviEnabledToggle.isOn);
            
            if (pathologyData.Naevis.Count != 0)
                _naeviConfigurator.SetUI(pathologyData.Naevis[0]);
            else
                _naeviConfigurator.ResetUI();
            
            //Restore Melanoma Section
            _melanomaEnabledToggle.isOn = pathologyData.MelanomaEnabled;
            _melanomaEnabledToggle.onValueChanged.Invoke(_melanomaEnabledToggle.isOn);
            _melanomaUIList.ReplaceItems(pathologyData.Melanoma);
        }

        private void OnNaeviUpdate()
        {
            // Unload old Naevi Textures
            var pathology = _pathologyVariantManager.ActivePathologyVariant.Pathology;
            foreach (Naevi p in pathology.Naevis)
            {
                p.Shape = null;
            }

            Resources.UnloadUnusedAssets();

            List<Naevi> naevies = _naeviConfigurator.GetNaevi();
            pathology.Naevis = naevies;
            _lesionManager.UpdateLesions();
        }

        public void OnNaeviEnabledChanged(bool toggled)
        {
            _pathologyVariantManager.ActivePathologyVariant.Pathology.NaeviEnabled = toggled;
            _naeviConfigurator.SetInteractable(toggled);
            _lesionManager.UpdateLesions();
        }
        
        public void OnMelanomaEnabledChanged(bool toggled)
        {
            _pathologyVariantManager.ActivePathologyVariant.Pathology.MelanomaEnabled = toggled;
            _lesionManager.UpdateLesions();
            foreach (Melanoma m in _pathologyVariantManager.ActivePathologyVariant.Pathology.Melanoma)
            {
                m.collider?.SetActive(toggled);
            }
        }

        public void LoadDataFromVariant(PathologyVariant pathologyVariant)
        {
            _pathologyDataConfigurator.SetUp(_pathologyVariantManager.GetCurrentAgentData(),
                pathologyVariant.Pathology);
            _lesionManager.UpdateLesions();

            foreach (Melanoma m in pathologyVariant.Pathology.Melanoma)
            {
                MelanomaCollider.SetMelanomaCollider(m);
            }

            RestoreUIState(pathologyVariant.Pathology);
            Resources.UnloadUnusedAssets();
        }

        public void WriteUnhandledChangesToVariant(PathologyVariant pathologyVariant)
        {
            //All changes are already handled by the PathologyDataConfigurator since all changes in the UI 
            //are applied to the active PathologyData object immediately
        }

        public override string PresetSubfolderName => "pathology";
        public override string PresetFileExtension => "patconf";

        public override bool ConfigurationIsValid(SerializablePathologyVariant.SerializablePathologyData configuration)
        {
            //TODO: Implement
            //throw new System.NotImplementedException();
            return true;
        }
    }
}