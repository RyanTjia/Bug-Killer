using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pistol : MonoBehaviour {

	public GameObject bullet;
    public Sprite Used;
	private GameObject bulletClone;
	private bool Fire;
	private bool Refilling;

	//Changing the text
	private Text CurrentAmmoText;
	private RectTransform Reload;
	private float ReloadSize;

    //Audio
    private AudioSource audiosource;
    public AudioClip ShootSound;

	//Initialize some stuff
	void Start() {

		//Players can shoot and reload
		Fire = true;
		Refilling = false;

		//For updating the pistol UI
		CurrentAmmoText = GameObject.Find("Slot(1)/CurrentAmmo").GetComponent<Text>();
		Reload = GameObject.Find("Slot(1)/Reloading").GetComponent<RectTransform>();
		ReloadSize = 60.0f;
		Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        //Lets the player know if they chose this weapon
        GameObject.Find("Slot(1)").GetComponent<Image>().overrideSprite = Used;

        audiosource = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update() {
        
        //Player must press the left mouse button to shoot
        if (Input.GetMouseButtonDown(0) && Player.CurrentBullet > 0 && Fire && !Refilling) {
        	Fire = false;
        	StartCoroutine(Fired());
        	bulletClone = Instantiate(bullet, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        	Player.CurrentBullet--;
        	CurrentAmmoText.text = "" + Player.CurrentBullet;
        }

        //Reload their current ammo
        else if ((Input.GetKeyDown(KeyCode.R) && Player.CurrentBullet < 10) || 
        	(Input.GetMouseButtonDown(0) && Player.CurrentBullet <= 0) && !Refilling) {
        	
        	Refilling = true;
        	StartCoroutine(Reloading());
        }
    }

    //Cooldowns
    IEnumerator Fired() {
        audiosource.PlayOneShot(ShootSound);
    	yield return new WaitForSeconds(0.25f);
    	Fire = true;
    }

    IEnumerator Reloading() {
    	float increase = 0;

        audiosource.Play(0);
        
    	while (increase < 1.0f) {
    		Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ReloadSize * increase);
    		increase = increase + 0.01f;
    		yield return new WaitForSeconds(0.00000000000000001f);
    	}

        audiosource.Stop();
    	Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    	CurrentAmmoText.text = "10";
    	Player.CurrentBullet = 10;
    	Refilling = false;
    }
}
