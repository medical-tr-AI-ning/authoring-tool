using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    /// <summary>
    /// Texture & Material data associated with an agent.
    /// </summary>
    public class AgentData : MonoBehaviour
    {

        public Texture2D[] Tex;
        public Texture2D[] HeightTex;
        public Material[] Mat;
        public GameObject[] Clothes;
        public GameObject[] Genitalia;

    
        public void SetNaked(bool visible)
        {
            foreach (GameObject g in Clothes)
            {
                g.SetActive(!visible);
            }

            foreach(GameObject g in Genitalia)
            {
                g.SetActive(visible);
            }
        }
    }
}
