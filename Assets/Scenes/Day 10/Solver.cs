using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtility;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_10
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        private string[] _inputLines;

        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                _inputLines = AdventUtilities.ProcessTextAssetIntoStringArray(handle.Result);
                Process(_inputLines);
            };

        }

        void Process(string[] inputs)
        {
            Register register = new Register();
            
            foreach (string command in inputs)
            {
                
                string[] splitCommand = command.Split(' ');
                string action = splitCommand[0];

                if (splitCommand.Length < 2)
                {
                    register.Command(action);
                    continue;
                }

                int.TryParse(splitCommand[1],out int value);
                register.Command(action, value);

            }
            
            Debug.Log($"The register sum is: {register.SpecialSum}");
         
            // Print the register's screenregister in rows of 40 characters

            string line = "";
            int spritePixelToDraw = 0;
            for (int i = 0; i < register.SpriteRegister.Count; i++)
            {
                int spritePosition = register.SpriteRegister[i];

                int currentDrawPosition = i % 40;

                string characterToAdd = ".";
                if (Math.Abs(spritePosition - currentDrawPosition) <= 1)
                {
                    characterToAdd = "#";
                }

                line += characterToAdd;
                spritePixelToDraw = spritePixelToDraw + 1 % 3;
                if ((i + 1) % 40 == 0)
                {
                    line += "\n";
                }
            }

            Debug.Log($"{line}");
            
        }

        class Register
        {
            private List<int> _xRegister = new List<int>();

            public List<int> SpriteRegister => _xRegister;
            
            
            private int _registerValue = 1;

            public int SpecialSum => _xRegister[19]*20 + _xRegister[59]*60 + _xRegister[99]*100 + _xRegister[139]*140 +
                                     _xRegister[179]*180 + _xRegister[219]*220;

            public Register()
            {
            }

            public void Cycle()
            {
                //Update the register position
                _xRegister.Add(_registerValue);
            }

            public void AddX(int valueAdd)
            {
                Cycle();
                Cycle();
                _registerValue += valueAdd;
            }

            public void Command(string command, int value = 0)
            {
                switch (command)
                {
                    case "noop":
                        Cycle();
                        break;
                    case "addx":
                        AddX(value);
                        break;
                    default:
                        break;
                }
            }
        }
        
    }
}