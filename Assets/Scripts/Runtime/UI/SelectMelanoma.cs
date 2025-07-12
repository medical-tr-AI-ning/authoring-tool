using Runtime.ScenarioConfiguration.Views.Pathologies;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class SelectMelanoma : MonoBehaviour
    {
        [SerializeField] private MelanomaConfigurator configurator;
        [SerializeField] private Image image;
        private PathologiesView view;


        void Start()
        {
            view = FindObjectsOfType<PathologiesView>()[0];
        }

        public void SelectThis()
        {
            configurator.SetTexture(image.sprite.texture);
            view.Drop();

        }
    }
}