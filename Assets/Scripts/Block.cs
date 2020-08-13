﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public bool active;
    public float fallTime;
    private FieldManager fm;
    private Vector2 lastInput;

    private void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        active = true;
        lastInput = Vector2.zero;
    }

    private void Start() {
        if (fm.ValidateMove(transform, Vector3.zero))
            StartCoroutine("MoveDown");
    }

    private void Update() {
        if (active)
            CheckInput();
        if (transform.childCount == 0)
            Destroy(transform.gameObject);
    }

    // CheckInput: Listens for user input, and executes according action(s).
    private void CheckInput() {
        Vector4 move = new Vector4();
        Vector2 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        move.x += Input.GetKeyDown(KeyCode.A) ? -1 : 0;
        move.x += Input.GetKeyDown(KeyCode.D) ? 1 : 0;
        move.w += Input.GetButtonDown("Rotate") ? 90 : 0;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.W) || Input.GetAxis("Vertical") == -1) move.y = -1.0f;
        if (Input.GetButtonDown("Hold")) fm.display.HoldBlock(transform.gameObject);

        if (input.x != lastInput.x || input.y != lastInput.y) {
            if (input.x == -1) move.x = -1.0f;
            else if (input.x == 1) move.x = 1.0f;
            if (input.y == 1) move.y = -1.0f;
        }

        if (fm.ValidateMove(transform, move) && move != Vector4.zero) {
            transform.Translate(move, Space.World);
            transform.Rotate(new Vector3(0, 0, move.w));
        }
        lastInput = input;
    }

    // MoveDown: Checks if block is at bottom of the field and disables if true, otherwise moves the block down.
    private IEnumerator MoveDown() {
        Vector2 move = Vector2.down;
        while (active) {
            yield return new WaitForSeconds(fallTime);
            if (fm.ValidateMove(transform, move))
                transform.Translate(move, Space.World);
            else {
                Disable();
                fm.AddToField(transform);
                fm.SpawnNextBlock();
            }
        }
    }

    public void Disable() {
        active = false;
        StopCoroutine("MoveDown");
    }
}
