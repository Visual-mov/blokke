﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    private FieldManager fm;
    public bool active;
    private Vector2 lastInput;

    void Awake() {
        fm = GameObject.Find("Field").GetComponent<FieldManager>();
        active = true;
        InvokeRepeating("Tick", 0.8f, 0.8f);
        lastInput = Vector2.zero;
    }

    void Start() {
        if(!fm.ValidateMove(transform,Vector3.zero) && active) {
            Destroy(transform.gameObject);
        }
    }

    void Update() {
        if(active)
            CheckInput();
        if (transform.childCount == 0)
            Destroy(transform.gameObject);
    }

    // CheckInput(): Listens for user input, and executes according action.
    private void CheckInput() {
        Vector4 move = new Vector4();
        Vector2 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetKeyDown(KeyCode.A)) move.x = -1.0f;
        if (Input.GetKeyDown(KeyCode.D)) move.x = 1.0f;
        if (Input.GetKeyDown(KeyCode.S)) move.y = -1.0f;
        if (Input.GetButtonDown("Rotate")) move.w = 90.0f;
        if (Input.GetButtonDown("Hold")) fm.display.HoldBlock(transform.gameObject);
        if (input.x != lastInput.x || input.y != lastInput.y) {
            if (input.x == -1) move.x = -1.0f;
            else if (input.x == 1) move.x = 1.0f;
            if (input.y == 1) move.y = -1.0f;
        }

        if (fm.ValidateMove(transform, move)) {
            transform.Translate(move.x, move.y, move.z, Space.World);
            transform.Rotate(new Vector3(0, 0, move.w));

        }
        lastInput = input;
    }

    // Tick(): Checks if block is at the bottom of the field and disables it if so, otherwise moves the block down.
    private void Tick() {
        Vector3 move = Vector3.down;
        if (fm.ValidateMove(transform, move)) {
            transform.Translate(move, Space.World);
        } else {
            Disable();
            fm.AddToField(transform);
            fm.SpawnNextBlock();
        }
    }

    // Disable(): Disables checking of user input and tick execution.
    public void Disable() {
        active = false;
        CancelInvoke("Tick");
    }
}
