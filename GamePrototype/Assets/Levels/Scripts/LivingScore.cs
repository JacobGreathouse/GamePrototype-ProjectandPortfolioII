using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Ethan added this script
// Script that adds a score display for how long the player has survived

public class Score : MonoBehaviour
{
    public Text scoreText; // Assign this in the inspector
    private int score = 0;

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 60 == 0) // Update score every second assuming 60fps
        {
            score++;
            scoreText.text = "Score: " + score;
        }
    }

   void UpdateScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
    }
}


