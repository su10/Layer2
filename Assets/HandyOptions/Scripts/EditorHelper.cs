#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jagapippi.Layer2
{
    public static class EditorHelper
    {
        public static T FindComponentInCurrentHierarchy<T>() where T : Component
        {
            var currentStageHandle = StageUtility.GetCurrentStageHandle();

            return currentStageHandle.IsValid()
                ? currentStageHandle.FindComponentOfType<T>()
                : Object.FindObjectOfType<T>();
        }
    }
}
#endif
