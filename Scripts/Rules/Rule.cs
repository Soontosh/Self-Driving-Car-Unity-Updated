using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

[CreateAssetMenu(menuName ="ProceduralTown/Rule")]
public class Rule : ScriptableObject
{
    [SerializeField]
    private string[] results = null;
    [SerializeField]
    bool randomResult = true;
    bool poop = false;
    public string letter;
    
    public string GetResult() {
        if (false) {
            int randomIndex = Random.Range(0, results.Length);
            string newString = RandomizeResult(results[randomIndex]);
            Debug.Log("New String Var: " + newString);
            return newString;
        }
        return results[Random.Range(0, results.Length)];
    }

    
    public string RandomizeResult(string currentString) {
        var RandomNumbersCount = Enumerable.Range(0, 3);
        StringBuilder stringBuilder = new StringBuilder(currentString);
        string possibleChars = "F+-";
        foreach (var number in RandomNumbersCount) {
            int RandomIndex = Random.Range(0, stringBuilder.Length);
            char randomChar = possibleChars[new System.Random().Next(possibleChars.Length)];
            stringBuilder.Insert(RandomIndex, randomChar);
        }
        return stringBuilder.ToString();
    }
}

