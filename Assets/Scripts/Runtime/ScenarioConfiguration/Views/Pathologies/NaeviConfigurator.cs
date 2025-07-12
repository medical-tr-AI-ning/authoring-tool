using System;using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using Runtime.UI;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    /// <summary>
    /// Handles all UI inputs regarding the Naevi configuration
    /// </summary>
    public class NaeviConfigurator : MonoBehaviour
    {
        [SerializeField]
        Texture2D HeadNaevi;
        [SerializeField]
        Texture2D HeadMask;
        [SerializeField]
        Texture2D TorsoNaevi;
        [SerializeField]
        Texture2D TorsoMask;
        [SerializeField]
        Texture2D ArmsNaevi;
        [SerializeField]
        Texture2D ArmsMask;
        [SerializeField]
        Texture2D LegsNaevi;
        [SerializeField]
        Texture2D LegsMask;

        [SerializeField]
        Material NaeviOverlayMat;
        [SerializeField]
        Material _HeightBlurMat;

        [SerializeField]
        Slider Spreading;
        [SerializeField]
        Slider Elevation;
        [SerializeField]
        float BlurSize = 0.003f;
        public UnityEvent NaeviUpdate;
        public UnityEvent NaeviElevationUpdate;
        JObject _parsedData;
        [SerializeField]
        string _pathToJson;

        public void Start()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, _pathToJson);
            string jsonData = File.ReadAllText(jsonPath);
            _parsedData = JObject.Parse(jsonData);
        }
        
        void OnEnable()
        {  
            Spreading.onValueChanged.AddListener(UpdateSpread);  
            Elevation.onValueChanged.AddListener(UpdateElevation); 
        }

        void OnDisable()
        {    
            Spreading.onValueChanged.RemoveListener(UpdateSpread);
            Elevation.onValueChanged.RemoveListener(UpdateElevation); 
        }

        public void SetInteractable(bool interactable)
        {
            Spreading.GetComponent<StyledSelectable>().SetInteractable(interactable);
            Spreading.interactable = interactable;
            Elevation.GetComponent<StyledSelectable>().SetInteractable(interactable);
            Elevation.interactable = interactable;
        }

        public void SetUI(Naevi naevi)
        {
            Spreading.value = (float)Math.Pow(naevi.Spread, 1/3f);
            Elevation.value = naevi.Elevation;
        }

        public void ResetUI()
        {
            Spreading.value = 0;
            Elevation.value = 0;
        }

        public void UpdateSpread(float sliderValue)
        {
            NaeviUpdate.Invoke();
            UpdateElevation(Elevation.value);
        }

        void UpdateElevation(float sliderValue)
        {
            _HeightBlurMat.SetFloat("_Height", sliderValue);
            NaeviElevationUpdate.Invoke();
        }

        public List<Naevi> GetNaevi()
        {
            List<Naevi> list = new List<Naevi>
            {
                GetNaevi(0, HeadNaevi, HeadMask),
                GetNaevi(1, TorsoNaevi, TorsoMask),
                GetNaevi(2, ArmsNaevi, ArmsMask),
                GetNaevi(3, LegsNaevi, LegsMask)
            };
            return list;
        }

        Naevi GetNaevi(int i, Texture2D main, Texture2D mask)
        {
            Naevi n = new Naevi();            
            n.Shape = RenderMaterialToTexture(main, mask, (float)Math.Pow(Spreading.value, 3f));
            n.HeightMap = RenderHeightMap(n.Shape);
            n.TextureCoord = new Vector2(0.5f + i, 0.5f);
            n.Spread = (float)Math.Pow(Spreading.value, 3f);
            n.Elevation = Elevation.value;
            SetHighResTextures(n, i, mask, main);
            return n;
        }

        void SetHighResTextures(Naevi n, int i, Texture2D mask, Texture2D main)
        {
            n.HighRes = new List<Naevi.HighResNaevi>();
            switch (i)
            {
                case 0:
                    CheckTextures(_parsedData, "head", HeadMask, main, n);
                    break;
                case 1:
                    CheckTextures(_parsedData, "body", TorsoMask, main, n);
                    break;
                case 2:
                    CheckTextures(_parsedData, "arms", ArmsMask, main, n);
                    break;
                case 3:
                    CheckTextures(_parsedData, "legs", LegsMask, main, n);
                    break;
            }
        }

        private void CheckTextures(JObject data, string section, Texture2D mask, Texture2D main, Naevi naevi)
        {
            if (data[section] != null)
            {
                foreach (var item in (JObject)data[section])
                {
                    string textureName = item.Key;
                    JObject properties = (JObject)item.Value;

                    int x = (int)properties["x"];
                    int y = (int)properties["y"];
                    int visibilityLevel = (int)item.Value["visibility_level"];

                    if (visibilityLevel < naevi.Spread * 255)
                    {
                        Naevi.HighResNaevi highRes = new Naevi.HighResNaevi();
                        highRes.Position = new Vector2(x,y);
                        highRes.Texture = Path.Combine(section,textureName);
                        naevi.HighRes.Add(highRes);
                    }
                }
            }
        }

        private RenderTexture RenderMaterialToTexture(Texture2D main, Texture2D mask, float threshold )
        {
            RenderTexture rt = new RenderTexture(main.width, main.height, 16, RenderTextureFormat.ARGB32);

            // Setze die RenderTexture als Ziel für den Renderprozess
            RenderTexture.active = rt;

            // Optional: Lösche die RenderTexture, damit sie sauber ist
            GL.Clear(true, true, Color.clear);

            NaeviOverlayMat.SetTexture("_MainTex", main);
            NaeviOverlayMat.SetTexture("_MaskTex", mask);
            NaeviOverlayMat.SetFloat("_Threshold", threshold);

            // Nutze Graphics.Blit, um das Material auf die RenderTexture zu rendern
            Graphics.Blit(null, rt, NaeviOverlayMat, 0);

            // Setze die RenderTexture wieder auf null, um wieder zum normalen Rendering zurückzukehren
            RenderTexture.active = null;
            return rt;
        }
        
        private RenderTexture RenderHeightMap(RenderTexture from)
        {
            _HeightBlurMat.SetFloat("_BlurSize", BlurSize); 
            RenderTexture rt = new RenderTexture(1024, 1024, 16, from.format);

            // Setze die RenderTexture als Ziel für den Renderprozess
            RenderTexture.active = rt;

            // Optional: Lösche die RenderTexture, damit sie sauber ist
            GL.Clear(true, true, Color.clear);

            // Nutze Graphics.Blit, um das Material auf die RenderTexture zu rendern
            Graphics.Blit(from, rt, _HeightBlurMat,0);

            // Setze die RenderTexture wieder auf null, um wieder zum normalen Rendering zurückzukehren
            RenderTexture.active = null;
            return rt;
        }
    }  
}
