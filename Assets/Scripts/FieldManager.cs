using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{

    static int fWidth = 10;
    public Object[] Blocks;
    public ArrayList Field;


    void Start() {
        Field = new ArrayList();
        SpawnPiece();
    }

    void Update()
    {
        
    }

    public bool ValidateMove(Transform block, Vector4 move) {
        //Debug.Log(transform.position);
        for (int i = 0; i < block.childCount; i++) {
            Vector3 pos = block.GetChild(i).position;
            if (CheckConstraints(pos + (Vector3) move)) return false;
            else if (CheckConstraints(block.TransformPoint(CalcRotation(block.InverseTransformPoint(pos), move.w)))) return false;
            foreach(Transform t in Field) {
                for(int j = 0; j < t.childCount; j++) {
                    if (t.GetChild(j).position == pos + (Vector3) move) return false;
                }
            }
        }
        return true;
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
        Field.Add(t);
    }
}
