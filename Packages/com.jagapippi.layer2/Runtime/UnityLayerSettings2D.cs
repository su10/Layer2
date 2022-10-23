using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.Layer2
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal sealed class UnityLayerSettings2D : UnityLayerSettingsBase<UnityLayerSettings2D>
    {
#if UNITY_EDITOR
        static UnityLayerSettings2D() => OnInitializeOnLoad();
#endif

        public override PhysicsDimensions physicsDimensions => PhysicsDimensions.Two;
        public override string name => "Unity Default Settings (2D)";
        public override bool GetCollision(int layer1, int layer2) => (Physics2D.GetIgnoreLayerCollision(layer1, layer2) == false);
    }
}
