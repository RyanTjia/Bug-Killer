using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sting : MonoBehaviour {

	//These will be for the projectile's aim
    private Rigidbody2D rigidbody2d;
    private GameObject Target;
    private float adjacent;
    private float opposite;
    private float AngleRad;

    // Start is called before the first frame update
    void Awake() {

    	//This command gathers information of the rigidbody, used for collisions and physics
        rigidbody2d = GetComponent<Rigidbody2D>();

        //Finding the angle at which to aim
        Target = GameObject.Find("Player(Clone)");
        AngleRad = Mathf.Atan2(Target.transform.position.y - transform.position.y, Target.transform.position.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, 0, (180 / Mathf.PI) * AngleRad - 90);

        adjacent = ((180 / Mathf.PI) * Mathf.Cos(AngleRad)) * 1.0f;
        opposite = ((180 / Mathf.PI) * Mathf.Sin(AngleRad)) * 1.0f;
    }

    //This is used for physics components but it won't be called every time
    void FixedUpdate() {

        //Position of the rigidbody
        Vector2 position = rigidbody2d.position;

        //The reason why there is a "f" next to the decimal
        //it is because Unity's values are float and not double 
        position.x = position.x + 0.1f * adjacent * Time.deltaTime;
        position.y = position.y + 0.1f * opposite * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        if (transform.position.magnitude > 33.0f) {
        	Destroy(gameObject);
        }
    }

    //Hit the player
    void OnTriggerEnter2D(Collider2D stuff) {

    	Player e = stuff.GetComponent<Player>();
    	if (e != null) {
			e.ReduceHealth(5);
    	}
    }
}
