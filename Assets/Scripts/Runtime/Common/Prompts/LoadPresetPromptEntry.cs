using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.Common.Prompts
{
    /// <summary>
    /// An entry in the LoadPresetPrompt. Is instantiated from a template.
    /// </summary>
    public class LoadPresetPromptEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public enum SpriteState
        {
           Default,
           Hover,
           Selected
        }
        public string presetDirectory { get; set; }
        public bool isDefaultPreset { get; set; }
        public LoadPresetPrompt loadPresetPrompt { get; set; }
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _renameButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private Graphic _inputCaret;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _hoverSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Image _selectButtonImage;
        private bool _selected;
        private string _presetName;

        public string presetName
        {
            get => _presetName;
            set
            {
                _presetName = value;
                _nameInput.text = _presetName;
            }
        }

        public void Start()
        {
            _selectButton.onClick.AddListener(OnSelectPressed);
            _deleteButton.onClick.AddListener(OnDeletePressed);
            _renameButton.onClick.AddListener(OnRenamePressed);
            _nameInput.onEndEdit.AddListener(OnRenameConfirmed);
            _inputCaret = _nameInput.transform.Find("Text Area/Caret").GetComponent<Graphic>();
            _inputCaret.raycastTarget = false;
        }

        public void OnDeletePressed()
        {
            loadPresetPrompt.RequestDeletePreset(this);
        }

        public void OnSelectPressed()
        {
            loadPresetPrompt.SelectPreset(this);
        }

        public void OnSelect()
        {
            SetSprite(SpriteState.Selected);
            _selected = true;
        }
        public void OnDeselect()
        {
            SetSprite(SpriteState.Default);
            _selected = false;
        }

        public void OnRenamePressed()
        {
            _nameInput.interactable = true;
            _nameInput.Select();
            _selectButton.gameObject.SetActive(false);
            _inputCaret.raycastTarget = true;
        }

        public void OnRenameConfirmed(string newName)
        {
            loadPresetPrompt.RenamePreset(this, _nameInput.text);
            _selectButton.gameObject.SetActive(true);
            _inputCaret.raycastTarget = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetSprite(SpriteState.Hover);
            if (isDefaultPreset) return;
            _renameButton.gameObject.SetActive(true);
            _deleteButton.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetSprite(_selected ? SpriteState.Selected : SpriteState.Default);
            if (isDefaultPreset) return;
            _renameButton.gameObject.SetActive(false);
            _deleteButton.gameObject.SetActive(false);
        }

        public void SetSprite(SpriteState spriteState)
        {
            _selectButtonImage.sprite = spriteState switch
            {
                SpriteState.Default => _defaultSprite,
                SpriteState.Hover => _hoverSprite,
                SpriteState.Selected => _selectedSprite,
                _ => _selectButtonImage.sprite
            };
        }
    }
}