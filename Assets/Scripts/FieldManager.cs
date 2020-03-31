using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour {

    const int FWidth = 10;
    const int FHeight = 20;
    const int QueueLength = 3;
    public GameObject[] Blocks;
    public Queue<GameObject> BlockQueue;
    private GameObject[,] Field;
    private float LSide, RSide;

    private SideDisplay display;

    void Awake() {
        LSide = transform.position.x - FWidth / 2;
        RSide = transform.position.x + FWidth / 2;
        display = GameObject.Find("SideDisplay").GetComponent<SideDisplay>();
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

    void Start() {
        for (int i = 0; i < QueueLength; i++) {
            BlockQueue.Enqueue(RandomBlock());
        }
        SpawnNextBlock();
    }

    // ValidateMove(): Checks if a given move hits constraints, or other blocks.
    public bool ValidateMove(Transform block, Vector4 move) {
        for (int i = 0; i < block.childCount; i++) {
            Vector3 pos = block.GetChild(i).position;
            if (CheckConstraints(pos + (Vector3)move) || CheckConstraints(CalcRotation(pos, block, move.w)))
                return false;
            for (int y = 0; y < FHeight; y++) {
                for (int x = 0; x < FWidth; x++) {
                    if (Field[x, y] == null) continue;
                    Transform FBlock = Field[x, y].transform;
                    if (CompV3(FBlock.position, pos + (Vector3)move) || CompV3(FBlock.position, CalcRotation(pos, block, move.w)))
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

    public void HoldBlock(GameObject block) {
        display.HoldBlock(block);
    }
    
    // AddToField(): Adds 
    public void AddToField(Transform t) {
        for (int i = 0; i < t.childCount; i++) {
            Vector3 childPos = t.GetChild(i).transform.position;
            Field[Mathf.CeilToInt(childPos.x - LSide) - 1, Mathf.CeilToInt(childPos.y) - 1] = t.GetChild(i).gameObject;
        }
        UpdateLines();
    }

    public void SpawnNextBlock() {
        GameObject block = BlockQueue.Dequeue();
        SpawnBlock(block);
        BlockQueue.Enqueue(RandomBlock());
        display.UpdatePreview();
    }

    public void SpawnBlock(GameObject block) {
        Instantiate(block, block.transform.position + new Vector3(LSide, 0, 0), Quaternion.identity);
    }

    public GameObject RandomBlock() {
        return Blocks[Random.Range(0, 6)];
    }

    private bool RowFilled(int row) {
        for (int x = 0; x < FWidth; x++) {
            if (Field[x, row] == null) return false;
        }
        return true;
    }

    private void RemoveRow(int y) {
        for (int x = 0; x < FWidth; x++) { 
            Destroy(Field[x, y]);
            Field[x, y] = null;
        }
    }

    private bool CheckConstraints(Vector3 pos) {
        return (pos.x >= RSide || pos.x <= LSide || pos.y <= 0) ? true : false;
    }

    private Vector3 CalcRotation(Vector2 pos, Transform block, float degree) {
        pos = block.InverseTransformPoint(pos);
        float a = degree * Mathf.PI / 180;
        float xp = pos.x * Mathf.Cos(a) - pos.y * Mathf.Sin(a);
        float yp = pos.x * Mathf.Sin(a) + pos.y * Mathf.Cos(a);
        return block.TransformPoint(new Vector3(xp, yp, 0));
    }

    private bool CompV3(Vector3 a, Vector3 b) {
        return Vector3.SqrMagnitude(a - b) < 0.0001f;
    }
}
