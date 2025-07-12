using Runtime.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    /// <summary>
    /// Handles all UI inputs regarding the Melanoma Configuration
    /// </summary>
    public class MelanomaConfigurator : MonoBehaviour
    {
        public Melanoma Selected;
        public Slider Size;
        public Slider Spreading;
        public Slider Limitation;
        public Slider Elevation;
        public Slider Brightness;
        public Slider Contrast;
        public ColorPicker Picker;
        private PathologiesView _view;
        public RawImage Raw;
        [SerializeField] private Material _previewMat;
        [SerializeField] private Material _heightBlurMat;
        [SerializeField] private RenderTexture _previewTexture;
        private EditableTexture _segmentationMask;
        public UnityEvent OnMelanomaUpdate;
        [SerializeField] private float _blurSize = .1f;
        public Texture2D DefaultBaseTexture;

        public void Awake()
        {
            _view = FindObjectOfType<PathologiesView>();
            _segmentationMask = GetComponent<EditableTexture>();
        }

        public void SetUI(Melanoma melanoma)
        {
            _segmentationMask = GetComponent<EditableTexture>();
            Size.value = melanoma.SizeUI;
            Spreading.value = melanoma.Spreading;
            Limitation.value = melanoma.Limitation;
            Elevation.value = melanoma.Elevation;
            Brightness.value = melanoma.Brightness;
            Contrast.value = melanoma.Contrast;
            Picker.SetPosition(melanoma.ColorPosition);

            SetTexture(melanoma.BaseShape);
            UpdateScale(Size.value);
            UpdateBlur(Limitation.value);
            UpdateSpread(Spreading.value);
            UpdateContrast(Contrast.value);
            UpdateBrightness(Brightness.value);
            UpdateElevation(Elevation.value);
            UpdateTint();
        }

        public void SetMelanoma(Melanoma melanoma)
        {
            melanoma.Spreading = Spreading.value;
            melanoma.Limitation = Limitation.value;
            melanoma.Elevation = Elevation.value;
            melanoma.Brightness = Brightness.value;
            melanoma.Contrast = Contrast.value;
            melanoma.ColorPosition = Picker.GetPosition();
            melanoma.BaseShape = _segmentationMask?.GetTexture();
            melanoma.SizeUI = Size.value;
        }

        void OnEnable()
        {
            Size.onValueChanged.AddListener(UpdateScale);
            Limitation.onValueChanged.AddListener(UpdateBlur);
            Spreading.onValueChanged.AddListener(UpdateSpread);
            Contrast.onValueChanged.AddListener(UpdateContrast);
            Brightness.onValueChanged.AddListener(UpdateBrightness);
            Elevation.onValueChanged.AddListener(UpdateElevation);
            Picker.onValueChanged.AddListener(UpdateTint);

            /*_SegmentationMask = GetComponent<EditableTexture>();
            _SegmentationMask.SetTexture(DefaultBaseTexture);
            _PreviewMat.SetTexture("_Melanoma", _SegmentationMask.GetRGB());
            _PreviewMat.SetTexture("_Segmentation_Mask", _SegmentationMask.GetAlpha());
            RenderMaterialToTexture(_PreviewMat,null, _PreviewTexture);*/
        }

        void OnDisable()
        {
            Size.onValueChanged.RemoveListener(UpdateScale);
            Limitation.onValueChanged.RemoveListener(UpdateBlur);
            Spreading.onValueChanged.RemoveListener(UpdateSpread);
            Contrast.onValueChanged.RemoveListener(UpdateContrast);
            Brightness.onValueChanged.RemoveListener(UpdateBrightness);
            Elevation.onValueChanged.RemoveListener(UpdateElevation);
            Picker.onValueChanged.RemoveListener(UpdateTint);
        }

        void UpdateScale(float sliderValue)
        {
            _previewMat.SetFloat("_Scale", sliderValue);
            Render();
        }

        void UpdateBlur(float sliderValue)
        {
            Texture2D texture;
            int spreadRadius = (int)Spreading.value;
            texture = _segmentationMask.SpreadAlpha(spreadRadius);
            texture = _segmentationMask.GaussianBlurAlpha((int)sliderValue, texture, inplace: false);
            _previewMat.SetFloat("_Blur", sliderValue);
            _previewMat.SetTexture("_Segmentation_Mask", texture);
            Render();
        }


        void UpdateSpread(float sliderValue)
        {
            Texture2D texture;
            texture = _segmentationMask.SpreadAlpha((int)sliderValue);
            int blurRadius = (int)Limitation.value;
            texture = _segmentationMask.GaussianBlurAlpha(blurRadius, texture, inplace: false);
            _previewMat.SetFloat("_Spread", sliderValue);
            _previewMat.SetTexture("_Segmentation_Mask", texture);
            Render();
        }

        void UpdateContrast(float sliderValue)
        {
            _previewMat.SetFloat("_Contrast", sliderValue);
            Render();
        }

        void UpdateTint()
        {
            _previewMat.SetColor("_Tint", Picker.GetColor());
            Render();
        }

        void UpdateBrightness(float sliderValue)
        {
            _previewMat.SetFloat("_Brightness", sliderValue);
            Render();
        }

        void UpdateElevation(float sliderValue)
        {
            _heightBlurMat.SetFloat("_Height", sliderValue);
            Render();
        }

        public void SetTexture(Texture2D texture2D)
        {
            if (_segmentationMask is null)
            {
                _segmentationMask = GetComponent<EditableTexture>();
                Debug.Log("Segmentation mask was null");
            }

            _segmentationMask.SetTexture(texture2D);
            _previewMat.SetTexture("_Melanoma", _segmentationMask.GetRGB());
            _previewMat.SetTexture("_Segmentation_Mask", _segmentationMask.GetAlpha());
            Render();
        }

        private void Render()
        {
            RenderMaterialToTexture(_previewMat, null, _previewTexture);

            _heightBlurMat.SetFloat("_BlurSize", _blurSize);
            RenderMaterialToTexture(_heightBlurMat, _previewTexture, Selected.HeightMap);

            Raw.texture = _previewTexture;
            OnMelanomaUpdate.Invoke();
        }

        private void RenderMaterialToTexture(Material mat, RenderTexture from, RenderTexture to)
        {
            // Setze die RenderTexture als Ziel für den Renderprozess
            RenderTexture.active = to;

            // Optional: Lösche die RenderTexture, damit sie sauber ist
            GL.Clear(true, true, Color.clear);

            // Nutze Graphics.Blit, um das Material auf die RenderTexture zu rendern
            Graphics.Blit(from, to, mat, 0);

            // Setze die RenderTexture wieder auf null, um wieder zum normalen Rendering zurückzukehren
            RenderTexture.active = null;
        }

        public void SetMelanoma(RenderTexture tex, float size, Color colorTint)
        {
            UpdateMelanoma(Selected, tex, size, colorTint);
            SetMelanoma(Selected);
        }

        public void SetMelanoma()
        {
            CreateMelanoma();
            SetMelanoma(Selected);
            SetTexture(DefaultBaseTexture);
            Selected.BaseShape = DefaultBaseTexture;
            SetUI(Selected);
            _view.Drop();
        }

        private Melanoma CreateMelanoma()
        {
            /*GameObject g = new GameObject("melanoma", typeof(Melanoma));
            g.GetComponent<Melanoma>().textureCoord = new Vector2(-1, -1);
            Selected = g.GetComponent<Melanoma>();
            return g.GetComponent<Melanoma>();*/
            Melanoma m = new Melanoma();
            m.TextureCoord = new Vector2(-1f, -1f);
            Selected = m;
            return m;
        }

        private void UpdateMelanoma(Melanoma melanoma, RenderTexture tex, float size, Color colorTint)
        {
            RenderTexture sourceRT = tex;

            RenderTexture destinationRT =
                new RenderTexture(sourceRT.width, sourceRT.height, sourceRT.depth, sourceRT.format);

            // Kopiere die RenderTexture
            CopyRenderTexture(sourceRT, destinationRT);

            melanoma.Shape = destinationRT;
            Selected = melanoma;

            _heightBlurMat.SetFloat("_BlurSize", _blurSize);
            melanoma.HeightMap = new RenderTexture(melanoma.Shape.width, melanoma.Shape.height, melanoma.Shape.depth,
                melanoma.Shape.format);
            RenderMaterialToTexture(_heightBlurMat, melanoma.Shape, Selected.HeightMap);

            OnMelanomaUpdate.Invoke();
        }

        public void CopyRenderTexture(RenderTexture source, RenderTexture destination)
        {
            // Überprüfe, ob die Ziel-RenderTexture korrekt initialisiert ist
            if (destination == null || destination.width != source.width || destination.height != source.height)
            {
                Debug.LogError("Destination RenderTexture muss die gleiche Größe wie die Source RenderTexture haben.");
                return;
            }

            // Kopiere die Source-RenderTexture in die Destination-RenderTexture
            Graphics.Blit(source, destination);
        }
    }
}