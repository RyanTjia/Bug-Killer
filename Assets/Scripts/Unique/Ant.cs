using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour {

	private bool Release;
	public GameObject Gas;

	// Called on start
	void Start() {
		Release = false;
	}

    // Update is called once per frame
    void Update() {
        
        //When the ant dies, it will release some gas
        if (GetComponent<Enemy>().health <= 0 && !Release) {
        	Release = true;
        	Instantiate(Gas, transform.position, Quaternion.identity);
        }
    }
}
