using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSystem : MonoBehaviour {

	float timer = 0.0f;
	int waitingTime = 10;
	bool respawn;
	GameObject appliedplayer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	if(respawn)
	{
    timer += Time.deltaTime;
    if(timer > waitingTime){  
        respawn = false;
		timer = 0;
		appliedplayer.SetActive(true);
		appliedplayer.transform.position = new Vector3(0,3,0);
    }
	}	
	}
	void Kill(string playername)
	{
        appliedplayer = GameObject.Find(playername);
		appliedplayer.SetActive(false);
		respawn = true;	
	}
}
