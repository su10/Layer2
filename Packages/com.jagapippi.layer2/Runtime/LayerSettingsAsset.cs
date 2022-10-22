using System;
using UnityEngine;

namespace Jagapippi.Layer2
{
    [CreateAssetMenu(menuName = "Layer2/Layer Settings")]
    public class LayerSettingsAsset : ScriptableObject, ILayerSettings
    {
        [field: SerializeField] public LayerSettings settings { get; private set; }

        #region ILayerSettings

        public string LayerToName(int layer) => this.settings.LayerToName(layer);
        public int NameToLayer(string name) => this.settings.NameToLayer(name);
        public bool GetCollision(int layer1, int layer2) => this.settings.GetCollision(layer1, layer2);

        public event Action<ILayerSettings> changed
        {
            add => this.settings.changed += value;
            remove => this.settings.changed -= value;
        }

        #endregion
    }
}
