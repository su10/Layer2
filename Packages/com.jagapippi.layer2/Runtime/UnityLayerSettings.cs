using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal sealed class UnityLayerSettings : UnityLayerSettingsBase<UnityLayerSettings>
    {
#if UNITY_EDITOR
        static UnityLayerSettings() => OnInitializeOnLoad();
#endif

        public override PhysicsDimensions physicsDimensions => PhysicsDimensions.Three;
        public override string name => "Unity Default Settings";
        public override bool GetCollision(int layer1, int layer2) => (Physics.GetIgnoreLayerCollision(layer1, layer2) == false);
    }
}
