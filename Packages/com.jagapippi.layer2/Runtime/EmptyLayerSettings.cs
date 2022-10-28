using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
    internal class EmptyLayerSettings : ILayerSettings
    {
        public static readonly EmptyLayerSettings instance = new();

#if UNITY_EDITOR
        private static readonly UnityEngine.Object PhysicsManager = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(PhysicsManager));
        private static readonly UnityEngine.Object Physics2DSettings = Unsupported.GetSerializedAssetInterfaceSingleton(nameof(Physics2DSettings));

        [InitializeOnLoadMethod]
        static void OnInitializeOnLoadMethod()
        {
            var layers = instance.GetNames();

            var physics = new SerializedObject(PhysicsManager);
            var collisionMatrixProperty = physics.FindProperty("m_LayerCollisionMatrix");
            var collisionMatrix = new uint[Layer.MaxCount];

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                collisionMatrix[i] = (uint)collisionMatrixProperty.GetArrayElementAtIndex(i).longValue;
            }

            var physics2D = new SerializedObject(Physics2DSettings);
            var collisionMatrix2DProperty = physics2D.FindProperty("m_LayerCollisionMatrix");
            var collisionMatrix2D = new uint[Layer.MaxCount];

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                collisionMatrix2D[i] = (uint)collisionMatrix2DProperty.GetArrayElementAtIndex(i).longValue;
            }

            EditorApplication.update += () =>
            {
                var changed = false;

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    var layerName = LayerMask.LayerToName(i);
                    if (layerName == layers[i]) continue;

                    layers[i] = layerName;
                    changed = true;
                }

                // Physics
                {
                    physics.Update();

                    for (var i = 0; i < Layer.MaxCount; i++)
                    {
                        var newValue = (uint)collisionMatrixProperty.GetArrayElementAtIndex(i).longValue;

                        if (newValue != collisionMatrix[i]) changed = true;

                        collisionMatrix[i] = newValue;
                    }
                }
                // Physics2D
                {
                    physics2D.Update();

                    for (var i = 0; i < Layer.MaxCount; i++)
                    {
                        var newValue = (uint)collisionMatrix2DProperty.GetArrayElementAtIndex(i).longValue;

                        if (newValue != collisionMatrix2D[i]) changed = true;

                        collisionMatrix2D[i] = newValue;
                    }
                }

                if (changed) instance.changedSerializedObject?.Invoke(instance);
            };
        }
#endif

        private EmptyLayerSettings()
        {
        }

        #region ILayerSettings

        public string name => "Project Settings";
        public string LayerToName(int layer) => LayerMask.LayerToName(layer);
        public int NameToLayer(string layerName) => LayerMask.NameToLayer(layerName);
        public bool GetCollision(int layer1, int layer2) => (Physics.GetIgnoreLayerCollision(layer1, layer2) == false);
        public bool GetCollision2D(int layer1, int layer2) => (Physics2D.GetIgnoreLayerCollision(layer1, layer2) == false);
#if UNITY_EDITOR
        public event Action<ILayerSettings> changedSerializedObject;
#endif

        #endregion
    }
}
