using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.ShaderKeywordFilter;
using UnityEditorInternal;
using UnityEngine;
using State = ShadowManState;

public enum ShadowManState {
  IDLE,
  RUN_AWAY,
  PATROL_RIGHT,
  PATROL_LEFT,
  CHASE,
}


public class ShadowMan : MonoBehaviour {
  #region Fields and Properties
  // set in inspector
  public float speed = 5;
  public Transform tfPlayer;
  public float distFromPlayer = 3;
  public float speedInc = 50;

  // private fields
  private Dictionary<State, Action> stateEnterMeths;
  private Dictionary<State, Action> stateExitMeths;
  private Dictionary<State, Action> stateStayMeths;
  private CharacterController2D charCon;
  private Animator animator;
  private float xTarget;
  private bool jump;

  // properties
  public State State { get; private set; }
  #endregion

  #region Life Cycle meths
  private void Start() {
    stateStayMeths = new Dictionary<State, Action>() {
      {State.IDLE, StateStayIdle },
      {State.RUN_AWAY, StateStayRunAway },
      {State.PATROL_RIGHT, StateStayPatrolRight },
      {State.PATROL_LEFT, StateStayPatrolLeft },
      {State.CHASE, StateStayChase },
    };
    stateEnterMeths = new Dictionary<State, Action>() {
      {State.IDLE, StateEnterIdle },
      {State.RUN_AWAY, StateEnterRunAway },
      {State.PATROL_RIGHT, StateEnterPatrolRight },
      {State.PATROL_LEFT, StateEnterPatrolLeft },
      {State.CHASE, StateEnterChase },
    };
    stateExitMeths = new Dictionary<State, Action>() {
      {State.IDLE, StateExitIdle },
      {State.RUN_AWAY, StateExitRunAway },
      {State.PATROL_RIGHT, StateExitPatrolRight },
      {State.PATROL_LEFT, StateExitPatrolLeft },
      {State.CHASE, StateExitChase },
    };
    jump = false;
    State = State.IDLE;
    charCon = GetComponent<CharacterController2D>();
    animator = GetComponent<Animator>();
  }


  private void FixedUpdate() {
    stateStayMeths[State].Invoke();
  }
  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.CompareTag("Player")) {
      if (State == State.PATROL_LEFT || State == State.PATROL_RIGHT) {
        ChangeState(State.CHASE);

      }
    }
  }
  private void OnTriggerExit2D(Collider2D collision) {
    if (collision.CompareTag("Player")) {
      if (State == State.CHASE) {
        ChangeState(State.PATROL_RIGHT);
      }
    }
  }
  #endregion

  #region State methz
  public void ChangeState(State newState) {
    if (State != newState) {
      stateExitMeths[State].Invoke();
      State = newState;
      stateEnterMeths[State].Invoke();
    }
  }

  #region State Exit meths
  private void StateExitChase() {
    speed -= speedInc;
  }
  private void StateExitPatrolLeft() {
  }
  private void StateExitPatrolRight() {
  }
  private void StateExitRunAway() {
  }
  private void StateExitIdle() {
  }
  #endregion
  #region State Enter meths
  private void StateEnterChase() {
    speed += speedInc;
    StartCoroutine(WaitThenJump());
  }
  private void StateEnterPatrolLeft() {
    xTarget = transform.position.x - 5;
  }
  private void StateEnterPatrolRight() {
    xTarget = transform.position.x + 5;
  }
  private void StateEnterRunAway() {

  }
  private void StateEnterIdle() {

  }
  #endregion
  #region State Stay meths
  private void StateStayChase() {
    float xPlayerPos = tfPlayer.position.x;
    float xMyPos = transform.position.x;
    float dir = (xPlayerPos - xMyPos) < 0 ? -1 : 1;
    charCon.Move(dir * speed * Time.fixedDeltaTime, false, jump);
    jump = false;
    animator.SetFloat("Idle Run", 1);
  }
  private void StateStayPatrolLeft() {
    if (transform.position.x <= xTarget) {
      ChangeState(State.PATROL_RIGHT);
    }
    else {
      charCon.Move(-speed * Time.fixedDeltaTime, false, jump);
      jump = false;
      animator.SetFloat("Idle Run", 1);
    }
  }
  private void StateStayPatrolRight() {
    if (transform.position.x >= xTarget) {
      ChangeState(State.PATROL_LEFT);
    }
    else {
      charCon.Move(speed * Time.fixedDeltaTime, false, jump);
      jump = false;
      animator.SetFloat("Idle Run", 1);
    }
  }
  private void StateStayRunAway() {
    if (Mathf.Abs(transform.position.x - tfPlayer.position.x) > distFromPlayer) {
      ChangeState(State.PATROL_RIGHT);
    }
    else {
      charCon.Move(speed * Time.fixedDeltaTime, false, jump);
      jump = false;
      animator.SetFloat("Idle Run", 1);
    }
  }
  private void StateStayIdle() {
    if (charCon.IsPlayerOnGround()) {
      ChangeState(State.RUN_AWAY);
    }
  }
  #endregion
  #endregion

  #region Helper meths
  private IEnumerator WaitThenJump() {
    yield return new WaitForSecondsRealtime(2);
    jump = true;
  }
  #endregion
}
