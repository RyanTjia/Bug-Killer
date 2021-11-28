using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {
    
    public int damage;

	//These will be for the projectile's aim
    private Rigidbody2D rigidbody2d;
    private float adjacent;
    private float opposite;
    private Vector3 MousePosition;
    private float AngleRad;

    // Start is called before the first frame update
    void Awake() {

    	//This command gathers information of the rigidbody, used for collisions and physics
        rigidbody2d = GetComponent<Rigidbody2D>();

        //Finding the angle at which to aim
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        AngleRad = Mathf.Atan2(MousePosition.y - transform.position.y, MousePosition.x - transform.position.x);

        adjacent = ((180 / Mathf.PI) * Mathf.Cos(AngleRad)) * 1.0f;
        opposite = ((180 / Mathf.PI) * Mathf.Sin(AngleRad)) * 1.0f;
    }

    public void SpreadShot(float radians) {
        AngleRad += radians * Mathf.PI / 180;

        adjacent = ((180 / Mathf.PI) * Mathf.Cos(AngleRad)) * 1.0f;
        opposite = ((180 / Mathf.PI) * Mathf.Sin(AngleRad)) * 1.0f;
    }

    //This is used for physics components but it won't be called every time
    void FixedUpdate() {

        //Position of the rigidbody
        Vector2 position = rigidbody2d.position;

        //The reason why there is a "f" next to the decimal
        //it is because Unity's values are float and not double 
        position.x = position.x + 0.5f * adjacent * Time.deltaTime;
        position.y = position.y + 0.5f * opposite * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        if (transform.position.magnitude > 33.0f) {
        	Destroy(gameObject);
        }
    }

    //Pierce through the enemies
    void OnTriggerEnter2D(Collider2D stuff) {

    	Enemy e = stuff.GetComponent<Enemy>();
    	if (e != null) {
            
            //Will only work if the enemy is not dead
            if (e.health > 0) {
                e.ReduceHealth(damage);

                //If this is regular bullet then destroy it
                if (string.Equals(name, "BulletShots(Clone)")) {
                    Destroy(gameObject);
                }
            }
    	}
    }
}
