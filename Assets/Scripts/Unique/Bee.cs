using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour {

    //If the bee damages the player then this bee dies as well
    void OnTriggerStay2D(Collider2D stuff) {

        Player e = stuff.GetComponent<Player>();
        if (e != null && GetComponent<Enemy>().health > 0) {

        	//Will only happen if the player is not immune
        	if (!e.GetImmunity()) {
        		GetComponent<Enemy>().ReduceHealth(8);
        		e.ReduceHealth(GetComponent<Enemy>().damage);
        	}
        }
    }
}
