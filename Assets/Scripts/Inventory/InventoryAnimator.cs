﻿using UnityEngine;
using System.Collections;

public class InventoryAnimator : MonoBehaviour {
	
	public ArrowAnimation tutorialArrowAnimation;

	public bool isPlaying;
	private float time, dum;
	private float animationTime = 50f;
	private float miTime;
	private UISprite sprite;
	private Vector3 originalScale;

	// Use this for initialization
	void Start () {
		isPlaying = false;
		time = 0f;
		miTime = animationTime/2;
		
		sprite = GetComponentInChildren<UISprite>();
		originalScale = sprite.transform.localScale;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying){
			time += 1;
			dum = 2*time/animationTime;
			if(time < miTime){
				sprite.transform.localScale = originalScale*(1+dum/4);
			}
			if(time > miTime){
				sprite.transform.localScale = originalScale*(1.5f-dum/4);
			}
		}
		if (time >= animationTime ){
			time = 0;
		}
	}
	
	
		public void Play() {
		tutorialArrowAnimation.Play(GUITransitioner.GameScreen.screen1);
		isPlaying = true;
		time =0f;
	}
	
		public void reset() {
		ArrowAnimation.Delete ("WorldEquipButtonPanel");
		sprite.transform.localScale = originalScale;
		isPlaying = false;
		
	}
}




