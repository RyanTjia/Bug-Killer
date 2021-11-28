using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Queen : MonoBehaviour {

    private Enemy Self;
	private bool CanLay;
	private bool ready;

	//Own health bar
	public GameObject HealthBar;
	private int Health;
    private RectTransform CurrentHealth;
    private float MaxHealthSize;

    //Enemies to spawn
    public GameObject Ant;
    public GameObject Bee;

    // Start is called before the first frame update
    void Start() {

    	//This is to make the boss unique from the other enemies
        Self = GetComponent<Enemy>();
        CanLay = true;
        ready = false;

        //Instantiate its own health bar
        Instantiate(HealthBar, new Vector2(0.0f, 0.0f), Quaternion.identity, GameObject.Find("/EnemyUI").transform);
        HealthBar = GameObject.Find(HealthBar.name + "(Clone)");
        HealthBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);

        //Instantiating its health
        Health = Self.health;
        CurrentHealth = HealthBar.transform.GetChild(0).GetComponent<RectTransform>();
        MaxHealthSize = CurrentHealth.rect.width;

        //Instantiating the text
        HealthBar.transform.GetChild(1).GetComponent<Text>().text = "The Queen";
        HealthBar.transform.GetChild(2).GetComponent<Text>().text = Self.health + "/" + Health;

        StartCoroutine(RNG());
    }

    // Update is called once per frame
    void Update() {
        
        //Continously update the health bar
        if (HealthBar != null) {
        	CurrentHealth.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MaxHealthSize * Self.health / Health);
        	HealthBar.transform.GetChild(2).GetComponent<Text>().text = Self.health + "/" + Health;
    	}

        //Stops any active coroutine when health is 0
        if (Self.health <= 0 && !ready) {
			Self.move = false;
        	StopCoroutine(RNG());
        	StopCoroutine(Lay(1));
        	if (!ready) {
        		ready = true;
        		Destroy(HealthBar);

                foreach (Transform child in GameObject.Find("Enemies").transform) {
                    Destroy(child.gameObject);
                }

        		GameObject.Find("Main Camera").GetComponent<Game>().NextWave(3);
        	}
        }
    }

    //Slower RNG to spawn ants and bees
    IEnumerator RNG() {

    	//Will start once the player hurt the queen
    	if (CanLay && Self.health > 0 && Self.health < Health && GameObject.Find("Enemies").transform.childCount < 21) {
        	CanLay = false;
        	StartCoroutine(Lay((int)(Health/Self.health)));
        }

        yield return new WaitForSeconds(3.0f);
        StartCoroutine(RNG());
    }

    //Spawning ability
    IEnumerator Lay(int limit) {

    	//Stops to lay first
    	GetComponent<Animator>().SetBool("Lay", true);
    	Self.move = false;

    	//Starts spawning enemies
    	//Amount of enemies spawned depends on the queen's health
    	for (int x = 0; x < limit + 1; x++) {

    		//This is so that the enemies spawn in front of the queen as a shield
    		Vector2 Target = GameObject.Find("Player(Clone)").transform.position;
    		float AngleRad = Mathf.Atan2(Target.y - transform.position.y, Target.x - transform.position.x);
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
    		Vector2 SpawnPosition = new Vector2(transform.position.x + ((180 / Mathf.PI) * Mathf.Cos(AngleRad)) * 0.01f, transform.position.y + ((180 / Mathf.PI) * Mathf.Sin(AngleRad)) * 0.01f);

    		//Ants
    		Instantiate(Ant, SpawnPosition, Quaternion.identity, GameObject.Find("Enemies").transform);

    		//Bees
    		Instantiate(Bee, SpawnPosition, Quaternion.identity, GameObject.Find("Enemies").transform);

    		//Slight cooldown
    		yield return new WaitForSeconds(0.5f);
    	}

	    //Resets
	    GetComponent<Animator>().SetBool("Lay", false);
	    Self.move = true;
	    CanLay = true;
    }
}
