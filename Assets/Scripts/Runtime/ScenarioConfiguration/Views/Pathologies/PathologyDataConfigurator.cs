using System;
using System.Collections.Generic;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    /// <summary>
    /// Provides functionality to configure pathology data with its
    /// correspondent agent.
    /// </summary>
    public class PathologyDataConfigurator
    {
        AgentData _agentData;

        PathologyData _data;

        public int TextureCount{ get {return _agentData.Tex.Length; }}

        public void SetUp(AgentData agent, PathologyData data)
        {
            _agentData = agent;
            _data = data;

            if(_agentData.Tex.Length != _agentData.HeightTex.Length || _agentData.Tex.Length != _agentData.Mat.Length)
            {
                throw new Exception("Agent " +  _agentData.gameObject.name + " differing Texture, Heighttexture and Material numbers");
            }

            if(_agentData.Tex.Length == 0 || _agentData.Mat.Length == 0 || _agentData.HeightTex.Length == 0)
            {
                throw new Exception("Agent " + _agentData.gameObject.name + " does not have either a Texture, HeightTexture or Material");
            }

            // Initialize modified textures
            _data.Modified = new RenderTexture[_agentData.Tex.Length];
            _data.ModifiedHeight = new RenderTexture[_agentData.HeightTex.Length];

            for(int i = 0; i < _data.Modified.Length; i++)
            {
                SetUpSkinTextures(i);
            }

            for(int i = 0; i < _data.Modified.Length; i++)
            {
                ApplyTexture(i);
            }
        }

        public void RestoreOriginalTextures()
        {
            for(int i = 0; i < _agentData.Tex.Length; i++)
            {
                _agentData.Mat[i].SetTexture("_DiffuseMap", _agentData.Tex[i]);
                _agentData.Mat[i].SetTexture("_BaseColorMap", _agentData.Tex[i]);
            }
        }

        // Method to set up skin textures for a specific index
        public void SetUpSkinTextures(int skinIndex)
        {
            _data.Modified[skinIndex] = ConvertToRenderTexture(_agentData.Tex[skinIndex]);
            _data.ModifiedHeight[skinIndex] = ConvertToRenderTexture(_agentData.HeightTex[skinIndex]);
        }

        // Method to apply modified texture for a specific index
        public void ApplyTexture(int skinIndex)
        {
            _agentData.Mat[skinIndex].SetTexture("_BaseColorMap", _data.Modified[skinIndex]);
            _agentData.Mat[skinIndex].SetTexture("_HeightMap", _data.ModifiedHeight[skinIndex]);
        }

        public RenderTexture GetSkinTexture(int skinIndex)
        {
            return _data.Modified[skinIndex];
        }

        public RenderTexture GetHeightTexture(int skinIndex)
        {
            return _data.ModifiedHeight[skinIndex];
        }

        public RenderTexture ConvertToRenderTexture(Texture2D texture2D)
        {
            // Erstelle eine neue RenderTexture mit den gleichen Abmessungen wie die Texture2D
            RenderTexture renderTexture =
                new RenderTexture(texture2D.width, texture2D.height, 0, RenderTextureFormat.ARGB32);
            renderTexture.wrapMode = TextureWrapMode.Repeat;

            // Aktiviere die RenderTexture, um das Ziel für das Rendering festzulegen
            RenderTexture.active = renderTexture;

            // Kopiere die Texture2D-Daten in die RenderTexture
            Graphics.Blit(texture2D, renderTexture);

            // Deaktiviere die RenderTexture
            RenderTexture.active = null;

            return renderTexture;
        }

        public List<Lesion> GetPathologiesList()
        {
            List<Lesion> list = new List<Lesion>();
            if (_data.NaeviEnabled)
                list.AddRange(_data.Naevis);
            if (_data.MelanomaEnabled)
                list.AddRange(_data.Melanoma);
            return list;
        }

        public List<Melanoma> GetMelanomaList()
        {
            return _data.Melanoma;
        }
    }
}
