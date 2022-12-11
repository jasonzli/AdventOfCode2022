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
            for (int i = 0; i < register.ScreenRegister.Count; i++)
            {
                if (i % 40 == 0)
                {
                    Debug.Log($"{line}");
                    line = "";
                }
                line += register.ScreenRegister[i];
            }
        }

        class Register
        {
            private List<int> _xRegister = new List<int>();

            private int _registerValue = 1;

            public int SpecialSum => _xRegister[19]*20 + _xRegister[59]*60 + _xRegister[99]*100 + _xRegister[139]*140 +
                                     _xRegister[179]*180 + _xRegister[219]*220;

            public List<char> ScreenRegister
            {
                get { return _screenRegister; }
            }
            
            private int _pixelIndex = 0;
            private List<char> _screenRegister;
            private int _screenIndex = 0;

            public Register()
            {
                _screenRegister = new List<char>(239);
            }

            public void Cycle()
            {
                //Is our sprite visible in the cycle
                
                int activePixelIndex = _registerValue + _pixelIndex - 1; // shift the pixel Index over
                _pixelIndex = _pixelIndex + 1 % 3; //loop this pixel index back to 0

                char pixel = '.'; // default is .
                
                if (_screenIndex == activePixelIndex)
                {
                    pixel = '#'; //sprite is visible so use #
                }
                
                // Add pixel value and increment screen index;                
                //_screenRegister[_screenIndex] = pixel;
                _screenRegister.Add(pixel);
                _screenIndex++;
                
                //Loop
                if (_screenIndex == 239)
                {
                    _screenIndex = 0;
                }
                
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