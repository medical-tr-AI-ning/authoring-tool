using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.Common.Prompts
{
    /// <summary>
    /// Abstract class for all prompts that provide a response
    /// </summary>
    /// <typeparam name="T">Content of the expected response</typeparam>
    public abstract class ResponsePrompt<T> : MonoBehaviour
    {
        private TaskCompletionSource<T> _taskCompletionSource;
        public delegate void PromptClosedEvent();
        public event PromptClosedEvent PromptClosed;
        
         /// <summary>
         /// Displays the prompt to the user. Sets up the TaskCompletionSource used to notify the caller
         /// once the prompt has been completed.
         /// </summary>
         /// <returns>Task that finishes with the response data once the prompt has been completed.</returns>
        public Task<T> ShowPrompt()
        {
            _taskCompletionSource = new TaskCompletionSource<T>();
            gameObject.SetActive(true);
            return _taskCompletionSource.Task;
        }

        /// <summary>
        /// Completes the task with the given response. Destroys the prompt GameObject afterwards
        /// </summary>
        /// <param name="response"></param>
        protected void SendResponseAndClose(T response)
        {
            SendResponse(response);
            PromptClosed?.Invoke();
            Destroy(gameObject);
        }

        /// <summary>
        /// Completes the task with the given response.
        /// </summary>
        /// <param name="response"></param>
        protected void SendResponse(T response)
        {
            if (!ResponseIsValid(response))
            {
                Debug.LogWarning($"Response data in prompt {gameObject.name} was invalid!");
                return;
            }
            _taskCompletionSource.SetResult(response);
        }

        protected void CancelResponseAndClose()
        {
            _taskCompletionSource.SetCanceled();
            ClosePrompt();
            
        }

        public void ClosePrompt()
        {
            PromptClosed?.Invoke();
            Destroy(gameObject);
        }

        /// <summary>
        /// Checks whether the data entered by the user is complete and valid.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected abstract bool ResponseIsValid(T response);
    }
}