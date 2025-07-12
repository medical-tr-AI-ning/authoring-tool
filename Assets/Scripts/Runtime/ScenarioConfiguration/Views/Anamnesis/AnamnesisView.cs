using System.Collections.Generic;
using System.Linq;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Anamnesis
{
    /// <summary>
    /// Handles the "Anamnese" view UI and logic.
    /// </summary>
    public class AnamnesisView : PresetConfigurationView<PathologyVariant.AnamnesisData>, IPathologyVariantView
    {
        public override string PresetSubfolderName => "anamnesis";
        public override string PresetFileExtension => "anmconfig";
        [SerializeField] private GameObject questionsContainer;

        [SerializeField] private AnamnesisPathologyVariantHandler _pathologyVariantHandler;


        public override void Initialize()
        {
            _pathologyVariantHandler.Initialize();
        }

        public override void LoadConfiguration(PathologyVariant.AnamnesisData configuration, string resourcesFolder)
        {
            LoadSerializedQuestions(configuration.AnamnesisQuestions);
        }

        public override PathologyVariant.AnamnesisData GetConfiguration(string resourcesFolder)
        {
            return new PathologyVariant.AnamnesisData
            {
                AnamnesisQuestions = GetSerializedQuestionsData()
            };
        }

        /// <summary>
        /// Restores serialized anamnesis question data. Clears all existing data.
        /// </summary>
        /// <param name="serializedQuestions"></param>
        private void LoadSerializedQuestions(List<AnamnesisQuestion> serializedQuestions)
        {
            ClearQuestions();
            Question[] questionGameObjects = GetQuestions();
            foreach (AnamnesisQuestion serializedQuestion in serializedQuestions)
            {
                Question matchingQuestion =
                    questionGameObjects.First(q => q.QuestionID.Equals(serializedQuestion.QuestionID));
                matchingQuestion.SetAnswer(serializedQuestion.AnswerText);
                foreach (AnamnesisQuestion.FollowUpQuestion serializedFollowUpQuestion in serializedQuestion
                             .FollowUpQuestions)
                {
                    FollowUpQuestion followUpQuestion = matchingQuestion.AddFollowUpQuestion();
                    followUpQuestion.Question = serializedFollowUpQuestion.QuestionText;
                    followUpQuestion.Answer = serializedFollowUpQuestion.AnswerText;
                }
            }
        }

        private List<AnamnesisQuestion> GetSerializedQuestionsData()
        {
            List<AnamnesisQuestion> serializedQuestions = new List<AnamnesisQuestion>();
            Question[] questions = questionsContainer.GetComponentsInChildren<Question>(includeInactive: true);
            foreach (Question question in questions)
            {
                serializedQuestions.Add(question.GetSerializedQuestionData());
            }

            return serializedQuestions;
        }

        private Question[] GetQuestions() =>
            questionsContainer.GetComponentsInChildren<Question>(includeInactive: true);


        private void ClearQuestions()
        {
            foreach (Question question in GetQuestions())
                question.Clear();
        }

        public override bool ConfigurationIsValid(PathologyVariant.AnamnesisData configuration)
        {
            //TODO: Implement Proper configuration verification
            return true;
        }

        public void RequestCopyAnamnesisDataFromDefaultPathologyVariant()
        {
            _pathologyVariantHandler.OnCopyAnamnesisDataFromDefaultPathologyVariantRequested();
        }

        public void LoadDataFromVariant(PathologyVariant pathologyVariant)
        {
            LoadSerializedQuestions(pathologyVariant.Anamnesis.AnamnesisQuestions);
        }

        public void WriteUnhandledChangesToVariant(PathologyVariant pathologyVariant)
        {
            pathologyVariant.Anamnesis = new PathologyVariant.AnamnesisData
            {
                AnamnesisQuestions = GetSerializedQuestionsData()
            };
        }
    }
}