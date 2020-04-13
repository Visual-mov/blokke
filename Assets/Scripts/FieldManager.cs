using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour {

    const int FWidth = 10;
    const int FHeight = 20;
    public int QueueLength = 3;
    public GameObject[] Blocks;
    public Queue<GameObject> BlockQueue;
    private GameObject[,] Field;
    private float LSide, RSide;

    public SideDisplay display;
    private ScoreBoard board;

    void Awake() {
        LSide = transform.position.x - FWidth / 2;
        RSide = transform.position.x + FWidth / 2;
        display = GameObject.Find("SideDisplay").GetComponent<SideDisplay>();
        board = GameObject.Find("Canvas").GetComponent<ScoreBoard>();
        Field = new GameObject[FWidth, FHeight];
        BlockQueue = new Queue<GameObject>();

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
    }

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
        for (int i = 0; i < QueueLength; i++) {
            BlockQueue.Enqueue(RandomBlock());
        }
        SpawnNextBlock();
    }

    // ValidateMove(): Checks if a given move hits constraints or other blocks, and if so returns false.
    public bool ValidateMove(Transform block, Vector4 move) {
        for (int i = 0; i < block.childCount; i++) {
            Vector3 pos = block.GetChild(i).position;
            if (CheckConstraints(pos + (Vector3)move) || CheckConstraints(CalcRotation(pos, block, move.w)))
                return false;
            for (int y = 0; y < FHeight; y++) {
                for (int x = 0; x < FWidth; x++) {
                    if (Field[x, y] == null) continue;
                    Transform FBlock = Field[x, y].transform;
                    if (FBlock.position == pos + (Vector3)move || FBlock.position == CalcRotation(pos, block, move.w))
                        return false;
                }
            }
        }
        return true;
    }
    
    // UpdateLines(): Loops through Field and deletes full lines while shifting all rows above.
    public void UpdateLines() {
        for(int y = 0; y < FHeight; y++) {
            while(RowFilled(y)) {
                board.AddToScore(100);
                RemoveRow(y);
                for(int i = y; i < FHeight; i++) {
                    for (int j = 0; j < FWidth; j++) {
                        GameObject block = Field[j, i];
                        if (i + 1 >= FHeight) {
                            Destroy(block);
                            Field[j, i] = null;
                        } else {
                            if(block != null) block.transform.Translate(Vector3.down,Space.World);
                            Field[j, i] = Field[j, i + 1];
                        }
                    }
                }
            }
        }
    }

    // AddToField(): Adds block to field array using rounded position as index, as sprite pivot is center.
    public void AddToField(Transform t) {
        board.AddToScore(10);
        for (int i = 0; i < t.childCount; i++) {
            Vector3 childPos = t.GetChild(i).transform.position;
            print(Mathf.FloorToInt(childPos.x - LSide) + " " + Mathf.FloorToInt(childPos.y));
            Field[Mathf.FloorToInt(childPos.x - LSide), Mathf.FloorToInt(childPos.y)] = t.GetChild(i).gameObject;
        }
        PrintField();
        UpdateLines();
    }

    // SpawnNextBlock(): Instantiates next block, and adds a random block to the end of the queue.
    public void SpawnNextBlock() {
        GameObject block = BlockQueue.Dequeue();
        SpawnBlock(block);
        BlockQueue.Enqueue(RandomBlock());
        display.UpdatePreview();
    }

    // SpawnBlock(): Instantiates given block with respect to field position.
    public void SpawnBlock(GameObject block) {
        Instantiate(block, block.transform.position + new Vector3(LSide, 0, 0), Quaternion.identity);
    }

    public GameObject RandomBlock() {
        return Blocks[Random.Range(0, 6)];
    }

    /* Helper Functions */

    // RowFilled(): Returns true if given row is full.
    private bool RowFilled(int row) {
        for (int x = 0; x < FWidth; x++) {
            if (Field[x, row] == null) return false;
        }
        return true;
    }

    // RemoveRow(): Removes given row from field array.
    private void RemoveRow(int row) {
        for (int x = 0; x < FWidth; x++) { 
            Destroy(Field[x, row]);
            Field[x, row] = null;
        }
    }

    // CheckConstraints(): Returns true if pos is within the field.
    private bool CheckConstraints(Vector3 pos) {
        return (pos.x >= RSide || pos.x <= LSide || pos.y <= 0) ? true : false;
    }

    // CalcRotation(): Calculates rotated vector with a counter-clockwise rotation of degree degrees
    private Vector3 CalcRotation(Vector2 pos, Transform block, float degree) {
        pos = block.InverseTransformPoint(pos);
        float a = degree * Mathf.PI / 180;
        float xp = pos.x * Mathf.Cos(a) - pos.y * Mathf.Sin(a);
        float yp = pos.x * Mathf.Sin(a) + pos.y * Mathf.Cos(a);
        return block.TransformPoint(new Vector3(xp, yp, 0));
    }
    // PrintField(): Prints text representation of field array. This is temporary and for debugging purposes.
    public void PrintField() {
        string field = "";
        for(int y = FHeight - 1; y >= 0; y--) {
            for(int x = 0; x < FWidth; x++) {
                if(Field[x,y] == null) {
                    field += "- ";
                } else {
                    field += "X ";
                }
            }
            field += "\n";
        }
        print(field);
    }
}
