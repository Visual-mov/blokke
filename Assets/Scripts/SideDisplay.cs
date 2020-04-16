using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDisplay : MonoBehaviour {
    private FieldManager fm;
    private Transform preview, hold;
    private GameObject heldBlock;
    private GameObject[] preQueue;
    private bool holding;

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
            Vector2 pos = new Vector2(preview.position.x, preview.position.y + (pHeight / 2) - 1.5f - i * (pHeight / preQueue.Length));
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
        g.transform.rotation = Quaternion.identity;
        g.transform.localScale = new Vector2(0.55f,0.55f);
        //print(pos - ReturnCenter(g.transform).normalized);
        g.transform.position = pos;
        g.transform.parent = t;
        g.GetComponent<Block>().Disable();
    }

    private Vector3 ReturnCenter(Transform block) {
        Vector3 center = Vector3.zero;
        for(int i = 0; i < block.childCount; i++) {
            center += block.GetChild(i).transform.position;
        }
        return center / block.childCount;
    }
}
