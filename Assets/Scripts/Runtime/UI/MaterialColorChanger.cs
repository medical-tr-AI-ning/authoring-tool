using System.Collections.Generic;
using UnityEngine;

namespace Runtime.UI
{
    public class MaterialColorChanger : MonoBehaviour
    {
        //Only works with the HDRP Standard Lit and 3DSMax Physical Material Shaders. 
        //Changes all Base Colors of the object and children to a lerp between starting color and desired color. 

        private readonly Color newColor = new Color(0f / 255f, 0.8f, 0.6f, 255f / 255f);
        private readonly Color newColor2 = new Color(140 / 255f, 0 / 255f, 53f / 255f, 255f / 255f);
        private readonly Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();

        private readonly string PhysicalMaterialShaderName = "HDRP/3DSMaxPhysicalMaterial/PhysicalMaterial3DsMax";
        private readonly string StandardHDPShaderName = "HDRP/Lit";
        private readonly string BaseColorProperty = "_BaseColor";
        private readonly string PhysicalMaterialBaseColorProperty = "_BASE_COLOR";

        void Start()
        {
            SaveOriginalColors();
        }

        public void ChangeToNewColor()
        {
            foreach (var material in GetMaterials())
            {
                string colorProperty = null;

                if (material.shader.name == PhysicalMaterialShaderName)
                {
                    colorProperty = "_BASE_COLOR";
                }
                else if (material.shader.name == StandardHDPShaderName)
                {
                    colorProperty = "_BaseColor";
                }

                if (colorProperty != null && material.HasProperty(colorProperty))
                {
                    Color originalColor = material.GetColor(colorProperty);
                    Color lerpedColor = Color.Lerp(originalColor, newColor, 0.6f);
                    material.SetColor(colorProperty, lerpedColor);
                }
            }
        }

        public void ChangeToNewColor2()
        {
            foreach (var material in GetMaterials())
            {
                string colorProperty = null;

                if (material.shader.name == PhysicalMaterialShaderName)
                {
                    colorProperty = "_BASE_COLOR";
                }
                else if (material.shader.name == StandardHDPShaderName)
                {
                    colorProperty = "_BaseColor";
                }

                if (colorProperty != null && material.HasProperty(colorProperty))
                {
                    Color originalColor = material.GetColor(colorProperty);
                    Color lerpedColor = Color.Lerp(originalColor, newColor2, 0.6f);
                    material.SetColor(colorProperty, lerpedColor);
                }
            }
        }

        public void ResetToOriginalColor()
        {
            foreach (var material in GetMaterials())
            {
                if (material.shader.name == PhysicalMaterialShaderName && originalColors.ContainsKey(material) && material.HasProperty(PhysicalMaterialBaseColorProperty))
                {
                    material.SetColor(PhysicalMaterialBaseColorProperty, originalColors[material]);
                }
                else if (material.shader.name == StandardHDPShaderName && originalColors.ContainsKey(material) && material.HasProperty(BaseColorProperty))
                {
                    material.SetColor(BaseColorProperty, originalColors[material]);
                }
                else
                {
                    Debug.LogWarning($"Original color not found or property missing for {material.name}. Shader: {material.shader.name}");
                }
            }
        }

        private void SaveOriginalColors()
        {
            foreach (var material in GetMaterials())
            {
                if (material.shader.name == PhysicalMaterialShaderName && material.HasProperty(PhysicalMaterialBaseColorProperty) && !originalColors.ContainsKey(material))
                {
                    originalColors[material] = material.GetColor(PhysicalMaterialBaseColorProperty);
                }
                else if (material.shader.name == StandardHDPShaderName && material.HasProperty(BaseColorProperty) && !originalColors.ContainsKey(material))
                {
                    originalColors[material] = material.GetColor(BaseColorProperty);
                }
                else
                {
                    Debug.LogWarning($"Material {material.name} does not have the expected property '{BaseColorProperty}' or '{PhysicalMaterialBaseColorProperty}', or color already saved. Shader: {material.shader.name}");
                }
            }
        }

        private IEnumerable<Material> GetMaterials()
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in meshRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (material.shader.name == PhysicalMaterialShaderName || material.shader.name == StandardHDPShaderName)
                    {
                        yield return material;
                    }
                    else
                    {
                        Debug.LogWarning($"Material {material.name} has shader '{material.shader.name}', expected '{PhysicalMaterialShaderName}' or '{StandardHDPShaderName}'.");
                    }
                }
            }
        }
    }
}