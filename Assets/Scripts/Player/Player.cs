using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    //Character's speed
    private float speed;

    //Health
    private int currentHealth;
    private bool immunity;
    private RectTransform CurrentHealth;
    private float MaxHealthSize;
    private Text CurrentHealthText;

    //These variables were placed here so that it can be accessed in any functions
    private Rigidbody2D rigidbody2d;
    private float horizontal;
    private float vertical;

    //For changing rotation
    private Vector3 MousePosition;
    private float AngleRad;
    private float AngleDeg;

    //Saving the ammo
    public static int CurrentBullet;
    public static int CurrentShell;
    public static int CurrentRifle;

    //Weapons
    private List<GameObject> Arsenal = new List<GameObject>();
    private int slot;
    private GameObject weapon;
    public GameObject Pistol;
    public GameObject Shotgun;
    public GameObject Sniper;

    //Dashing or dodging
    private RectTransform Dash;
    private float DashSize;
    private bool Refilling;
    private bool Move;

    public bool dead;

    //Start is called before the first frame update
    void Start() {

        //This command gathers information of the rigidbody, used for collisions and physics
        rigidbody2d = GetComponent<Rigidbody2D>();

        //Initiating player's health
        currentHealth = 100;
        immunity = false;
        CurrentHealth = GameObject.Find("Health/Full").GetComponent<RectTransform>();
        CurrentHealthText = GameObject.Find("Health/Text").GetComponent<Text>();
        CurrentHealthText.text = "" + currentHealth;
        CurrentHealth.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
        MaxHealthSize = CurrentHealth.rect.width;

        //Sets speed
        speed = 3.0f;

        //Initializing the bullet count
        CurrentBullet = 10;
        CurrentShell = 5;
        CurrentRifle = 1;

        //Starting off with the pistol
        Instantiate(Pistol, transform.position, Quaternion.identity, transform);
        weapon = Pistol;
        slot = 0;
        Arsenal.Add(Pistol);

        //For updating the dash UI
        Refilling = false;
        Move = true;
        Dash = GameObject.Find("Dodge/Reloading").GetComponent<RectTransform>();
        DashSize = Dash.rect.height;
        Dash.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        dead = false;
    }

    //Update is called once per frame
    void Update() {

        //Input controls
        if (Move) {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        //This whole thing was to rotate the player character depending on where the mouse is
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        AngleRad = Mathf.Atan2(MousePosition.y - transform.position.y, MousePosition.x - transform.position.x);
        AngleDeg = (180 / Mathf.PI) * AngleRad;
        transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);

        //Players can dash by pressing either left or right shift
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && !Refilling) {
            Refilling = true;
            StartCoroutine(Reloading());
            StartCoroutine(Dashing());
        }

        //Players can change weapons by pressing "Q" or "E"
        if (Input.GetKeyDown(KeyCode.Q)) {

            //Switches icon
            GameObject.Find("Slot(" + (slot + 1) + ")").GetComponent<Image>().overrideSprite = null;

            //Remove current weapon and instantiate the next
            DestroyImmediate(GameObject.Find(weapon.name + "(Clone)"));
            GameObject.Find("Slot(" + (slot + 1) + ")/Reloading").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            
            //Moves to the left, but appears at the right if it's already at the first
            slot--;
            if (slot < 0) {
                slot = Arsenal.Count - 1;
            }

            weapon = Arsenal[slot];
            Instantiate(weapon, transform.position, transform.rotation, transform);
        }

        else if (Input.GetKeyDown(KeyCode.E)) {

            //Switches icon
            GameObject.Find("Slot(" + (slot + 1) + ")").GetComponent<Image>().overrideSprite = null;

            //Remove current weapon and instantiate the next
            DestroyImmediate(GameObject.Find(weapon.name + "(Clone)"));
            GameObject.Find("Slot(" + (slot + 1) + ")/Reloading").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            //Moves to the left, but appears at the right if it's already at the first
            slot++;
            if (slot > Arsenal.Count - 1) {
                slot = 0;
            }

            weapon = Arsenal[slot];
            Instantiate(weapon, transform.position, transform.rotation, transform);
        }
    }

    //This is used for physics components but it won't be called every time
    void FixedUpdate() {

        //Position of the rigidbody
        Vector2 position = rigidbody2d.position;

        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        //But if player reaches the edge then they must be placed back in
        if (Mathf.Abs(position.x) > 11.0f) {
            position.x = Mathf.Sign(position.x) *  11.0f;
        }

        if (Mathf.Abs(position.y) > 11.0f) {
            position.y = Mathf.Sign(position.y) *  11.0f;
        }

        rigidbody2d.MovePosition(position);
    }

    //Receive next weapon
    public void ReceiveWeapon(int wave) {

        if (wave == 2) {
            Arsenal.Add(Shotgun);
            StartCoroutine(SubAnnoucement("+Shotgun"));
        }

        else if (wave == 3) {
            Arsenal.Add(Sniper);
            StartCoroutine(SubAnnoucement("+Sniper Rifle"));
        }
    }

    //Receive damage
    public void ReduceHealth(int damage) {

        //Gives immunity
        if (!immunity) {
            immunity = true;
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, 100);
            StartCoroutine(ImmunityFrame());
        }

        //Update the health bar
        CurrentHealth.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MaxHealthSize * currentHealth / 100);
        CurrentHealthText.text = "" + currentHealth;

        //If player has zero health then do a game over
        if (currentHealth <= 0 && !dead) {
            dead = true;
            StopCoroutine(ImmunityFrame());
            GameObject.Find("Main Camera").GetComponent<Game>().PlayerLost();
        }
    }

    //Restore health
    public void RestoreHealth(int amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, 100);
        CurrentHealth.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MaxHealthSize * currentHealth / 100);
        CurrentHealthText.text = "" + currentHealth;
    }

    //Gets player's immunity frame
    public bool GetImmunity() {
        return immunity;
    }

    //Immunity frame
    IEnumerator ImmunityFrame() {

        float alpha = 1.0f;

        //Continously make the player's sprite flash to show immunity
        for (int x = 0; x < 3; x++) {

            while (alpha > 0.0f && currentHealth > 0) {
                GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f, alpha);
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f, alpha);
                alpha = alpha - 0.05f;
                yield return new WaitForSeconds(0.00001f);
            }

            while (alpha < 1.0f && currentHealth > 0) {
                GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f, alpha);
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color (1.0f, 1.0f, 1.0f, alpha);
                alpha = alpha + 0.05f;
                yield return new WaitForSeconds(0.00001f);
            }
        }

        immunity = false;
    }

    //For dashing purposes
    IEnumerator Reloading() {
        float increase = 0;

        while (increase < 1.0f) {
            Dash.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, DashSize * increase);
            increase = increase + 0.005f;
            yield return new WaitForSeconds(0.00000000000000001f);
        }

        Dash.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        Refilling = false;
    }

    IEnumerator Dashing() {
        speed = 5.0f;
        immunity  = true;
        Move = false;

        //This is because sometimes, the dash is not as fast as intended
        if (horizontal != 0.0f) {
            horizontal = 1.0f * Mathf.Sign(horizontal);
        }

        if (vertical != 0.0f) {
            vertical = 1.0f * Mathf.Sign(vertical);
        }

        yield return new WaitForSeconds(0.5f);
        speed = 3.0f;
        immunity = false;
        Move = true;
    }

    //Sub-Annoucement
    IEnumerator SubAnnoucement(string text) {

        GameObject.Find("Sub-Announcement").GetComponent<Text>().text = text;
        yield return new WaitForSeconds(1.0f);

        GameObject.Find("Sub-Announcement").GetComponent<Text>().text = "";
    }
}
