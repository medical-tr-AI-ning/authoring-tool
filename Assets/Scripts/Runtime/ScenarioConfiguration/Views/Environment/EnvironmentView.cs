using System;
using System.Collections.Generic;
using System.Linq;
using medicaltraining.assetstore.ScenarioConfiguration;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Common;
using Runtime.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

namespace Runtime.ScenarioConfiguration.Views.Environment
{
    /// <summary>
    /// Handles the "Raumeinrichtung" view UI and logic.
    /// </summary>
    public class EnvironmentView : PresetConfigurationView<EnvironmentConfig>
    {
        [SerializeField] private Display3D _display3D;
        private PlacedObject _activeObject;
        private PlacedObjectUIItemList _itemList;
        [SerializeField] private float _rotationStep = 30f;
        [SerializeField] private Transform _objectContainer;
        [SerializeField] private List<PrefabMapping> _prefabMappings;
        [SerializeField] private GameObject _placingHint;
        [SerializeField] private GetPointOnRendertexture _3DMarker;
        [SerializeField] private TMP_Text _RoomTitle;
        [SerializeField] private RoomOption _defaultRoomOption;
        [SerializeField] private List<RoomOption> _roomOptions;
        private const float _vectorTolerance = 0.01f;
        private SetCursor _cursorSetter;
        private PlacedObject _selectedObject;
        private RoomOption _selectedRoomOption;
        [SerializeField] private EnvironmentSelectablesList _environmentSelectablesList;


        public override string PresetSubfolderName => "environment";
        public override string PresetFileExtension => "envconfig";

        public List<PlacedObject> PlacedObjects { get; private set; }

        public override void Initialize()
        {
            PlacedObjects = new List<PlacedObject>();
            _itemList = FindObjectOfType<PlacedObjectUIItemList>();
            _cursorSetter = FindObjectOfType<SetCursor>();
            _display3D = GetComponentInChildren<Display3D>();
            _3DMarker.gameObject.SetActive(false);
            _3DMarker.Pickup.AddListener(() => PickUpPlacedObject(_selectedObject));
            _3DMarker.Delete.AddListener(() => RemovePlacedObject(_selectedObject));
            _environmentSelectablesList.Initialize();
            SetSelectedRoom(_defaultRoomOption, true);
        }

        public void StartPlacement(string prefabID)
        {
            if (_activeObject) removeActiveObject();
            PlacedObject instantiatedObject = preparePlacedObject(prefabID);
            attachObjectToMouse(instantiatedObject);
            _placingHint.SetActive(true);
            _3DMarker.gameObject.SetActive(false);
        }

        public void RemovePlacedObject(PlacedObject placedObject)
        {
            unregisterPlacedObject(placedObject);
            Destroy(placedObject.gameObject);
            _3DMarker.gameObject.SetActive(false);
        }


        public void PickUpPlacedObject(PlacedObject placedObject)
        {
            unregisterPlacedObject(placedObject);
            attachObjectToMouse(placedObject);
            _3DMarker.gameObject.SetActive(false);
        }


        public void ConfirmPlacement()
        {
            if (_activeObject.CurrentState != PlacedObject.PlacementState.Placeable) return;
            _activeObject.SetPlacementState(PlacedObject.PlacementState.Placed);
            registerPlacedObject(_activeObject);
            detachObjectFromMouse();
            _placingHint.SetActive(false);
            _3DMarker.gameObject.SetActive(false);
        }

        public void ClearPlacedObjects()
        {
            for (int i = PlacedObjects.Count - 1; i >= 0; i--)
                RemovePlacedObject(PlacedObjects[i]);
        }

        public void SetSelectedRoom(string environmentID, bool updateSelectablesList)
        {
            RoomOption environmentOption = _roomOptions.FirstOrDefault(option => option.RoomID == environmentID);
            if (!environmentOption)
            {
                Debug.LogError(
                    $"Agent with ID {environmentID} doesn't exist! Falling back to {_defaultRoomOption.RoomID}!");
                environmentOption = _defaultRoomOption;
            }

            SetSelectedRoom(environmentOption, updateSelectablesList);
        }

        public void SetSelectedRoom(RoomOption selectedRoomOption, bool updateSelectablesList)
        {
            ClearPlacedObjects();
            foreach (RoomOption roomOption in _roomOptions)
            {
                bool isSelected = roomOption.Equals(selectedRoomOption);
                roomOption.Room.SetActive(isSelected);
            }
            _selectedRoomOption = selectedRoomOption;
            _RoomTitle.text = selectedRoomOption.RoomName;
            
            if (updateSelectablesList) _environmentSelectablesList.SelectEntry(selectedRoomOption);
        }
        

        public override void LoadConfiguration(EnvironmentConfig environmentConfig, string resourcesFolder)
        {
            foreach (RoomOption room in _roomOptions)
            {
                if (room.RoomID == environmentConfig.EnvironmentID)
                {
                    _selectedRoomOption = room;
                    break;
                }
            }
            SetSelectedRoom(environmentConfig.EnvironmentID, true);
            loadSerializedObjectPlacements(environmentConfig.ObjectPlacements);
        }

        public override EnvironmentConfig GetConfiguration(string resourcesFolder)
        {
            EnvironmentConfig environmentConfig = new EnvironmentConfig
            {
                EnvironmentID = _selectedRoomOption.RoomID,
                ObjectPlacements = new List<ObjectPlacement>()
            };
            foreach (PlacedObject placedObject in PlacedObjects)
            {
                environmentConfig.ObjectPlacements.Add(new ObjectPlacement(placedObject.transform,
                    placedObject.PrefabID));
            }

            return environmentConfig;
        }

        public override bool ConfigurationIsValid(EnvironmentConfig configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration.EnvironmentID);
        }

        private void loadSerializedObjectPlacements(List<ObjectPlacement> objectPlacements)
        {
            ClearPlacedObjects();
            foreach (ObjectPlacement objectPlacement in objectPlacements)
            {
                PlacedObject placedObject = preparePlacedObject(objectPlacement.PrefabID);
                if (placedObject is null) continue;
                objectPlacement.SerializableTransform.ApplyPropertiesToTransform(placedObject.transform);
                registerPlacedObject(placedObject);
            }
        }

        private void registerPlacedObject(PlacedObject placedObject)
        {
            PlacedObjects.Add(placedObject);
            _itemList.AddListItem(placedObject);
        }

        private void unregisterPlacedObject(PlacedObject placedObject)
        {
            PlacedObjects.Remove(placedObject);
            _itemList.RemoveListItem(placedObject);
        }

        private PlacedObject preparePlacedObject(string prefabID)
        {
            //Find Prefab for prefabID
            PrefabMapping prefabMapping = _prefabMappings.Find(mapping => mapping.prefabID == prefabID);
            if (prefabMapping is null)
            {
                Debug.LogError($"{prefabID} has no corresponding Prefab Mapping!");
                return null;
            }

            //Instantiate Prefab
            GameObject go = Instantiate(prefabMapping.prefab, _objectContainer, true);
            go.gameObject.SetActive(true);

            //Configure PlacedObject
            PlacedObject placedObject = go.GetComponent<PlacedObject>();
            if (placedObject is null)
            {
                Debug.LogError($"{go.name} is missing a PlacedObject Component!");
                return null;
            }

            placedObject.PrefabID = prefabID;
            return placedObject;
        }

        private PlacedObject getClickedOnPlacedObject()
        {
            GameObject obj = getClickedOnGameObject(LayerMask.GetMask("PlacedObjects"));
            PlacedObject placedObject = obj?.GetComponent<PlacedObject>();
            return placedObject;
        }

        private GameObject getClickedOnGameObject(LayerMask layerMask)
        {
            Ray mousePointerRay = _display3D.GetViewRenderCamera().ViewportPointToRay(_display3D.GetMouseCoords(out _));
            if (Physics.Raycast(mousePointerRay, out RaycastHit hit, 50, layerMask))
            {
                return hit.transform.gameObject;
            }

            return null;
        }

        private void attachObjectToMouse(PlacedObject go) => _activeObject = go;
        private void detachObjectFromMouse() => _activeObject = null;

        private void removeActiveObject()
        {
            _3DMarker.gameObject.SetActive(false);
            if (!_activeObject) return;
            RemovePlacedObject(_activeObject);
        }

        private void handleButtonInputs()
        {
            //Left Click
            if (Input.GetMouseButtonDown(0))
            {
                PlacedObject placedObject = getClickedOnPlacedObject();
                if (placedObject != null && _activeObject == null)
                {
                    //PickUpPlacedObject(placedObject);
                    _3DMarker.gameObject.SetActive(true);
                    _3DMarker.HighlightedObject = placedObject.transform;
                    _3DMarker.SetName(placedObject.DisplayName);
                    _selectedObject = placedObject;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                bool inside;
                _display3D.GetMouseCoords(out inside);
                if (!inside) return;
                if (_activeObject)
                {
                    ConfirmPlacement();
                }
            }

            //Right Click
            if (Input.GetMouseButtonDown(1))
            {
                if (_activeObject) rotateObjectOneStep();
            }

            //Escape

            if (Input.GetButtonDown("Cancel"))
            {
                removeActiveObject();
            }
        }

        private bool isMouseOnUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private void updateActiveObject()
        {
            Ray mousePointerRay = _display3D.GetViewRenderCamera().ViewportPointToRay(_display3D.GetMouseCoords(out _));
            LayerMask allExceptPlacedObjects = ~(1 << LayerMask.NameToLayer("PlacedObjects"));
            //Update Position
            if (Physics.Raycast(mousePointerRay, out RaycastHit hit, 50, allExceptPlacedObjects))
            {
                _activeObject.transform.position = hit.point;
                float diffToUpVector = Math.Abs((hit.normal - Vector3.up).magnitude);
                bool surfaceIsHorizontal = diffToUpVector < _vectorTolerance;
                _activeObject.SetPlacementState(surfaceIsHorizontal
                    ? PlacedObject.PlacementState.Placeable
                    : PlacedObject.PlacementState.Unplaceable);
            }
            else
            {
                _activeObject.SetPlacementState(PlacedObject.PlacementState.Unplaceable);
            }
        }


        private void rotateObjectOneStep()
        {
            if (!_activeObject) return;
            _activeObject.transform.Rotate(new Vector3(0, 1, 0), _rotationStep, Space.World);
        }

        private void FixedUpdate()
        {
            if (_activeObject) updateActiveObject();
            if (_activeObject && _cursorSetter)
            {
                _cursorSetter.Cursor2();
            }
            else if (_cursorSetter)
            {
                _cursorSetter.Cursor1();
            }
        }

        private void Update()
        {
            handleButtonInputs();
        }

        new void OnDisable()
        {
            base.OnDisable();
            removeActiveObject();
            _cursorSetter?.Cursor1();
            _3DMarker.gameObject.SetActive(false);
        }

        new void OnEnable()
        {
            base.OnEnable();
            _placingHint.SetActive(false);
        }
    }
}