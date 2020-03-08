using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{

    static int fWidth = 10;
    private GameObject[] Blocks;
    private List<GameObject> Field;

    private void Awake() {
        Field = new List<GameObject>();
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

        //Debug.Log(transform.position);
        for (int i = 0; i < block.childCount; i++) {
            Vector3 pos = block.GetChild(i).position;
            if (CheckConstraints(pos + (Vector3) move))
                return false;
            else if (CheckConstraints(block.TransformPoint(CalcRotation(block.InverseTransformPoint(pos), move.w))))
                return false;
            foreach(GameObject t in Field) {
                if (t.transform.position == pos + (Vector3) move)
                    return false;
            }
        }
        return true;
    }

    public void UpdateLines() {
        for(int i = 0; i < Field.Count; i++) {
            if (Mathf.Approximately(Field[i].transform.position.y, 0.5f)) {
                //RemoveBlock(i);
                //i -= 1;
            }
        }
    }

    private void RemoveBlock(int index) {
        Destroy(Field[index]);
        Field.RemoveAt(index);
    }

    private bool CheckConstraints(Vector3 pos) {
        return (pos.x >= fWidth || pos.x < 0 || pos.y <= 0) ? true : false;
    }

    private Vector3 CalcRotation(Vector2 pos, float degree) {
        float a = degree * Mathf.PI / 180;
        float xp = -pos.y * Mathf.Sin(a) + pos.x * Mathf.Cos(a);
        float xy = pos.y * Mathf.Cos(a) - pos.x * Mathf.Sin(a);
        return new Vector3(xp,xy,0);
    }

    public void SpawnPiece() {
        Object.Instantiate(Blocks[Random.Range(0, 6)]);
    }
    public void AddToField(Transform t) {
        for(int i = 0; i < t.childCount; i++) {
            Field.Add(t.GetChild(i).gameObject);
        }
        UpdateLines();
    }
}
