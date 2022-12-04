using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_02
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

                TextAsset inputAsset = handle.Result;

                string[] allLines = inputAsset.text.Split('\n');

                Debug.Log($"Lines: {allLines.Length}");

                _inputLines = allLines;
                
                //create games from all lines
                List<Game> games = new List<Game>();
                foreach (string line in allLines)
                {
                    games.Add(new Game(line));
                }

                int score = games.Sum(game => game.YourResult());
                
                Debug.Log($"Score: {score}");
                
                //create games from all lines
                List<GameRevised> revisedGames = new List<GameRevised>();
                foreach (string line in allLines)
                {
                    revisedGames.Add(new GameRevised(line));
                }

                int revisedScore = revisedGames.Sum(game => game.YourResult());
                
                Debug.Log($"Revised Score: {revisedScore}");
            };
            
        }

        class Game
        {
            private string _firstPlayerChoice;
            private string _secondPlayerChoice;

            // Rock : A , X
            // Paper: B , Y
            // Scissors: C , Z
            private static Dictionary<string, int> resultsDictionary = new Dictionary<string, int>
            {
                {"AX", 3}, // Rock Rock
                {"AY", 6}, // Rock Paper
                {"AZ", 0}, // Rock Scissors
                {"BX", 0}, // Paper Rock
                {"BY", 3}, // Paper Paper
                {"BZ", 6}, // Paper Scissors
                {"CX", 6}, // Scissors Rock
                {"CY", 0}, // Scissors Paper
                {"CZ", 3}, // Scissors Scissors
            };
            public Game(string choices)
            {
                string[] choicesArray = choices.Split(' ');
                _firstPlayerChoice = choicesArray[0];
                _secondPlayerChoice = choicesArray[1];
            }
            

            public int YourResult()
            {
                int scoreBasedOnChoice = ValueOfChoice();
                int scoreFromGame = ValueOfGame();

                return scoreFromGame + scoreBasedOnChoice;
            }

            private int ValueOfGame()
            {
                string key = _firstPlayerChoice+ _secondPlayerChoice;
                key = key.Trim();
                return resultsDictionary[key];
            }
            
            private int ValueOfChoice()
            {
                switch (_secondPlayerChoice.Trim())
                {
                    case "X": return 1;
                    case "Y": return 2;
                    case "Z": return 3;
                    default: return 0;
                }
            }
        }
        
        class GameRevised
        {
            private string _firstPlayerChoice;
            private string _secondPlayerChoice;

            //X means you need to lose
            //Y means you need to draw
            //Z means you need to win
            
            // Rock : A , A
            // Paper: B , B
            // Scissors: C , C
            private static Dictionary<string, int> resultsDictionary = new Dictionary<string, int>
            {
                {"AA", 3}, // Rock Rock
                {"AB", 6}, // Rock Paper
                {"AC", 0}, // Rock Scissors
                {"BA", 0}, // Paper Rock
                {"BB", 3}, // Paper Paper
                {"BC", 6}, // Paper Scissors
                {"CA", 6}, // Scissors Rock
                {"CB", 0}, // Scissors Paper
                {"CC", 3}, // Scissors Scissors
            };
            
            private static Dictionary<string, string> ChoiceFromResult = new Dictionary<string, string>
            {
                {"AX", "C"}, //Lose
                {"AY", "A"}, //Tie
                {"AZ", "B"}, //Win
                {"BX", "A"}, //Lose
                {"BY", "B"}, //Tie
                {"BZ", "C"}, //Win
                {"CX", "B"}, //Lose
                {"CY", "C"}, //Tie
                {"CZ", "A"}, //Win
            };

            public GameRevised(string choices)
            {
                string[] choicesArray = choices.Split(' ');
                _firstPlayerChoice = choicesArray[0];

                string choiceAndResult = _firstPlayerChoice + choicesArray[1];
                _secondPlayerChoice = ChoiceFromResult[choiceAndResult.Trim()];

            }
            

            public int YourResult()
            {
                int scoreBasedOnChoice = ValueOfChoice();
                int scoreFromGame = ValueOfGame();

                return scoreFromGame + scoreBasedOnChoice;
            }

            private int ValueOfGame()
            {
                string key = _firstPlayerChoice+ _secondPlayerChoice;
                key = key.Trim();
                return resultsDictionary[key];
            }
            
            private int ValueOfChoice()
            {
                switch (_secondPlayerChoice.Trim())
                {
                    case "A": return 1;
                    case "B": return 2;
                    case "C": return 3;
                    default: return 0;
                }
            }
        }
    }
}