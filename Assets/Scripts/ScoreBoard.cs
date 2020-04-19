using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    FieldManager fm;
    Text scoreText, linesText, levelText;
    int score, lines, level;
    int nextUp, levelNum;

    void Awake() {
        levelNum = 1000;
        nextUp = levelNum;
        level = 1;
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        Transform statsBoard = transform.Find("Board").transform;
        scoreText = statsBoard.Find("Score").GetComponent<Text>();
        linesText = statsBoard.Find("Lines").GetComponent<Text>();
        levelText = statsBoard.Find("Level").GetComponent<Text>();
    }

    public void AddToScore(int n) {
        score += n;
        scoreText.text = "Score:" + score;
        if (score >= nextUp)
            IncrementLevel();
    }
    public void AddLine() {
        lines += 1;
        linesText.text = "Lines:" + lines;
    }

    public void IncrementLevel() {
        nextUp += levelNum;
        if (fm.fallTime >= 0.2) {
            level += 1;
            levelText.text = "Level:" + level;
            fm.fallTime -= 0.05f;
        }
    }

    public void ResetStats() {
        scoreText.text = "Score:0";
        linesText.text = "Lines:0";
        levelText.text = "Level:1";
        score = 0;
        lines = 0;
        level = 0;
        nextUp = 0;
    }
}
