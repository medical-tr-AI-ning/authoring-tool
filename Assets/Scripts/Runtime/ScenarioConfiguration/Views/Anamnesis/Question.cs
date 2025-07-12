using System;
using System.Collections.Generic;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using TMPro;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Anamnesis
{
    /// <summary>
    /// UI element representing a top-level anamnesis question.
    /// </summary>
    [Serializable]
    public class Question : MonoBehaviour
    {
        private List<FollowUpQuestion> _followupQuestions;
        [SerializeField] private FollowUpQuestion _followUpQuestionTemplate;
        [SerializeField] private GameObject _followUpQuestionTargetContainer;
        [SerializeField] private TMP_InputField _answerInputField;
        [SerializeField] public string QuestionID;


        public FollowUpQuestion AddFollowUpQuestion()
        {
            FollowUpQuestion followUpQuestion =
                Instantiate(_followUpQuestionTemplate, _followUpQuestionTargetContainer.transform, false);
            followUpQuestion.gameObject.SetActive(true);
            return followUpQuestion;
        }

        public void OrderUp(FollowUpQuestion followUpQuestion)
        {
            MoveFollowUpQuestion(followUpQuestion, -1);
        }

        public void OrderDown(FollowUpQuestion followUpQuestion)
        {
            MoveFollowUpQuestion(followUpQuestion, 1);
        }

        private void MoveFollowUpQuestion(FollowUpQuestion followUpQuestion, int movement)
        {
            int newIndex = followUpQuestion.transform.GetSiblingIndex() + movement;
            if (newIndex < 0 || newIndex >= getFollowUpQuestionCount())
            {
                Debug.LogWarning("Invalid index for follow up question");
            }

            followUpQuestion.transform.SetSiblingIndex(newIndex);
        }

        public void Remove(FollowUpQuestion followUpQuestion)
        {
            Destroy(followUpQuestion.gameObject);
        }

        public AnamnesisQuestion GetSerializedQuestionData()
        {
            AnamnesisQuestion questionData = new AnamnesisQuestion
            {
                QuestionID = QuestionID,
                AnswerText = _answerInputField.text
            };
            List<AnamnesisQuestion.FollowUpQuestion> serializedFollowUpQuestions =
                new List<AnamnesisQuestion.FollowUpQuestion>();
            foreach (FollowUpQuestion followUpQuestion in getFollowUpQuestions())
            {
                serializedFollowUpQuestions.Add(new AnamnesisQuestion.FollowUpQuestion
                    { AnswerText = followUpQuestion.Answer, QuestionText = followUpQuestion.Question });
            }

            questionData.FollowUpQuestions = serializedFollowUpQuestions;
            return questionData;
        }

        public void Clear()
        {
            foreach (FollowUpQuestion followUpQuestion in getFollowUpQuestions())
            {
                Destroy(followUpQuestion.gameObject);
            }

            _answerInputField.text = string.Empty;
        }

        public void SetAnswer(string answer) => _answerInputField.text = answer;

        public void OnAddFollowUpQuestionPressed() =>
            AddFollowUpQuestion();


        private FollowUpQuestion[] getFollowUpQuestions() =>
            _followUpQuestionTargetContainer.transform.GetComponentsInChildren<FollowUpQuestion>(includeInactive: true);

        private int getFollowUpQuestionCount() => _followUpQuestionTargetContainer.transform.childCount;
    }
}