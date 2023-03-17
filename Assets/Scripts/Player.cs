using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
  private CharacterController2D charCon;
  private float move;
  // Start is called before the first frame update
  void Start() {
    charCon = GetComponent<CharacterController2D>();
  }

  // Update is called once per frame
  void Update() {
    move = 0;
    move += (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
    move += (Input.GetKey(KeyCode.RightArrow) ? +1 : 0);
  }

  private void FixedUpdate() {
    charCon.Move(move, false, false);
  }
}
