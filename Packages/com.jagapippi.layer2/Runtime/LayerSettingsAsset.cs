using UnityEngine;

namespace Jagapippi.Layer2
{
    [CreateAssetMenu(menuName = "Layer2/Layer Settings")]
    public class LayerSettingsAsset : ScriptableObject
    {
        [field: SerializeField] public LayerSettings settings { get; private set; }
    }
}
