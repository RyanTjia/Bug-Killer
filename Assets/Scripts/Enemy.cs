using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    //Enemy's stat
	public int health;
    public float speed;
    public bool move;
    public int damage;
    private int finalDamage;
    private float finalSpeed;

    //Focuses on the player
	private GameObject Player;

	//For moving and looking at the player
	private Vector2 Target;
    public float AngleRad;
    public float AngleDeg;

    //This is only for the jewel wasp
    public bool FinalBoss;

    // Start is called before the first frame update
    void Awake() {
        
        //Following the player
        Player = GameObject.Find("Player(Clone)");

        //Allowed to move by default
        move = true;

        finalDamage = damage;
        finalSpeed = speed;
        FinalBoss = false;
    }

    void FixedUpdate() {

        //Allows the enemy to move towards the player
        if (move) {
            Target = Player.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, Target, finalSpeed * Time.deltaTime);

            //Constantly look at player
            AngleRad = Mathf.Atan2(Target.y - transform.position.y, Target.x - transform.position.x);
            AngleDeg = (180 / Mathf.PI) * AngleRad;
            transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
        }
    }

    //This is for when the gas buff the enemies' speed and damage
    public void BuffStat(double damagebuff, float speedbuff) {
        finalDamage = Mathf.Clamp((int)(damage * damagebuff), damage, damage * 2);
        finalSpeed = Mathf.Clamp(speed * speedbuff, speed, 2.5f);
    }

    //Reduce the enemy's health when shot
    public void ReduceHealth(int damage) {
    	health = health - damage;
    	
        //For when the enemy dies
        if (health <= 0 && !FinalBoss) {
            GetComponent<Animator>().SetBool("Dead", true);
            move = false;
            StartCoroutine(Death());
        }
    }

    //"Death Animation"
    IEnumerator Death() {

        float alpha = 1.0f;

        //Fades away
        while (alpha > 0.0f) {
            GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f, alpha);
            alpha = alpha - 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
    }

    //Touches the player
    void OnTriggerStay2D(Collider2D stuff) {

        Player e = stuff.GetComponent<Player>();
        if (e != null && health > 0) {
            e.ReduceHealth(finalDamage);
        }
    }
}
