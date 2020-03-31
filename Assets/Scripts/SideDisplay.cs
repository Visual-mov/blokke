using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDisplay : MonoBehaviour
{
    private FieldManager fm;
    public Vector3 PreviewPos, HoldPos;
    private GameObject heldBlock;
    private bool holding;

    private void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        PreviewPos = transform.Find("Preview").position;
        HoldPos = transform.Find("Hold").position;
        holding = false;
    }

    void Update() {
    }

    public void UpdatePreview() {
        GameObject[] blocks = fm.BlockQueue.ToArray();
        for (int i = 0; i < blocks.Length; i++) {
            //preview[i] = Instantiate((GameObject)Resources.Load("Prefabs/" + blocks[i].name.Replace("(Clone)", "")), new Vector3(HoldPos.x, 17.3f - (i * 2.3f), 0), new Quaternion());
        }
    }

    public void HoldBlock(GameObject block) {
        if (!holding) {
            MoveBlock(block);
            heldBlock = block;
            fm.SpawnNextBlock();
            holding = true;
        } else {
            // sus
            fm.SpawnBlock((GameObject)Resources.Load("Prefabs/" + heldBlock.name.Replace("(Clone)", "")));
            Destroy(heldBlock);
            MoveBlock(block);
            heldBlock = block;
        }
        
    }

    void MoveBlock(GameObject g) {
        g.transform.position = HoldPos;
        g.transform.rotation = new Quaternion();
        g.transform.localScale = new Vector3(0.8f,0.8f,0);
        g.GetComponent<Block>().Disable();
    }
}
