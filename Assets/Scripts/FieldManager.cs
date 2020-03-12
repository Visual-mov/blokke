using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour {

    const int fWidth = 10;
    const int fHeight = 20;
    private GameObject[] Blocks;
    private GameObject[,] Field;
    private float LSide, RSide;

    void Awake() {
        Field = new GameObject[fWidth, fHeight];
        LSide = transform.position.x - fWidth / 2;
        RSide = transform.position.x + fWidth / 2;
        string[] names = {
            "I-Block", "J-Block",
            "L-Block", "O-Block",
            "S-Block", "T-Block",
            "Z-Block"
        };
        Blocks = new GameObject[names.Length];
        for (int i = 0; i < names.Length; i++) {
            Blocks[i] = (GameObject) Resources.Load("Prefabs/" + names[i]);
        }
    }

    void Start() {
        SpawnPiece();
    }

    public bool ValidateMove(Transform block, Vector4 move) {
        for (int i = 0; i < block.childCount; i++) {
            Vector3 pos = block.GetChild(i).position;
            Vector3 rotatedPos = CalcRotation(pos, block, move.w);
            if (CheckConstraints(pos + (Vector3) move))
                return false;
            else if (CheckConstraints(rotatedPos))
                return false;
            for(int y = 0; y < fHeight; y++) {
                for(int x = 0; x < fWidth; x++) {
                    Vector3 FPos = Field[x, y].transform.position;
                    if (Field[x, y] != null && (FPos == pos + (Vector3) move || FPos == rotatedPos))
                        return false;
                }
            }
        }
        return true;
    }

    public void UpdateLines() {
        for(int y = 0; y < fHeight; y++) {
            while(RowFilled(y)) {
                RemoveRow(y);
                for(int i = y; i < fHeight; i++) {
                    print(i);
                    for (int j = 0; j < fWidth; j++) {
                        if (i + 1 >= fHeight) {
                            Destroy(Field[j, i]);
                            Field[j, i] = null;
                        } else {
                            if(Field[j, i] != null) Field[j, i].transform.Translate(Vector3.down,Space.World);
                            Field[j, i] = Field[j, i + 1];
                        }
                    }
                }
            }
        }
    }

    public void AddToField(Transform t) {
        for (int i = 0; i < t.childCount; i++) {
            Vector3 childPos = t.GetChild(i).transform.position;
            Field[Mathf.CeilToInt(childPos.x - LSide) - 1, Mathf.CeilToInt(childPos.y) - 1] = t.GetChild(i).gameObject;
        }
        UpdateLines();
    }
    
    public void SpawnPiece() {
        GameObject block = Blocks[Random.Range(0, 6)];
        Instantiate(block, block.transform.position + new Vector3(LSide, 0, 0), new Quaternion());
    }

    private bool RowFilled(int row) {
        for (int x = 0; x < fWidth; x++) {
            if (Field[x, row] == null) return false;
        }
        return true;
    }

    private void RemoveRow(int y) {
        for (int x = 0; x < fWidth; x++) {
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
        float xp = -pos.y * Mathf.Sin(a) + pos.x * Mathf.Cos(a);
        float xy = pos.y * Mathf.Cos(a) - pos.x * Mathf.Sin(a);
        return block.TransformPoint(new Vector3(xp,xy,0));
    }
}
