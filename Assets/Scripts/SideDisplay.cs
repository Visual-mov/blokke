using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDisplay : MonoBehaviour
{
    FieldManager fm;
    Vector3 PreviewPos, HoldPos;

    private void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        PreviewPos = GameObject.Find("Preview").transform.position;
        HoldPos = GameObject.Find("Hold").transform.position;
    }

    void Start() {
        print(PreviewPos);
        print(HoldPos);
    }

    void Update() {
        
    }

    void ShowBlock(Block block, int position) {

    }
}
