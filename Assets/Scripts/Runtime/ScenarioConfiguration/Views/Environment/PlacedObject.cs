using Runtime.UI;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Environment
{
    /// <summary>
    /// An instatiated prefabbed object that has been placed into the scene by the user.
    /// </summary>
    public class PlacedObject : MonoBehaviour
    {
        public string PrefabID { get; set; }
        public string DisplayName;

        //TODO: Make this static or global somehow
        [SerializeField] private Material _placeableMaterial;
        [SerializeField] private Material _unplaceableMaterial;

        private Material[][] _materials;
        private Renderer[] _renderers;
        private Rigidbody _rigidbody;
        public PlacementState CurrentState { get; private set; }
        public MaterialColorChanger Color;

        public enum PlacementState
        {
            Placeable,
            Unplaceable,
            Placed
        }

        private void Awake()
        {
            Color = gameObject.AddComponent<MaterialColorChanger>();
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            if (!_rigidbody) Debug.LogWarning($"PlacedObject {name} has no RigidBody Component!");
            _renderers = gameObject.GetComponentsInChildren<Renderer>();
            _materials = new Material[_renderers.Length][];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _materials[i] = _renderers[i].materials;
            }
        }

        public void SetPlacementState(PlacementState state)
        {
            CurrentState = state;
            updateMaterials();
            setFreezePosition(state == PlacementState.Placed);
        }

        private void updateMaterials()
        {
            /*if (CurrentState == PlacementState.Placed)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].materials = _materials[i];
                }
            }
            else
            {
                Material replacementMaterial =
                    CurrentState == PlacementState.Placeable ? _placeableMaterial : _unplaceableMaterial;

                foreach (Renderer t in _renderers)
                {
                    Material[] transparents = new Material[t.materials.Length];
                    for (int j = 0; j < t.materials.Length; j++)
                        transparents[j] = replacementMaterial;

                    t.materials = transparents;
                }
            }*/
            if(CurrentState == PlacementState.Placed)
            {
                Color.ResetToOriginalColor();
            } 
            else if (CurrentState == PlacementState.Placeable)
            {
                Color.ChangeToNewColor();
            }
            else if (CurrentState == PlacementState.Unplaceable)
            {
                Color.ChangeToNewColor2();
            }
        }

        private void setFreezePosition(bool freeze)
        {
            if (!_rigidbody) return;
            _rigidbody.constraints = freeze ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
        }
    }
}