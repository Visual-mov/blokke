﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldManager : MonoBehaviour {

    public int queueLength = 3;
    public GameObject[] blocks;
    public Queue<GameObject> blockQueue;
    public GameObject curBlock;
    public float fallTime;
    const int fWidth = 10;
    const int fHeight = 20;
    float lSide, rSide;
    GameObject[,] field;
    Text overText;

    public SideDisplay display;
    ScoreBoard board;

    void Awake() {
        lSide = transform.position.x - fWidth / 2;
        rSide = transform.position.x + fWidth / 2;
        fallTime = 1.0f;
        display = GameObject.Find("SideDisplay").GetComponent<SideDisplay>();
        board = GameObject.Find("Canvas").GetComponent<ScoreBoard>();
        field = new GameObject[fWidth, fHeight];
        overText = GameObject.Find("Canvas").transform.Find("OverText").GetComponent<Text>();
        string[] names = {
            "I-Block", "J-Block",
            "L-Block", "O-Block",
            "S-Block", "T-Block",
            "Z-Block"
        };
        blocks = new GameObject[names.Length];
        for (int i = 0; i < names.Length; i++) {
            blocks[i] = (GameObject)Resources.Load("Prefabs/" + names[i]);
        }
        InitQueue();
    }

    // Field debug
    //private void OnDrawGizmos() {
    //    if (Application.isPlaying) {
    //        for (int i = 0; i < fHeight; i++) {
    //            for (int j = 0; j < fWidth; j++) {
    //                if (Field[j, i] != null)
    //                    Gizmos.DrawSphere(Field[j, i].transform.position, 0.3f);
    //            }
    //        }
    //    }
    //}

    void Start() {
        SpawnNextBlock();
    }

    // ValidateMove(): Checks if a given move hits constraints or other blocks, and if so returns false.
    public bool ValidateMove(Transform block, Vector4 move) {
        for (int i = 0; i < block.childCount; i++) {
            Vector2 pos = block.GetChild(i).position;
            Vector2 rotatedPos = CalcRotation(pos, block, move);
            if (CheckConstraints(pos + (Vector2)move) || CheckConstraints(rotatedPos))
                return false;
            for (int y = 0; y < fHeight; y++) {
                for (int x = 0; x < fWidth; x++) {
                    if (field[x, y] == null)
                        continue;
                    Transform FBlock = field[x, y].transform;
                    if (CompVector(FBlock.position, pos + (Vector2)move) || CompVector(FBlock.position, rotatedPos))
                        return false;
                }
            }
        }
        return true;
    }

    // UpdateLines(): Loops through Field and deletes full lines while shifting all rows above.
    public void UpdateLines() {
        for (int y = 0; y < fHeight; y++) {
            while (RowFilled(y)) {
                board.AddToScore(100);
                board.AddLine();
                //BlinkRow(y, 4, 0.2f);
                RemoveRow(y);
                for (int i = y; i < fHeight; i++) {
                    for (int j = 0; j < fWidth; j++) {
                        if (i + 1 >= fHeight)
                            RemoveRow(i);
                        else {
                            if (field[j, i] != null) field[j, i].transform.Translate(Vector2.down,Space.World);
                            field[j, i] = field[j, i + 1];
                        }
                    }
                }
            }
        }
    }

    void BlinkRow(int row, int cycles, float secPerCycle) {
        for (int i = 0; i < cycles; i++) {
            for (int x = 0; x < fWidth; x++) {
                field[x, row].GetComponent<SpriteRenderer>().enabled = (i % 2 == 0) ? false : true;
            }
        }
    }

    // AddToField(): Adds block to field array using rounded position as index, as sprite pivot is center.
    public void AddToField(Transform t) {
        board.AddToScore(10);
        for (int i = 0; i < t.childCount; i++) {
            Vector2 childPos = t.GetChild(i).transform.position;
            field[Mathf.FloorToInt(childPos.x - lSide), Mathf.FloorToInt(childPos.y)] = t.GetChild(i).gameObject;
        }
        UpdateLines();
    }

    // SpawnNextBlock(): Instantiates next block, and adds a random block to the end of the queue.
    public void SpawnNextBlock() {
        GameObject block = blockQueue.Dequeue();
        block.GetComponent<Block>().fallTime = fallTime;
        SpawnBlock(block);
        blockQueue.Enqueue(RandomBlock());
        display.UpdatePreview();
    }

    public void RestartGame() {
        overText.enabled = false;
        Destroy(curBlock);
        board.InitStats();
        InitQueue();
        display.RemoveHeld();
        for (int y = 0; y < fHeight; y++) RemoveRow(y);
        SpawnNextBlock();
    }

    public void EndGame() {
        overText.enabled = true;
        Destroy(curBlock);
    }

    /* Helper Functions */
    // RowFilled(): Returns true if given row is full.
    bool RowFilled(int row) {
        for (int x = 0; x < fWidth; x++) {
            if (field[x, row] == null) return false;
        }
        return true;
    }

    // RemoveRow(): Removes given row from field array.
    void RemoveRow(int row) {
        for (int x = 0; x < fWidth; x++) { 
            Destroy(field[x, row]);
            field[x, row] = null;
        }
    }

    // CheckConstraints(): Returns true if pos is within the field.
    public bool CheckConstraints(Vector3 pos) {
        return (pos.x >= rSide || pos.x <= lSide || pos.y <= 0) ? true : false;
    }

    // CalcRotation(): Calculates rotated vector with a counter-clockwise rotation of degree degrees
    Vector3 CalcRotation(Vector2 pos, Transform block, Vector4 move) {
        pos = block.InverseTransformPoint(pos);
        float a = move.w * Mathf.PI / 180;
        float xp = pos.x * Mathf.Cos(a) - pos.y * Mathf.Sin(a);
        float yp = pos.x * Mathf.Sin(a) + pos.y * Mathf.Cos(a);
        return block.TransformPoint(new Vector3(xp, yp, 0)) + (Vector3)move;
    }

    // SpawnBlock(): Instantiates given block with respect to field position.
    public void SpawnBlock(GameObject block) {
        curBlock = Instantiate(block, block.transform.position + new Vector3(lSide, 0, 0), Quaternion.identity);
        if (!ValidateMove(curBlock.transform, Vector3.zero)) {
            EndGame();
        }
    }

    public GameObject RandomBlock() {
        return blocks[Random.Range(0, blocks.Length)];
    }

    bool CompVector(Vector2 a, Vector2 b) {
        return Vector2.SqrMagnitude(a - b) < 0.0001f;
    }

    void InitQueue() {
        blockQueue = new Queue<GameObject>();
        for (int i = 0; i < queueLength; i++) {
            blockQueue.Enqueue(RandomBlock());
        }
    }
}
