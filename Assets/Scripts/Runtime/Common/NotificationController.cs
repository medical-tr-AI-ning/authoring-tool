using Runtime.ScenarioList;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [SerializeField] private Transform _notificationContainer;
    [SerializeField] private Notification _notificationTemplate;

    public void CreateNotification(string text)
    {
        Notification notification = Instantiate(_notificationTemplate, _notificationContainer);
        notification.SetNotificationText(text);
    }
}
