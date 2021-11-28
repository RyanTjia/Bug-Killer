using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun : MonoBehaviour {

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
    public AudioClip ReloadSound;

	//Initialize some stuff
	void Start() {

		//Players can shoot and reload
		Fire = true;
		Refilling = false;

		//For updating the pistol UI
		CurrentAmmoText = GameObject.Find("Slot(2)/CurrentAmmo").GetComponent<Text>();
		Reload = GameObject.Find("Slot(2)/Reloading").GetComponent<RectTransform>();
		ReloadSize = 60.0f;
		Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        //Lets the player know if they chose this weapon
        GameObject.Find("Slot(2)").GetComponent<Image>().overrideSprite = Used;

        audiosource = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update() {
        
        //Player must press the left mouse button to shoot
        if (Input.GetMouseButtonDown(0) && Player.CurrentShell > 0 && Fire) {
        	Fire = false;
        	StartCoroutine(Fired());

        	//Five bullets will be shot at the cost of one
        	for (int x = -2; x <= 2; x++) {
	        	bulletClone = Instantiate(bullet, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
	        	bulletClone.GetComponent<Shot>().SpreadShot((float)(x * 5));
	        }

        	Player.CurrentShell--;
        	CurrentAmmoText.text = "" + Player.CurrentShell;

        	//If player press LMB while in the middle of reloading, then stop reloading
        	if (Refilling) {
        		StopCoroutine(Reloading());
        		Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        		Refilling = false;
        	}
        }

        //Reload their current ammo
        else if (((Input.GetKeyDown(KeyCode.R) && Player.CurrentShell < 5) || 
        	(Input.GetMouseButtonDown(0) && Player.CurrentShell <= 0)) && !Refilling) {
        	
        	Refilling = true;
        	StartCoroutine(Reloading());
        }
    }

    //Cooldowns
    IEnumerator Fired() {
        audiosource.PlayOneShot(ShootSound);
    	yield return new WaitForSeconds(2.5f);
    	Fire = true;
    }

    IEnumerator Reloading() {
    	float increase = 0;

    	//Will continue refilling until player reached max current shells or if they pressed the LMB
    	while ((!Input.GetMouseButtonDown(0) || Player.CurrentShell == 0) && Player.CurrentShell < 5) {
	    	while ((!Input.GetMouseButtonDown(0) || Player.CurrentShell == 0) && increase < 1.0f) {
	    		Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ReloadSize * increase);
	    		increase = increase + 0.05f;
	    		yield return new WaitForSeconds(0.00000000000000001f);
	    	}

            audiosource.PlayOneShot(ReloadSound);

	    	if ((!Input.GetMouseButtonDown(0) || Player.CurrentShell == 0)) {
		    	increase = 0;
		    	Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
		    	Player.CurrentShell++;
		    	CurrentAmmoText.text = "" + Player.CurrentShell;
		    }
	    }

        audiosource.Stop();
	    Reload.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    	Refilling = false;
    }
}
