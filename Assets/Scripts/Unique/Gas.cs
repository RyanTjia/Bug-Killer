using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : MonoBehaviour {

	//Called on start
	void Start() {
		StartCoroutine(Fading());
	}

	//The gas will only appear for a while
	IEnumerator Fading() {
		yield return new WaitForSeconds(15.0f);
		Destroy(gameObject);
	}

    //Enemies are inside
    void OnTriggerEnter2D(Collider2D stuff) {

        Enemy e = stuff.GetComponent<Enemy>();
        if (e != null) {
        	e.BuffStat(1.5, 1.5f);
        }
    }

    //Enemies left the gas
    void OnTriggerExit2D(Collider2D stuff) {
    	Enemy e = stuff.GetComponent<Enemy>();
    	if (e != null) {
    		e.BuffStat(0.0, 0.0f);
    	}
    }
}
