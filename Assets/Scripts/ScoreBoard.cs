using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    int[] time;
    FieldManager fm;
    Text scoreText, linesText, levelText, timeText;
    int score, lines, level;
    int nextUp, levelNum;

    void Awake() {
        levelNum = 1000;
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        Transform statsBoard = transform.Find("ScoreBoard").transform;
        scoreText = statsBoard.Find("Score").GetComponent<Text>();
        linesText = statsBoard.Find("Lines").GetComponent<Text>();
        levelText = statsBoard.Find("Level").GetComponent<Text>();
        timeText = statsBoard.Find("Time").GetComponent<Text>();
        time = new int[3];
        InitStats();
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

    public void UpdateTime() {
        timeText.text = $"{time[2]}:{time[1].ToString("00")}:{time[0].ToString("00")}";
    }

    public IEnumerator Tick() {
        while (true) {
            yield return new WaitForSeconds(1);
            time[0]++;
            if (time[0] == 60) {
                time[0] = 0;
                time[1]++;
            }
            if (time[1] == 60) {
                time[1] = 0;
                time[2]++;
            }
        }
    }

    // IncrementLevel: Increases level and difficulty (speed of block's decent)
    public void IncrementLevel() {
        nextUp += levelNum;
        if (fm.fallTime >= 0.2) {
            level += 1;
            levelText.text = "Level:" + level;
            fm.fallTime -= 0.05f;
        }
    }

    // InitStats: Resets state of score board.
    public void InitStats() {
        StartCoroutine("Tick");
        scoreText.text = "Score:0";
        linesText.text = "Lines:0";
        levelText.text = "Level:1";
        timeText.text = "0:00:00";
        score = 0;
        lines = 0;
        level = 1;
        nextUp = levelNum;
        time = new int[3];
    }
}
