﻿using UnityEngine;
using System.Collections;
using Makezi.StateMachine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 10f;							// Constant movement speed
	public float size = 0.75f;								// Transform scale 
	public GameObject childObj;								// Reference to child game object containing Shape component

	public delegate void DeathHandler();
	public static event DeathHandler onPlayerDeath;			// Event triggered on player death

	private StateMachine<PlayerController> stateMachine;	// Reference to player's state machine
	private Shape shape;									// Reference to attached shape component						
	private Shape[] childShapes;							// Array containing reference to all child game objects with Shape component
	private Vector3 startingPosition;						// Reference to initial starting position
	private bool isDead;									// Flag for death status

	void Awake(){
		// Set child objects to active
		childShapes = GetComponentsInChildren<Shape>();
		foreach(Shape s in childShapes){
			s.gameObject.SetActive(false);
		}
		childObj.SetActive(true);
		startingPosition = transform.position;
	}

	void Start(){
		InitStates();
	}

	/* Initialises state machine and adds available player states */
	private void InitStates(){
		stateMachine = new StateMachine<PlayerController>(this, new PlayerIdle());
		stateMachine.AddState(new PlayerMoving());
	}

	void Update(){
		stateMachine.Update();
	}

	void FixedUpdate(){
		stateMachine.FixedUpdate();
	}

	/* Change player shape sprite */
	public void ChangeShape(){
		if(childObj != null){
			childObj.SetActive(false);
			shape = null;
		}
		// Randomly set child object with Shape component as active
		childObj = childShapes[Random.Range(0, childShapes.Length)].gameObject;
		childObj.SetActive(true);
		shape = childObj.GetComponent<Shape>();
	}

	/* Initialises player, setting defaults */
	public void Init(){
		gameObject.SetActive(true);
		isDead = false;
		SetCurrentSize();
		transform.position = startingPosition;
		ChangeShape();
		stateMachine.SetState<PlayerIdle>();
	}

	/* Signifies player death */
	public void Destroy(){
		isDead = true;
		gameObject.SetActive(false);
		if(onPlayerDeath != null){ onPlayerDeath(); }
	}

	/* Sets current scale of GameObject */
	private void SetCurrentSize(){
		transform.localScale = new Vector3(size, size, 1);
	}

	public bool Dead {
		get { return isDead; }
	}

	public Shape Shape {
		get { return shape; }
	}

}
