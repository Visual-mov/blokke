using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    FieldManager fm;
    Text scoreText;
    int score;

    void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        scoreText = transform.Find("Score").GetComponent<Text>();
        score = 0;
    }

    public void AddToScore(int n) {
        score += n;
        scoreText.text = "Score: " + score;
    }
}
