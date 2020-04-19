﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDisplay : MonoBehaviour {

    FieldManager fm;
    Transform preview, hold;
    GameObject heldBlock;
    GameObject[] preQueue;
    bool holding;

    private void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        preview = transform.Find("Preview");
        hold = transform.Find("Hold");
        holding = false;
        preQueue = new GameObject[fm.QueueLength];
    }

    public void UpdatePreview() {
        GameObject[] blocks = fm.BlockQueue.ToArray();
        float pHeight = preview.TransformVector(preview.GetComponent<BoxCollider2D>().size).y;
        for (int i = 0; i < blocks.Length; i++) {
            Destroy(preQueue[i]);
            preQueue[i] = Instantiate(blocks[i]);
            float divLength = pHeight / preQueue.Length;
            Vector2 pos = new Vector2(preview.position.x, (preview.position.y + (pHeight / 2) - divLength / 2) - i * divLength);
            MoveBlock(preQueue[i], pos, preview);
        }
    }

    public void HoldBlock(GameObject block) {
        if (!holding) {
            fm.SpawnNextBlock();
            MoveBlock(block, hold.position, hold);
            holding = true;
        } else {
            fm.SpawnBlock(Resources.Load("Prefabs/" + heldBlock.name.Replace("(Clone)", "")) as GameObject);
            MoveBlock(block, hold.position, hold);
            Destroy(heldBlock);
        }
        heldBlock = block;

    }

    private void MoveBlock(GameObject g, Vector3 pos, Transform t) {
        g.GetComponent<Block>().Disable();
        g.transform.rotation = Quaternion.identity;
        g.transform.localScale = new Vector2(0.55f,0.55f);
        g.transform.position = pos - ReturnCenter(g.transform);
        g.transform.parent = t;
    }

    private Vector3 ReturnCenter(Transform block) {
        Vector3 center = Vector3.zero;
        for(int i = 0; i < block.childCount; i++) {
            center += block.GetChild(i).transform.localPosition;
        }
        return Vector3.Scale(center / block.childCount, block.localScale);
    }
}
