using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Worker : MonoBehaviour {



	public TreeScript treeScript;

	SkeletonAnimation skeletonAnimation;

	public Spine.AnimationState spineAnimationState;
	public Spine.Skeleton skeleton;


	private StateMachine<States> fsm;

	private Vector3 mTargetPos; 

	public float walkSpeed = 1f;


	private float checkDis = 0;
	private int walkDirect = 1;


	public float cutTreeDistance = 0.6f;



	public enum States
	{
		WalkTo,
		Cut,
		Idle,
	}

	private States nextState;
		
	// Use this for initialization
	void Start () {

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.AnimationState;
		skeleton = skeletonAnimation.Skeleton;

		skeletonAnimation.AnimationState.Event += HandleEvent;


		//Initialize State Machine Engine		
		fsm = StateMachine<States>.Initialize(this, States.Idle);
		mTargetPos = Vector3.zero;
		nextState = States.Idle;
	}
	
	// Update is called once per frame
	void Update () {


		
	}


	IEnumerator DoDemoRoutine () {

		yield return new WaitForSeconds(1.5f);
	
	}


	void HandleEvent (Spine.TrackEntry trackEntry, Spine.Event e) {
		if (e.Data.Name == "cut_down") {			
			bool inverse = false;
			if (transform.position.x < treeScript.transform.position.x) {
				inverse = true;
			} else {
				inverse = false;
			}
			treeScript.beCut (inverse);
		}
	}


	void doWalkToPos(Vector3 pos){
	
		mTargetPos = pos;
		if (transform.position.x < mTargetPos.x) {
		
			skeletonAnimation.Skeleton.FlipX = true;
		} else {
			skeletonAnimation.Skeleton.flipX = false;
		}
		fsm.ChangeState (States.WalkTo);
	}


	void doWalkForCutToPos(Vector3 pos){
		
		int dir = 1;
		if (transform.position.x < mTargetPos.x) {

			skeletonAnimation.Skeleton.FlipX = true;
			dir = -1;
		} else {
			skeletonAnimation.Skeleton.flipX = false;
			dir = 1;
		}
		skeletonAnimation.timeScale = 1.5f;
		Vector3 tmpPos = pos;
		pos.x += dir * cutTreeDistance;
		mTargetPos = pos;
		fsm.ChangeState (States.WalkTo);
		nextState = States.Cut;
	}


	void doIdle(){

		fsm.ChangeState (States.Idle);
	}


	void doCut(){

		fsm.ChangeState (States.Cut);
	}



	//stateMachine

	void WalkTo_Enter(){
		
		spineAnimationState.SetAnimation(0, "walk", true);
	}

	void WalkTo_Update(){

		Debug.Log ("walk_to_update");
		checkDis = (mTargetPos - transform.position).sqrMagnitude;
		if (transform.position.x < mTargetPos.x) {
			
			walkDirect = 1;
		} else {
			walkDirect = -1;
		}
		Vector3 tmpv = transform.position;

		if (checkDis > walkSpeed * walkSpeed* Time.deltaTime* Time.deltaTime) {

			tmpv.x = tmpv.x + walkDirect * walkSpeed * Time.deltaTime;
			transform.position = tmpv;
		} else {
		
			tmpv.x = mTargetPos.x;
			transform.position = tmpv;
			if (nextState == States.Idle) {
				doIdle ();
			} else if (nextState == States.Cut) {
				doCut ();
			}
		}
	}

	void WalkTo_Exit(){


		skeletonAnimation.timeScale = 1f;

	}


	void Idle_Enter(){

		Debug.Log ("idle_enter");
		spineAnimationState.SetAnimation(0, "idle", true);

	}

	void Cut_Enter(){
		
	
		spineAnimationState.SetAnimation(0, "cut_wood", true);
	}
	void Cut_Update(){

		if (treeScript && treeScript.idDie()) {
			doIdle ();
		}
	}


	void OnGUI()
	{
		//Example of polling state 
		var state = fsm.State;

		GUILayout.BeginArea(new Rect(50,50,120,40));

		if(state == States.Idle && GUILayout.Button("WalkTo"))
		{
			doWalkForCutToPos (treeScript.transform.position);
		}

		GUILayout.EndArea();
	}



}
