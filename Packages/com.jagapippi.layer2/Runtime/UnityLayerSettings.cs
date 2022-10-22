using UnityEngine;

namespace Jagapippi.Layer2
{
    internal sealed class UnityLayerSettings : ILayerSettings
    {
        public static readonly UnityLayerSettings instance = new();

        private UnityLayerSettings()
        {
        }

        #region ILayerSettings

        public string LayerToName(int layer) => LayerMask.LayerToName(layer);
        public int NameToLayer(string name) => LayerMask.NameToLayer(name);
        public bool GetCollision(int layer1, int layer2) => (Physics.GetIgnoreLayerCollision(layer1, layer2) == false);

        #endregion
    }
}
