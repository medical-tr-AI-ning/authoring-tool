using TMPro;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Anamnesis
{
    /// <summary>
    /// UI element representing a follow-up question to a top-level anamnesis question.
    /// </summary>
    public class FollowUpQuestion : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _followUpQuestionInputField;
        [SerializeField] private TMP_InputField _followUpAnswerInputField;
        private Question _parentQuestion;


        private string _question;

        public string Question
        {
            get => _followUpQuestionInputField.text;
            set => _followUpQuestionInputField.text = value;
        }

        public string Answer
        {
            get => _followUpAnswerInputField.text;
            set => _followUpAnswerInputField.text = value;
        }

        public void Start()
        {
            _parentQuestion = GetComponentInParent<Question>();
        }


        public void OnOrderUpPressed()
        {
            _parentQuestion.OrderUp(this);
        }

        public void OnOrderDownPressed()
        {
            _parentQuestion.OrderDown(this);
        }

        public void OnRemovePressed()
        {
            _parentQuestion.Remove(this);
        }
    }
}