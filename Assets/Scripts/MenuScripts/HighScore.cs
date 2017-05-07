using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    private List<KeyValuePair<string, int>> highScores; //A collection of all scores and scorers

    // Use this for initialization
    void Start()
    {
        DeserializeHighScores();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Get all currently saved scores
    private void DeserializeHighScores()
    {
        //Create new dictionary to store in
        highScores = new List<KeyValuePair<string, int>>();

        //Take the string saved in player prefs, and chop it up. Save the chopped pieces into the dictionary
        string[] stringifiedScores = PlayerPrefs.GetString("HighScores").Split(',');
        foreach(string scorePair in stringifiedScores)
        {
            string[] valuePair = scorePair.Split(':');
            highScores.Add(new KeyValuePair<string, int>(valuePair[0], int.Parse(valuePair[1])));
        }
    }

    //Save scores to the player preferences
    public void SerializeHighScores()
    { 
        string stringifiedScores = string.Empty;

        //for each score, save it in this format to the stringified version NAME:SCORE,
        foreach(KeyValuePair<string, int> score in highScores)
        {
            stringifiedScores += score.Key + ':' + score.Value + ',';
        }

        //Remove the trailing comma and save
        stringifiedScores = stringifiedScores.Remove(stringifiedScores.Length - 1);
        PlayerPrefs.SetString("HighScores", stringifiedScores);
    }

    //Clear all high scores saved in player prefs
    public void ClearHighScores()
    {
        PlayerPrefs.SetString("HighScores", string.Empty);
    }

    //Return an array of key value pairs in sorted order by score
    public KeyValuePair<string, int>[] GetSortedHighScores()
    {
        List<KeyValuePair<string, int>> sorted = new List<KeyValuePair<string, int>>();
        foreach(KeyValuePair<string,int> kvp in highScores)
        {
            sorted.Add(kvp);
        }

        return sorted.OrderByDescending(o => o.Value).ToArray();
    }

    //Saves a new score, and determines its rank in the leaderboard
    public int DetermineScoreRankingAndSave(string name, int score)
    {
        //Get sorted before saving (so we don't overlap)
        KeyValuePair<string, int>[] sorted = GetSortedHighScores();

        //Save the score and serialize scores
        highScores.Add(new KeyValuePair<string,int>(name, score));
        SerializeHighScores();

        //Determine where the new score fits into the ranking
        for (int i = 0; i < sorted.Length; i++)
        {
            if (score > sorted[i].Value)
            {
                return i + 1;
            }
        }
        return sorted.Length + 1;
    }
}
