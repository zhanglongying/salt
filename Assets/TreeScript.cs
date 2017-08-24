using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class TreeScript : MonoBehaviour {



	SkeletonAnimation skeletonAnimation;

	public Spine.AnimationState spineAnimationState;
	public Spine.Skeleton skeleton;

	public int hp = 10;

	private string curState = "normal";

	private bool isFirst = false;

	public GameObject stakeTemplate;



	// Use this for initialization
	void Start () {

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.AnimationState;
		skeleton = skeletonAnimation.Skeleton;

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void beCut(bool inverse = false){

		string cutAnimationName = "cut";
		string dieAnimationName = "die";
		if (inverse) {
			cutAnimationName = "cut_i";
			dieAnimationName = "die_i";
		}


		if (curState == "normal") {
			if (hp > 0) {
				if(isFirst)spineAnimationState.SetAnimation (0, cutAnimationName, false);
				if (!isFirst) {
					isFirst = true;
					//StartCoroutine (coolDown ());
				}
			} else {
				spineAnimationState.SetAnimation (0, dieAnimationName, false);
				curState = "die";
		
				StartCoroutine (showStake());
				StartCoroutine (destoryCoroutine ());
			}
			hp--;
		}

	}


	public bool idDie(){

		return curState == "die";


	}

	IEnumerator destoryCoroutine () {

		yield return new WaitForSeconds(3.0f);
		Destroy (gameObject);
	}
	IEnumerator showStake(){
		yield return new WaitForSeconds(0.1f);

		GameObject stake = Instantiate (stakeTemplate);
		stake.transform.position = transform.position;

	}
}
