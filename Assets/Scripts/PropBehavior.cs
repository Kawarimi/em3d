using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBehavior : MonoBehaviour {
	GameObject appliedplayer;
	bool killaura = false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame && !gameObject.tag.Equals("Unshiftable")
	void Update ()	{
	   if(killaura)
	   {
		appliedplayer.SendMessage("ManaDrain", 0.5f);
	   }
	}
	void KillAura(string name)
	{		
        killaura = !killaura;
	appliedplayer = GameObject.Find(name); 
	}
	void OnTriggerStay(Collider other)
	{
	if(killaura && other.gameObject.tag.Equals("Player")) 
	{		
	other.gameObject.SendMessage("GotHit", 2.0f);			
	}
	}
    void ItemTake(GameObject player)
    {
        transform.LookAt(player.transform);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 50);
        Destroy(gameObject, 0.5f);
    }
}
 