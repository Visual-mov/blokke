using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDisplay : MonoBehaviour {

    FieldManager fm;
    Transform preview, hold;
    GameObject[] preQueue;
    GameObject hBlock;
    bool holding;

    private void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        preview = transform.Find("Preview");
        hold = transform.Find("Hold");
        holding = false;
        preQueue = new GameObject[fm.queueLength];
    }

    // UpdatePreview: Updates blocks being displayed in preview to show block queue.
    public void UpdatePreview() {
        GameObject[] blocks = fm.blockQueue.ToArray();
        float pHeight = preview.TransformVector(preview.GetComponent<BoxCollider2D>().size).y;
        for (int i = 0; i < blocks.Length; i++) {
            Destroy(preQueue[i]);
            preQueue[i] = Instantiate(blocks[i]);
            float divLength = pHeight / preQueue.Length;
            Vector2 pos = new Vector2(preview.position.x, preview.position.y + (pHeight / 2) - divLength / 2 - i * divLength);
            MoveBlock(preQueue[i], pos, preview);
        }
    }

    // HoldBlock: Puts given block in holding position and spawn previously held block if it exists. If not, spawns next block in queue.
    public void HoldBlock(GameObject block) {
        if (!holding) {
            fm.SpawnNextBlock();
            MoveBlock(block, hold.position, hold);
            holding = true;
        } else {
            fm.SpawnBlock(Array.Find(fm.blocks, b => b.name == hBlock.name.Replace("(Clone)", "")));
            MoveBlock(block, hold.position, hold);
            Destroy(hBlock);
        }
        hBlock = block;

    }

    // MoveBlock: Disables and displays block at given position.
    void MoveBlock(GameObject g, Vector3 pos, Transform t) {
        g.GetComponent<Block>().Disable();
        g.transform.rotation = Quaternion.identity;
        g.transform.localScale = new Vector2(0.55f,0.55f);
        g.transform.position = pos - ReturnCenter(g.transform);
        g.transform.parent = t;
    }

    // ReturnCenter: Calculates true center of block with respect to all children, instead of pivot.
    Vector3 ReturnCenter(Transform block) {
        Vector3 center = Vector3.zero;
        foreach (Transform child in block) {
            center += child.transform.localPosition;
        }
        return Vector3.Scale(center / block.childCount, block.localScale);
    }

    public void RemoveHeld() {
        Destroy(hBlock);
        holding = false;
    }
}
