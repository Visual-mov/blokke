using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    void Start()
    {
        InvokeRepeating("MoveDown",1,1);
    }

    void Update()
    {
        CheckInput();
    }
    void CheckInput() {
        Vector3 move = new Vector3();
        if(Input.GetKeyDown(KeyCode.A)) { move.x = -1;}
        else if (Input.GetKeyDown(KeyCode.D)) { move.x = 1; }

        if(Input.GetKeyDown(KeyCode.W)) {
            // snap to bottom
        } else if (Input.GetKeyDown(KeyCode.S)) { move.y = -1; }
        transform.Translate(move,Space.World);

        if(Input.GetKeyDown(KeyCode.Space)) {
            //transform.Rotate(transform.TransformPoint(rPoint),90,Space.Self);
            transform.Rotate(0, 0, 90);
        }
    }
    void MoveDown() {
       transform.Translate(0, -1, 0, Space.World);
    }
}
