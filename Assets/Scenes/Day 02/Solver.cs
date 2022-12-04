using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_02
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                
            };
            
        }
    }
}