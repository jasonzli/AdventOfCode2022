using System.Collections.Generic;
using System.Linq;
using Palmmedia.ReportGenerator.Core;
using SharedUtility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_03
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
            List<Rucksack> rucksacks = new List<Rucksack>();

            foreach (string input in inputs)
            {
                rucksacks.Add(new Rucksack(input));
            }

            Debug.Log($"Total priority values: {rucksacks.Sum(sack => sack.MatchingItemValue)}");

            Debug.Log($"Finding badges...");
            
            List<ElfGroup> elfGroups = new List<ElfGroup>();

            int count = 0;
            List<Rucksack> groups = new List<Rucksack>();
            foreach (Rucksack rucksack in rucksacks)
            {
                groups.Add(rucksack);
                count++;
                
                if (count == 3)
                {
                    elfGroups.Add(new ElfGroup(groups));
                    groups.Clear();
                    count = 0;
                }
            }
            Debug.Log($"Total groups: {elfGroups.Count}");
            
            Debug.Log($"The sum of the badge numbers is: {elfGroups.Sum(group => group.BadgeValue)}");
        }

        class Rucksack
        {
            private static Dictionary<char, int> _priorityTable = new Dictionary<char, int>
            {
                {'a', 1},
                {'b', 2},
                {'c', 3},
                {'d', 4},
                {'e', 5},
                {'f', 6},
                {'g', 7},
                {'h', 8},
                {'i', 9},
                {'j', 10},
                {'k', 11},
                {'l', 12},
                {'m', 13},
                {'n', 14},
                {'o', 15},
                {'p', 16},
                {'q', 17},
                {'r', 18},
                {'s', 19},
                {'t', 20},
                {'u', 21},
                {'v', 22},
                {'w', 23},
                {'x', 24},
                {'y', 25},
                {'z', 26},
                {'A', 27},
                {'B', 28},
                {'C', 29},
                {'D', 30},
                {'E', 31},
                {'F', 32},
                {'G', 33},
                {'H', 34},
                {'I', 35},
                {'J', 36},
                {'K', 37},
                {'L', 38},
                {'M', 39},
                {'N', 40},
                {'O', 41},
                {'P', 42},
                {'Q', 43},
                {'R', 44},
                {'S', 45},
                {'T', 46},
                {'U', 47},
                {'V', 48},
                {'W', 49},
                {'X', 50},
                {'Y', 51},
                {'Z', 52},
            };

            private HashSet<char> _leftPack = new HashSet<char>();
            private HashSet<char> _rightPack = new HashSet<char>();

            private char _matchingItem;
            public int MatchingItemValue => _priorityTable[_matchingItem];
            public HashSet<char> FullPack { get; private set; }

            public Rucksack(string items)
            {
                string left = items.Substring(0, items.Length / 2);
                string right = items.Substring(items.Length / 2);

                foreach (char c in left)
                {
                    _leftPack.Add(c);
                }

                foreach (char r in right)
                {
                    _rightPack.Add(r);
                }

                FullPack = new HashSet<char>(_leftPack.Concat(_rightPack));
                
                
                _matchingItem = GetMatchingItem();
                Debug.Log(
                    $"Left sack: {left}\nRight sack:{right}\nMatching item: {_matchingItem}\nMatching item value: {MatchingItemValue}");

            }


            private char GetMatchingItem()
            {
                return _leftPack.Intersect(_rightPack).ToList()[0];
            }
        }

        class ElfGroup
        {
            private Rucksack[] _rucksacks;
            private char _identityBadge;
            private static Dictionary<char, int> _priorityTable = new Dictionary<char, int>
            {
                {'a', 1},
                {'b', 2},
                {'c', 3},
                {'d', 4},
                {'e', 5},
                {'f', 6},
                {'g', 7},
                {'h', 8},
                {'i', 9},
                {'j', 10},
                {'k', 11},
                {'l', 12},
                {'m', 13},
                {'n', 14},
                {'o', 15},
                {'p', 16},
                {'q', 17},
                {'r', 18},
                {'s', 19},
                {'t', 20},
                {'u', 21},
                {'v', 22},
                {'w', 23},
                {'x', 24},
                {'y', 25},
                {'z', 26},
                {'A', 27},
                {'B', 28},
                {'C', 29},
                {'D', 30},
                {'E', 31},
                {'F', 32},
                {'G', 33},
                {'H', 34},
                {'I', 35},
                {'J', 36},
                {'K', 37},
                {'L', 38},
                {'M', 39},
                {'N', 40},
                {'O', 41},
                {'P', 42},
                {'Q', 43},
                {'R', 44},
                {'S', 45},
                {'T', 46},
                {'U', 47},
                {'V', 48},
                {'W', 49},
                {'X', 50},
                {'Y', 51},
                {'Z', 52},
            };

            public int BadgeValue => _priorityTable[_identityBadge];
            
            public ElfGroup(IEnumerable<Rucksack> rucksacks)
            {
                _rucksacks = rucksacks.ToArray();
                _identityBadge = FindMatchingItemInAllRucksacks();
            }

            private char FindMatchingItemInAllRucksacks()
            {
                HashSet<char> firstElfRucksack = _rucksacks[0].FullPack;
                HashSet<char> secondElfRucksack = _rucksacks[1].FullPack;
                HashSet<char> thirdElfRucksack = _rucksacks[2].FullPack;

                Debug.Log($"Checking: {new string(firstElfRucksack.ToArray())}\nSecond String: {new string(secondElfRucksack.ToArray())}\nThird String: {new string(thirdElfRucksack.ToArray())}");
                    
                //if there is no intersection, return 0
                List<char> matchingItem =
                    firstElfRucksack.Intersect(secondElfRucksack).Intersect(thirdElfRucksack).ToList();

                if (matchingItem.Count == 0)
                {
                    return '0';
                }
                Debug.Log($"Found: {matchingItem[0]}");
                return matchingItem[0];
            }
        }

}
}