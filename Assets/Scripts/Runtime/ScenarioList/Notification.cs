using TMPro;
using UnityEngine;

namespace Runtime.ScenarioList
{
    /// <summary>
    /// A notification that shows for a specified amount of seconds before destroying itself.
    /// </summary>
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TMP_Text _notificationText;
        [SerializeField] private float _displayLength;

        public void SetNotificationText(string text) => _notificationText.text = text;

        public void Start()
        {
            Destroy(gameObject, _displayLength);
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}