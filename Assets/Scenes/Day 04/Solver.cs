using System.Collections.Generic;
using System.Linq;
using SharedUtility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_04
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        private string[] _inputLines;

        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                Debug.Log($"Length: {handle.Result.text.Length}");

                _inputLines = AdventUtilities.ProcessTextAssetIntoStringArray(handle.Result);

                Process(_inputLines);
            };

        }

        void Process(string[] inputs)
        {
            // everything comes as "LB-UB,LB,UB" so you can divide it based on the comma
            List<RegionBounds> bounds = new List<RegionBounds>();

            int containingCount = 0;
            int overlappingCount = 0;
            
            foreach (string input in inputs)
            {
                //break up input based on ,
                string[] twoBounds = input.Split(',');

                string firstBoundInput = twoBounds[0];
                string secondBoundInput = twoBounds[1];

                string[] firstBoundEndpoints = firstBoundInput.Split('-');
                string[] secondBoundEndpoints = secondBoundInput.Split('-');

                int firstBoundLower = int.Parse(firstBoundEndpoints[0]);
                int firstBoundUpper = int.Parse(firstBoundEndpoints[1]);
                
                RegionBounds firstBound = new RegionBounds(firstBoundLower, firstBoundUpper);
                
                int secondBoundLower = int.Parse(secondBoundEndpoints[0]);
                int secondBoundUpper = int.Parse(secondBoundEndpoints[1]);
                
                RegionBounds secondBound = new RegionBounds(secondBoundLower, secondBoundUpper);

                if (firstBound.ContainsOtherBounds(secondBound) ||
                    secondBound.ContainsOtherBounds(firstBound))
                {
                    containingCount++;
                    Debug.Log($"Bounds contained! Found so far: {containingCount}");
                }

                if (firstBound.ContainsAnyOverlap(secondBound))
                {
                    overlappingCount++;
                    Debug.Log($"Bounds have overlap! Found so far: {overlappingCount}");
                }
            }
            
            Debug.Log($"Containing Count: {containingCount}");
            Debug.Log($"Overlapping Count: {overlappingCount}");

        }

        class RegionBounds
        {
            public int LowerBound { get; private set; }
            public int UpperBound { get; private set; }

            public RegionBounds(int lowerBound, int upperBound)
            {
                if (lowerBound > upperBound)
                {
                    Debug.LogWarning($"Upper bound {upperBound} is lower than the lower bound {lowerBound}");
                }
                
                LowerBound = lowerBound;
                UpperBound = upperBound;
            }

            /// <summary>
            /// Determines if this Bounds can contain the parameter bounds
            /// </summary>
            /// <param name="boundsToCompare">The bounds to check</param>
            /// <returns>True if the bounds are within the upper and lower bounds</returns>
            public bool ContainsOtherBounds(RegionBounds boundsToCompare)
            {
                return LowerBound <= boundsToCompare.LowerBound && UpperBound >= boundsToCompare.UpperBound;
            }

            public bool ContainsAnyOverlap(RegionBounds boundsToCompare)
            {
                return LowerBound <= boundsToCompare.UpperBound && UpperBound >= boundsToCompare.LowerBound;
            }
        }
    }
}