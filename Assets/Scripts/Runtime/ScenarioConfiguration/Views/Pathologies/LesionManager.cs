using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    /// <summary>
    /// Handles the placement and texture operations for naevi and melanoma.
    /// </summary>
    public class LesionManager
    {
        private readonly Material _overlayMaterial;
        private readonly PathologyDataConfigurator _pathologyDataConfigurator;
        private readonly MelanomaConfigurator _configurator;

        public LesionManager(Material overlayMaterial, MelanomaConfigurator configurator,
            PathologyDataConfigurator pathologyDataConfigurator)
        {
            _overlayMaterial = overlayMaterial;
            _configurator = configurator;
            _pathologyDataConfigurator = pathologyDataConfigurator;
            _configurator.OnMelanomaUpdate.AddListener(delegate
            {
                UpdateLesions(Mathf.FloorToInt(_configurator.Selected.TextureCoord.x));
            });
        }

        // Handle melanoma placement using PathologyDatas
        public void HandleMelanomaPlacement(RaycastHit hit, ref bool placing, string segment)
        {
            if (_configurator.Selected == null) return;

            // TODO Melanoma too close not placeable
            //Selected.transform.position = hit.point;

            if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
            {
                if (_configurator.Selected.Placeable)
                {
                    int skinIndex = Mathf.FloorToInt(_configurator.Selected.TextureCoord.x);
                    _configurator.Selected.TextureCoord = hit.textureCoord;
                    _configurator.Selected.Position = hit.point;
                    _configurator.Selected.Placement = segment;

                    // Set Melanoma Collider for Melanoma Selection in the 3D View
                    MelanomaCollider.SetMelanomaCollider(_configurator.Selected);

                    // New Melanoma are only added to the List after they get textureCoords    
                    if (!_pathologyDataConfigurator.GetMelanomaList().Contains(_configurator.Selected))
                    {
                        _pathologyDataConfigurator.GetMelanomaList().Add(_configurator.Selected);
                    }
                    
                    UpdateLesions(skinIndex);
                    if (skinIndex != Mathf.FloorToInt(_configurator.Selected.TextureCoord.x))
                    {
                        UpdateLesions(Mathf.FloorToInt(_configurator.Selected.TextureCoord.x));
                    }

                    placing = false;
                }
            }
        }

        public void UpdateLesions()
        {
            for(int i = 0; i < _pathologyDataConfigurator.TextureCount; i++)
            {
                UpdateLesions(i);
            }
        }

        public void UpdateLesions(int skinIndex)
        {
            if(skinIndex < 0) return;
            
            _pathologyDataConfigurator.SetUpSkinTextures(skinIndex);

            foreach (Lesion m in _pathologyDataConfigurator.GetPathologiesList())
            {
                if (Mathf.FloorToInt(m.TextureCoord.x) == skinIndex)
                {
                    ApplyLesion(m, skinIndex);
                }
            }

            _pathologyDataConfigurator.ApplyTexture(skinIndex);
        }

        private void ApplyLesion(Lesion lesion, int skinIndex)
        {
            RenderTexture targetTexture = _pathologyDataConfigurator.GetSkinTexture(skinIndex);
            RenderTexture targetHeightTexture = _pathologyDataConfigurator.GetHeightTexture(skinIndex);
            if (targetTexture != null)
            {
                AddLesion(new Vector2(lesion.TextureCoord.x - skinIndex, lesion.TextureCoord.y), targetTexture, lesion,
                    lesion.Shape);
            }

            if (targetHeightTexture != null)
            {
                AddLesion(new Vector2(lesion.TextureCoord.x - skinIndex, lesion.TextureCoord.y), targetHeightTexture,
                    lesion, lesion.HeightMap);
            }
        }

        public void AddLesion(Vector2 uvPosition, RenderTexture skinTexture, Lesion lesion,
            RenderTexture lesionsTexture)
        {
            //RenderTexture lesionTexture = Resize(lesion.Shape, (int)(lesion.Shape.width), (int)(lesion.Shape.height));
            ApplyLesionToTexture(uvPosition, skinTexture, lesion, lesionsTexture);
        }

        void ApplyLesionToTexture(Vector2 uvPosition, RenderTexture skinTexture, Lesion lesion,
            RenderTexture lesionTexture)
        {
            // Berechne die halbe Breite und Höhe des Melanoms in Bezug auf die Hauttexturgröße und der Melanom-Größe
            float lesionHalfWidth = skinTexture.width * lesion.Size / 2.0f;
            float lesionHalfHeight = skinTexture.height * lesion.Size / 2.0f;

            // Passe die UV-Position an, sodass sie die Mitte der Melanom-Textur repräsentiert
            uvPosition.x -= lesionHalfWidth / skinTexture.width;
            uvPosition.y -= lesionHalfHeight / skinTexture.height;

            // Setze die Hauttextur in den Shader
            _overlayMaterial.SetTexture("_MainTex", skinTexture);

            // Setze die Melanom-RenderTexture in den Shader
            _overlayMaterial.SetTexture("_OverlayTex", lesionTexture);

            // Setze die UV-Position der Melanom-Textur (angepasst für die Mitte)
            _overlayMaterial.SetFloat("_OverlayPosX", uvPosition.x);
            _overlayMaterial.SetFloat("_OverlayPosY", uvPosition.y);

            // Berechne die Skalierung abhängig von der lesion.size
            float
                scale = lesion
                    .Size; // 1 -> Melanom ist so groß wie die Hauttextur, 0.1 -> Melanom ist 1/10 der Hauttextur
            _overlayMaterial.SetFloat("_OverlayScale", scale);

            // Führe den Shader in einer temporären RenderTexture aus und kopiere das Ergebnis in die Haupt-RenderTexture
            RenderTexture tempRT =
                RenderTexture.GetTemporary(skinTexture.width, skinTexture.height, 0, RenderTextureFormat.ARGB32);

            // Rendern des Overlays (Melanom) auf das aktuelle Ergebnis (skinTexture oder vorheriges Overlay)
            Graphics.Blit(skinTexture, tempRT, _overlayMaterial);

            // Kopiere das Ergebnis zurück in die Haupt-RenderTexture
            Graphics.Blit(tempRT, skinTexture);

            // Gib die temporäre RenderTexture frei
            RenderTexture.ReleaseTemporary(tempRT);
        }

        public void RemoveMelanoma(Melanoma melanoma)
        {
            if (_pathologyDataConfigurator.GetMelanomaList().Contains(melanoma))
            {
                int skinIndex = Mathf.FloorToInt(melanoma.TextureCoord.x);
                //TODO Melanoma COlliding with each other
                //Object.Destroy(melanoma.gameObject);
                _pathologyDataConfigurator.GetMelanomaList().Remove(melanoma);
                UpdateLesions(skinIndex);
            }
        }

        public void SelectMelanoma(Melanoma melanoma)
        {
            if (_pathologyDataConfigurator.GetMelanomaList().Contains(melanoma))
            {
                _configurator.Selected = melanoma;
            }
        }
    }
}
