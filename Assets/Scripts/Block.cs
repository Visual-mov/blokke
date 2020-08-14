﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public float fastSpeed = 0.02f;
    public bool active;
    private FieldManager fm;
    private Vector2 lastPad;
    private Coroutine moveDown, holdDown;

    private void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        active = true;
        lastPad = Vector2.zero;
        holdDown = null;
        moveDown = null;
    }

    private void Start() {
        if (fm.ValidateMove(transform, Vector3.zero))
            moveDown = StartCoroutine(MoveDown(fm.fallSpeed));
    }

    private void Update() {
        if (active)
            CheckInput();

        // Destroy block parent gameobject if all children have been cleared.
        if (transform.childCount == 0)
            Destroy(transform.gameObject);
    }

    /* CheckInput: Checks for keyboard or controller input and applies move. */
    private void CheckInput() {
        Vector4 move = new Vector4();

        // Controller input
        if(Input.GetJoystickNames().Length != 0) {
            Vector2 padInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (padInput.x != lastPad.x)
                move.x = padInput.x;

            if (padInput.y != lastPad.y) {
                if (padInput.y == 1)
                    move.y = -1.0f;
                else if (padInput.y == -1)
                    holdDown = StartCoroutine(MoveDown(fastSpeed));
                else if (padInput.y == 0 && holdDown != null)
                    StopCoroutine(holdDown);
            }
            lastPad = padInput;
        }

        // Keyboard input
        move.x += Input.GetKeyDown(KeyCode.A) ? -1 : 0;
        move.x += Input.GetKeyDown(KeyCode.D) ? 1 : 0;
        move.w += Input.GetButtonDown("Rotate") ? 90 : 0;

        if (Input.GetKeyDown(KeyCode.S))
            move.y = -1.0f;
        if (Input.GetButtonDown("Hold"))
            fm.display.HoldBlock(transform.gameObject);

        if (Input.GetKeyDown(KeyCode.W))
            holdDown = StartCoroutine(MoveDown(fastSpeed));
        else if (Input.GetKeyUp(KeyCode.W) && holdDown != null)
            StopCoroutine(holdDown);

        // Apply move
        if (fm.ValidateMove(transform, move) && move != Vector4.zero) {
            transform.Translate(move, Space.World);
            transform.Rotate(new Vector3(0, 0, move.w));
        }
    }

    /* MoveDown: Checks if block is at bottom of the field and disables if true, otherwise moves the block down. */
    private IEnumerator MoveDown(float speed) {
        while (active) {
            yield return new WaitForSeconds(speed);
            if (fm.ValidateMove(transform, Vector2.down)) {
                transform.Translate(Vector2.down, Space.World);
            } else if (active) {
                Disable();
                fm.AddToField(transform);
                fm.SpawnNextBlock();
            }
        }
    }

    public void Disable() {
        active = false;
        if(moveDown != null)
            StopCoroutine(moveDown);
    }
}
