using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    FieldManager fm;
    Text scoreText, linesText, levelText;
    int score, lines, level;
    int nextUp;

    void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        Transform statsBoard = transform.Find("Stats").transform;
        scoreText = statsBoard.Find("Score").GetComponent<Text>();
        linesText = statsBoard.Find("Lines").GetComponent<Text>();
        levelText = statsBoard.Find("Level").GetComponent<Text>();
        nextUp = 1000;
    }

    public void AddToScore(int n) {
        score += n;
        scoreText.text = "Score:" + score;
        if (score >= nextUp)
            incrementLevel();
    }
    public void AddLine() {
        lines += 1;
        linesText.text = "Lines:" + lines;
    }

    public void incrementLevel() {
        nextUp += 1000;
        if (fm.fallTime >= 0.2) {
            level += 1;
            levelText.text = "Level:" + level;
            fm.fallTime -= 0.05f;
        }
    }
}
