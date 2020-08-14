using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    private int[] time;
    private FieldManager fm;
    public Text scoreText, linesText, levelText, timeText;
    private int score, lines, level;
    private int nextUp, levelNum;

    private void Awake() {
        levelNum = 1000;
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        time = new int[3];
        InitStats();
    }

    // Tick: Adds a second to time, rolls over seconds or minutes if equal to 60.
    public IEnumerator Tick() {
        while (true) {
            yield return new WaitForSeconds(1);
            time[0]++;
            for(int i = 0; i < time.Length - 1; i++) {
                if(time[i] == 60) {
                    time[i] = 0;
                    time[i + 1]++;
                }
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
}
