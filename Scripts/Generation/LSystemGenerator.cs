using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Profiling;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentence;
    [Range(0, 10)]
    public int iterationLimit = 1;
    [Range(0, 10)]
    public float ignoreRuleChance = 0.15f;

    private void Start() {
        Debug.Log(GenerateSentence());
    }
    
    public string GenerateSentence(string word = null) {
        if (word == null) {
            word = rootSentence;
        }
        return GrowRecursive(word);
    }

    private string GrowRecursive(string word, int iterationIndex = 0) {
        if(iterationIndex >= iterationLimit) {
            return word;
        }
        StringBuilder newWord = new StringBuilder();

        foreach (var c in word) {
            newWord.Append(c);
            ProcessRulesRecursively(newWord, c, iterationIndex);
        }

        return newWord.ToString();
    }

    private void ProcessRulesRecursively(StringBuilder newWord, char c, int iterationIndex) {
        foreach (var rule in rules) {
            if (rule.letter == c.ToString()) {
                
                if (Random.value < ignoreRuleChance && iterationIndex > 7) {
                    continue;
                }
                
                newWord.Append(GrowRecursive(rule.GetResult(), iterationIndex + 1));
            }
        }
    }
}
