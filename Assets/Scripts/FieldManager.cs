using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{

    const int fWidth = 10;
    const int fHeight = 20;
    private GameObject[] Blocks;
    private GameObject[,] Field;

    private void Awake() {
        Field = new GameObject[fWidth, fHeight];
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
            if (CheckConstraints(pos + (Vector3) move))
                return false;
            else if (CheckConstraints(CalcRotation(pos, block, move.w)))
                return false;
            for(int y = 0; y < fHeight; y++) {
                for(int x = 0; x < fWidth; x++) {
                    if (Field[x, y] != null && Field[x,y].transform.position == pos + (Vector3) move)
                        return false;
                }
            }
        }
        return true;
    }

    public void UpdateLines() {
        // bad code no look!! >:(
        for(int y = 0; y < fHeight; y++) {
            if(RowFilled(y)) {
                RemoveRow(y);
                GameObject[] TempRow = new GameObject[fWidth];
                for(int i = y; i < fHeight; i++) {
                    for (int j = 0; j < fWidth; j++) {
                        if (i + 1 > fHeight - 1) {
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
        return (pos.x >= fWidth || pos.x < 0 || pos.y <= 0) ? true : false;
    }

    private Vector3 CalcRotation(Vector2 pos, Transform block, float degree) {
        pos = block.InverseTransformPoint(pos);
        float a = degree * Mathf.PI / 180;
        float xp = -pos.y * Mathf.Sin(a) + pos.x * Mathf.Cos(a);
        float xy = pos.y * Mathf.Cos(a) - pos.x * Mathf.Sin(a);
        return block.TransformPoint(new Vector3(xp,xy,0));
    }

    public void SpawnPiece() {
        Instantiate(Blocks[Random.Range(0, 6)]);
    }

    public void AddToField(Transform t) {
        for (int i = 0; i < t.childCount; i++) {
            Vector3 childPos = t.GetChild(i).transform.position;
            Field[Mathf.CeilToInt(childPos.x)-1, Mathf.CeilToInt(childPos.y)-1] = t.GetChild(i).gameObject;
        }
        
        UpdateLines();
    }
}
