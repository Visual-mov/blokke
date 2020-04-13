using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    FieldManager fm;
    Text scoreText;
    Text linesText;
    int score, lines, top;

    void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        scoreText = transform.Find("Stats").transform.Find("Score").GetComponent<Text>();
        //linesText = transform.Find("Lines").GetComponent<Text>();
        score = 0;
        lines = 0;
        top = 0;
    }

    public void AddToScore(int n) {
        score += n;
        scoreText.text = "Score:" + score;
    }
}
