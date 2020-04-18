using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    FieldManager fm;
    Text scoreText, linesText, levelText;
    int score, lines, level;

    void Awake() {
        //fm = GameObject.Find("Field").GetComponent<FieldManager>();
        Transform statsBoard = transform.Find("Stats").transform;
        scoreText = statsBoard.Find("Score").GetComponent<Text>();
        linesText = statsBoard.Find("Lines").GetComponent<Text>();
        linesText = statsBoard.Find("Level").GetComponent<Text>();
        score = 0;
        lines = 0;
    }

    public void AddToScore(int n) {
        score += n;
        scoreText.text = "Score:" + score;
    }
    public void AddLine() {
        lines += 1;
        linesText.text = "Lines:" + lines;
    }
}
