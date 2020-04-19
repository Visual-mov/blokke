using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour {

    public int QueueLength = 3;
    public GameObject[] Blocks;
    public Queue<GameObject> BlockQueue;
    public GameObject curBlock;
    public float fallTime;
    const int FWidth = 10;
    const int FHeight = 20;
    float LSide, RSide;
    GameObject[,] Field;

    public SideDisplay display;
    ScoreBoard board;

    void Awake() {
        LSide = transform.position.x - FWidth / 2;
        RSide = transform.position.x + FWidth / 2;
        fallTime = 1.0f;
        display = GameObject.Find("SideDisplay").GetComponent<SideDisplay>();
        board = GameObject.Find("Canvas").GetComponent<ScoreBoard>();
        Field = new GameObject[FWidth, FHeight];
        string[] names = {
            "I-Block", "J-Block",
            "L-Block", "O-Block",
            "S-Block", "T-Block",
            "Z-Block"
        };
        Blocks = new GameObject[names.Length];
        for (int i = 0; i < names.Length; i++) {
            Blocks[i] = (GameObject)Resources.Load("Prefabs/" + names[i]);
        }
        InitQueue();
    }

    // Field debug
    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            for (int i = 0; i < FHeight; i++) {
                for (int j = 0; j < FWidth; j++) {
                    if (Field[j, i] != null)
                        Gizmos.DrawSphere(Field[j, i].transform.position, 0.3f);
                }
            }
        }
    }

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
            for (int y = 0; y < FHeight; y++) {
                for (int x = 0; x < FWidth; x++) {
                    if (Field[x, y] == null)
                        continue;
                    Transform FBlock = Field[x, y].transform;
                    if (CompVector(FBlock.position, pos + (Vector2)move) || CompVector(FBlock.position, rotatedPos))
                        return false;
                }
            }
        }
        return true;
    }

    // UpdateLines(): Loops through Field and deletes full lines while shifting all rows above.
    public void UpdateLines() {
        for (int y = 0; y < FHeight; y++) {
            while (RowFilled(y)) {
                board.AddToScore(100);
                board.AddLine();
                //BlinkRow(y, 4, 0.2f);
                RemoveRow(y);
                for (int i = y; i < FHeight; i++) {
                    for (int j = 0; j < FWidth; j++) {
                        if (i + 1 >= FHeight)
                            RemoveRow(i);
                        else {
                            if (Field[j, i] != null) Field[j, i].transform.Translate(Vector2.down,Space.World);
                            Field[j, i] = Field[j, i + 1];
                        }
                    }
                }
            }
        }
    }

    void BlinkRow(int row, int cycles, float secPerCycle) {
        for (int i = 0; i < cycles; i++) {
            for (int x = 0; x < FWidth; x++) {
                Field[x, row].GetComponent<SpriteRenderer>().enabled = (i % 2 == 0) ? false : true;
            }
        }
    }

    // AddToField(): Adds block to field array using rounded position as index, as sprite pivot is center.
    public void AddToField(Transform t) {
        board.AddToScore(10);
        for (int i = 0; i < t.childCount; i++) {
            Vector2 childPos = t.GetChild(i).transform.position;
            Field[Mathf.FloorToInt(childPos.x - LSide), Mathf.FloorToInt(childPos.y)] = t.GetChild(i).gameObject;
        }
        UpdateLines();
    }

    // SpawnNextBlock(): Instantiates next block, and adds a random block to the end of the queue.
    public void SpawnNextBlock() {
        GameObject block = BlockQueue.Dequeue();
        block.GetComponent<Block>().fallTime = fallTime;
        SpawnBlock(block);
        BlockQueue.Enqueue(RandomBlock());
        display.UpdatePreview();
    }

    public void RestartGame() {
        for (int y = 0; y < FHeight; y++) {
            RemoveRow(y);
        }
        Destroy(curBlock);
        board.ResetStats();
        InitQueue();
        SpawnNextBlock();
    }

    /* Helper Functions */
    // RowFilled(): Returns true if given row is full.
    bool RowFilled(int row) {
        for (int x = 0; x < FWidth; x++) {
            if (Field[x, row] == null) return false;
        }
        return true;
    }

    // RemoveRow(): Removes given row from field array.
    void RemoveRow(int row) {
        for (int x = 0; x < FWidth; x++) { 
            Destroy(Field[x, row]);
            Field[x, row] = null;
        }
    }

    // CheckConstraints(): Returns true if pos is within the field.
    public bool CheckConstraints(Vector3 pos) {
        return (pos.x >= RSide || pos.x <= LSide || pos.y <= 0) ? true : false;
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
        curBlock = Instantiate(block, block.transform.position + new Vector3(LSide, 0, 0), Quaternion.identity);
    }

    public GameObject RandomBlock() {
        return Blocks[Random.Range(0, 7)];
    }

    bool CompVector(Vector2 a, Vector2 b) {
        return Vector2.SqrMagnitude(a - b) < 0.0001f;
    }

    void InitQueue() {
        BlockQueue = new Queue<GameObject>();
        for (int i = 0; i < QueueLength; i++) {
            BlockQueue.Enqueue(RandomBlock());
        }
    }
}
