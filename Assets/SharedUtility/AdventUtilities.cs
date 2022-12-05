using UnityEngine;

namespace SharedUtility
{
    public static class AdventUtilities
    {
        /// <summary>
        /// Take in the TextAsset and process it into a string array
        /// </summary>
        /// <param name="text">text asset with input data, \n delimited</param>
        /// <returns>Array of all lines as strings</returns>
        public static string[] ProcessTextAssetIntoStringArray(TextAsset text)
        {
            TextAsset inputAsset = text;

            string[] lineSeparators = new string[] { "\r\n" };
            string[] allLines = inputAsset.text.Split(lineSeparators, System.StringSplitOptions.None);

            Debug.Log($"Lines: {allLines.Length}");

            return allLines;
        }
    }
}