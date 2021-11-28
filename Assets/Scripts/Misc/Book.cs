using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {

	//List of bugs
	public GameObject Roach;
	public GameObject Fly;
	public GameObject Ant;
	public GameObject Bee;
	public GameObject Beetle;
	public GameObject Queen;
	public GameObject Wasp;

	//An actual list
	private List<GameObject> List = new List<GameObject>();
	private int element;

    // Start is called before the first frame update
    void Start() {

        //This will be only for the bug book
        if (name == "Book(Clone)") {
            Instantiate(Roach, transform.position, Quaternion.identity, transform);

            //Puts the list in an actual List
            element = 0;
            List.Add(Roach);
            List.Add(Fly);
            List.Add(Ant);
            List.Add(Bee);
            List.Add(Beetle);
            List.Add(Queen);
            List.Add(Wasp);
        }
    }

    //Moving to the next page or the previous page
    public void NextPage() {

    	if (element < List.Count - 1) {
    		Destroy(GameObject.Find(List[element].name + "(Clone)"));
    		element++;
    		Instantiate(List[element], transform.position, Quaternion.identity, transform);
    	}
    }

    public void PreviousPage() {

    	if (element > 0) {
    		Destroy(GameObject.Find(List[element].name + "(Clone)"));
    		element--;
    		Instantiate(List[element], transform.position, Quaternion.identity, transform);
    	}
    }

    //Exit the book
    public void MainMenu() {
    	Destroy(gameObject);
    }
}
