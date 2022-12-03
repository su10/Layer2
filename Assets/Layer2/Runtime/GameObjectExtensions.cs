using UnityEngine;

namespace Jagapippi.Layer2
{
    internal static class GameObjectExtensions
    {
        public static void Destroy(this GameObject go)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(go.gameObject);
            }
            else
            {
                Object.DestroyImmediate(go.gameObject);
            }
        }

        public static bool DontDestroyOnLoadActivated(this GameObject go)
        {
            const string sceneName = "DontDestroyOnLoad";
            return (go.scene.name == sceneName);
        }
    }
}
