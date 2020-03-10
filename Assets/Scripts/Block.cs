using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    private FieldManager fm;
    private bool Enable;

    private void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        Enable = true;
    }

    void Start() {
        if(!fm.ValidateMove(transform,Vector3.zero)) {
            Destroy(transform.gameObject);
            fm.GameOver = true;
        }
        InvokeRepeating("Tick",1,1);
    }

    void Update() {
        if(Enable)
            CheckInput();
        if (transform.childCount == 0)
            Destroy(transform.gameObject);
    }

    void CheckInput() {
        Vector4 move = new Vector4();
        // Change to Axes
        if (Input.GetKeyDown(KeyCode.A)) move.x = -1.0f;
        else if (Input.GetKeyDown(KeyCode.D)) move.x = 1.0f;
        if (Input.GetKeyDown(KeyCode.S)) move.y = -1.0f;
        if (Input.GetKeyDown(KeyCode.Space)) move.w = 90.0f;

        if (fm.ValidateMove(transform, move)) {
            transform.Translate(move.x, move.y, move.z, Space.World);
            transform.Rotate(new Vector3(0, 0, move.w));

        }
    }

    void Tick() {
        Vector3 move = Vector3.down;
        if (fm.ValidateMove(transform, move)) {
            transform.Translate(move, Space.World);
        } else {
            CancelInvoke("Tick");
            Enable = false;
            fm.AddToField(transform);
            fm.SpawnPiece();
        }
    }
}
