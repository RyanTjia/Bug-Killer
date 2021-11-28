using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelWasp : MonoBehaviour {

    private Enemy Self;
	private bool CanScreech;
	private bool ready;

	//Own health bar
	public GameObject HealthBar;
	private int Health;
    private RectTransform CurrentHealth;
    private float MaxHealthSize;

    //Shoots stingers
    public GameObject Stinger;

    //For sound
    private AudioSource audiosource;
    public AudioClip Screech;

    // Start is called before the first frame update
    void Start() {

    	//This is to make the boss unique from the other enemies
        Self = GetComponent<Enemy>();
        CanScreech = true;
        ready = false;

        //This is for the final boss
        Self.FinalBoss = true;
        Self.move = false;

        //Instantiate its own health bar
        Instantiate(HealthBar, new Vector2(0.0f, 0.0f), Quaternion.identity, GameObject.Find("/EnemyUI").transform);
        HealthBar = GameObject.Find(HealthBar.name + "(Clone)");
        HealthBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);

        //Instantiating its health
        Health = Self.health;
        CurrentHealth = HealthBar.transform.GetChild(0).GetComponent<RectTransform>();
        MaxHealthSize = CurrentHealth.rect.width;

        //Instantiating the text
        HealthBar.transform.GetChild(1).GetComponent<Text>().text = "Jewel Wasp";
        HealthBar.transform.GetChild(2).GetComponent<Text>().text = Self.health + "/" + Health;

        audiosource = GetComponent<AudioSource>();

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
        	StopCoroutine(Summon());
        	if (!ready) {
        		ready = true;
        		Destroy(HealthBar);

        		GameObject.Find("Main Camera").GetComponent<Game>().EmptyReinforcement();
                foreach (Transform child in GameObject.Find("Enemies").transform) {
                    Destroy(child.gameObject);
                }

        		GameObject.Find("Main Camera").GetComponent<Game>().FinalVictory();
        	}
        }
    }

    //Screech
    public void Screeching() {
        audiosource.PlayOneShot(Screech);
    }

    //Slower RNG to spawn ants and bees
    IEnumerator RNG() {

    	//Will summoning once the player hurt the Jewel Wasp
    	if (CanScreech && Self.health > 0 && Self.health < Health && GameObject.Find("Main Camera").GetComponent<Game>().More()) {
        	CanScreech = false;
        	StartCoroutine(Summon());
        }

        //Shoots stingers at the player
        else if (Self.move) {
        	Instantiate(Stinger, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(3.0f);
        StartCoroutine(RNG());
    }

    //Spawning ability
    IEnumerator Summon() {

    	//Stops to lay first
    	GetComponent<Animator>().SetInteger("Screech", 1);
    	Self.move = false;

    	//After a while screech
    	yield return new WaitForSeconds(2.0f);
    	GetComponent<Animator>().SetInteger("Screech", 2);
        yield return new WaitForSeconds(0.5f);
        Screeching();

    	//This will be to call in reinforcement
    	GameObject.Find("Main Camera").GetComponent<Game>().Reinforcement();
    	yield return new WaitForSeconds(2.0f);

	    //Resets
	    GetComponent<Animator>().SetInteger("Screech", 0);
	    Self.move = true;
	    CanScreech = true;
    }
}
