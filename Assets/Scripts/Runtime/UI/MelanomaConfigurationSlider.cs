using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class MelanomaConfigurationSlider : MonoBehaviour
    {
        private Material mat;
        private EditableTexture segmentationMask;

        [SerializeField] private Slider scaleSlider;    
        [SerializeField] private Slider blurSlider;    
        [SerializeField] private Slider spreadSlider;    
        [SerializeField] private Slider contrastSlider;
        [SerializeField] private Slider brightnessSlider;
 

        void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
            segmentationMask = GetComponent<EditableTexture>();
        }

        void OnEnable()
        {
            scaleSlider.onValueChanged.AddListener(UpdateScale);    
            blurSlider.onValueChanged.AddListener(UpdateBlur);    
            spreadSlider.onValueChanged.AddListener(UpdateSpread);    
            contrastSlider.onValueChanged.AddListener(UpdateContrast);    
            brightnessSlider.onValueChanged.AddListener(UpdateBrightness);    
        }

        void OnDisable()
        {
            scaleSlider.onValueChanged.RemoveListener(UpdateScale);    
            blurSlider.onValueChanged.RemoveListener(UpdateBlur);    
            spreadSlider.onValueChanged.RemoveListener(UpdateSpread);    
            contrastSlider.onValueChanged.RemoveListener(UpdateContrast);    
            brightnessSlider.onValueChanged.RemoveListener(UpdateBrightness);    
        }

        void UpdateScale(float sliderValue)
        {
            mat.SetFloat("_Scale", sliderValue);
        }

        void UpdateBlur(float sliderValue)
        {
            Texture2D texture;
            int spreadRadius = (int)spreadSlider.value;
            texture = segmentationMask.SpreadAlpha(spreadRadius);
            texture = segmentationMask.GaussianBlurAlpha((int)sliderValue, texture, inplace: false);
            mat.SetFloat("_Blur", sliderValue);
            mat.SetTexture("_Segmentation_Mask", texture);
        }

        void UpdateSpread(float sliderValue)
        {
            Texture2D texture;
            texture = segmentationMask.SpreadAlpha((int)sliderValue);
            int blurRadius = (int)blurSlider.value;
            texture = segmentationMask.GaussianBlurAlpha(blurRadius, texture, inplace: false);
            mat.SetFloat("_Spread", sliderValue);
            mat.SetTexture("_Segmentation_Mask", texture);
        }

        void UpdateContrast(float sliderValue)
        {
            mat.SetFloat("_Contrast", sliderValue);
        }

        void UpdateBrightness(float sliderValue)
        {
            mat.SetFloat("_Brightness", sliderValue);
        }
    }
}
