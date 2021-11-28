using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Beetle : MonoBehaviour {

	private Enemy Self;
	private bool CanRam;
	private bool ready;

	//Own health bar
	public GameObject HealthBar;
	private int Health;
    private RectTransform CurrentHealth;
    private float MaxHealthSize;

    //For moving and looking at the player
	private Vector2 Target;
    private float AngleRad;
    private float AngleDeg;

    // Start is called before the first frame update
    void Start() {

    	//This is to make the boss unique from the other enemies
        Self = GetComponent<Enemy>();
        CanRam = true;
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
        HealthBar.transform.GetChild(1).GetComponent<Text>().text = "Beetle";
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
        	StopCoroutine(Ram());
        	GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        	if (!ready) {
        		ready = true;
        		Destroy(HealthBar);
        		GameObject.Find("Main Camera").GetComponent<Game>().NextWave(2);
        	}
        }
    }

    //Slower RNG to ram
    IEnumerator RNG() {

    	if (Random.Range(1, (Self.health / 2) + 1) <= 12 && CanRam && Self.health > 0) {
        	CanRam = false;
        	StartCoroutine(Ram());
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RNG());
    }

    //Ramming ability
    IEnumerator Ram() {

    	Vector2 Target;
    	float color = 1.0f;

    	//Stops to charge first
    	GetComponent<Animator>().ResetTrigger("Reset");
    	GetComponent<Animator>().SetBool("Charging", true);
    	Self.move = false;

    	//Becomes red to show enraged while charging
    	//Also focus on the player
    	while (color > 0.0f && Self.health > 0) {
            GetComponent<SpriteRenderer>().color = new Color (1.0f, color, color, 1.0f);
            Target = GameObject.Find("Player(Clone)").transform.position;
	        AngleRad = Mathf.Atan2(Target.y - transform.position.y, Target.x - transform.position.x);
	        AngleDeg = (180 / Mathf.PI) * AngleRad;
	        transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
	        color = color - 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

	    //Charges at them
	    Target = GameObject.Find("Player(Clone)").transform.position;
	    AngleRad = Mathf.Atan2(Target.y - transform.position.y, Target.x - transform.position.x);
	    Target = new Vector2(((180 / Mathf.PI) * Mathf.Cos(AngleRad)) * 1000.0f, ((180 / Mathf.PI) * Mathf.Sin(AngleRad)) * 1000.0f);
	    GetComponent<Animator>().SetBool("Charging", false);

	    //Changes damage to 20
	    GetComponent<Enemy>().BuffStat(2.0, 0.0f);

	    //Will continue until it hit the border
	    while ((transform.position.x < 11.0f && transform.position.x > -11.0f) && 
	    	(transform.position.y < 11.0f && transform.position.y > -11.0f)) {

        	transform.position = Vector2.MoveTowards(transform.position, Target, 1.0f);
        	yield return new WaitForSeconds(0.001f);
	    }

	    //If the beetle is past the border then bring it back
	    if (transform.position.x > 11.0f) {
	    	transform.position = new Vector2(11.0f, transform.position.y);
	    }
	    else if (transform.position.x < -11.0f) {
	    	transform.position = new Vector2(-11.0f, transform.position.y);
	    }

	    if (transform.position.y > 11.0f) {
	    	transform.position = new Vector2(transform.position.x, 11.0f);
	    }
	    else if (transform.position.y < -11.0f) {
	    	transform.position = new Vector2(transform.position.x, -11.0f);
	    }

	    //Resets
	    GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	    GetComponent<Animator>().SetTrigger("Reset");
	    GetComponent<Enemy>().BuffStat(0.0, 0.0f);
	    Self.move = true;
	    CanRam = true;
    }
}
