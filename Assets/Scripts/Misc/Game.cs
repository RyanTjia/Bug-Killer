using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    //The player
    public GameObject SpawnPlayer;

	//Enemies to spawn in current wave
	private GameObject Boss;

	//List of enemies
	public GameObject Roach;
	public GameObject Fly;
	public GameObject Beetle;
	public GameObject Bee;
	public GameObject Ant;
	public GameObject Queen;
    public GameObject Wasp;

	//Dictionaries and lists to make sure that equal amount of enemies spawn
	Dictionary<GameObject, int> Enemies = new Dictionary<GameObject, int>();
	List<GameObject> RandomEnemy = new List<GameObject>();

    //This is for camera movement
    private GameObject Target;

    // Start is called before the first frame update
    void Start() {
        HomeScreen();
    }

    //Update is used to update camera's position
    void Update() {
        if (Target != null) {
            transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, -10.0f);
        }
    }

    //Prepares the next wave
    public void NextWave(int next) {

    	//Restore the health of the player
    	GameObject.Find("Player(Clone)").GetComponent<Player>().RestoreHealth(100);

    	//Wave 2
    	if (next == 2) {
    		Boss = Queen;
    		Enemies.Add(Bee, 50);
    		Enemies.Add(Ant, 10);
    		RandomEnemy.Add(Bee);
    		RandomEnemy.Add(Ant);
    	}

    	GameObject.Find("Player(Clone)").GetComponent<Player>().ReceiveWeapon(next);

    	StartCoroutine(Announcement(next));
    }

    //The Jewel Wasp will call in reinforcements
    public bool More() {

        if (RandomEnemy.Count == 0) {
            return true;
        }
        return false;
    }

    public void Reinforcement() {
        Enemies.Add(Roach, 10);
        Enemies.Add(Fly, 10);
        Enemies.Add(Bee, 20);
        Enemies.Add(Ant, 5);
        RandomEnemy.Add(Roach);
        RandomEnemy.Add(Fly);
        RandomEnemy.Add(Bee);
        RandomEnemy.Add(Ant);

        StartCoroutine(WaspTroop());
    }

    //For when the Jewel Wasp has been defeated
    public void EmptyReinforcement() {
        StopCoroutine(WaspTroop());
        Enemies.Clear();
        RandomEnemy.Clear();
    }

    //This is time for Jewel Wasp's departure not death
    public void FinalVictory() {
        StartCoroutine(FinaleExit());
    }

    //Home Screen
    public void HomeScreen() {
        GameObject.Find("PlayerUI").GetComponent<Canvas>().enabled = false;
        Destroy(GameObject.Find("Player(Clone)"));
        GameObject.Find("Home Screen").GetComponent<Canvas>().enabled = true;
    }

    //Begin game
    public void BeginGame() {

    	Boss = Beetle;

    	Enemies.Clear();
        RandomEnemy.Clear();
    	/*Enemies.Add(Roach, 25);
    	Enemies.Add(Fly, 25);
    	RandomEnemy.Add(Roach);
    	RandomEnemy.Add(Fly);*/

        //Resets camera to the center
        transform.position = new Vector3(0.0f, 0.0f, -10.0f);

        GameObject.Find("PlayerUI").GetComponent<Canvas>().enabled = true;

        //Camera will follow the player
        Instantiate(SpawnPlayer, new Vector2(0.0f, 0.0f), Quaternion.identity);
        Target = GameObject.Find("Player(Clone)");

        GameObject.Find("Home Screen").GetComponent<Canvas>().enabled = false;
        StartCoroutine(Announcement(1));
    }

    //Shows the book
    public void ShowBook(GameObject Book) {

        if (!GameObject.Find("Book(Clone)")) {
            Instantiate(Book, GameObject.Find("Home Screen").transform.position, Quaternion.identity, GameObject.Find("Home Screen").transform);
        }
    }

    //Shows how to play
    public void HowTo(GameObject Help) {

    	if (!GameObject.Find("HelpPage(Clone)")) {
    		Instantiate(Help, GameObject.Find("Home Screen").transform.position, Quaternion.identity, GameObject.Find("Home Screen").transform);
    	}
    }

    //For when the player died
    public void PlayerLost() {

    	Destroy(GameObject.Find("Player(Clone)").transform.GetChild(0).gameObject);
    	GameObject.Find("Player(Clone)").GetComponent<Player>().enabled = false;
    	GameObject.Find("Player(Clone)").GetComponent<CircleCollider2D>().enabled = false;
    	StartCoroutine(GameOver());
    }

    //Announcement
    IEnumerator Announcement(int wave) {

    	float alpha = 0.0f;

    	if (wave == 3) {
    		GameObject.Find("Announcement").GetComponent<Text>().text = "Final Wave";
    	}

    	else {
    		GameObject.Find("Announcement").GetComponent<Text>().text = "Wave " + wave;
    	}

    	//Fades in
    	while (alpha < 1.0f) {
    		GameObject.Find("Announcement").GetComponent<Text>().color = new Color (1.0f, 1.0f, 0.0f, alpha);
    		alpha = alpha + 0.01f;
    		yield return new WaitForSeconds(0.01f);
    	}

    	yield return new WaitForSeconds(2.5f);

    	//Fades out
    	while (alpha > 0.0f) {
    		GameObject.Find("Announcement").GetComponent<Text>().color = new Color (1.0f, 1.0f, 0.0f, alpha);
    		alpha = alpha - 0.01f;
    		yield return new WaitForSeconds(0.01f);
    	}

    	//Start the wave
        if (wave == 3) {
            StartCoroutine(FinaleEntrance());
        }

        else {
            StartCoroutine(Spawn());
        }
    }

    //Time to spawn enemies
    IEnumerator Spawn() {

    	float x = 0.0f;
    	float y = 0.0f;

    	while (true && GameObject.Find("Player(Clone)")) {

    		//Making sure that there isn't too much enemy
    		if (GameObject.Find("Enemies").transform.childCount < 20 && RandomEnemy.Count > 0) {

	    		//Randomizes where the enemies will be around the border
	    		if (Random.Range(0, 2) == 0) {
	    			x = Random.Range(-12.0f, 12.0f);
	    			y = Mathf.Sign(Random.Range(-1, 1)) * 12.0f;
	    		}

	    		else {
	    			y = Random.Range(-12.0f, 12.0f);
	    			x = Mathf.Sign(Random.Range(-1, 1)) * 12.0f;
	    		}

	    		//Instantiate the enemy
	    		GameObject CurrentEnemy = RandomEnemy[Random.Range(0, RandomEnemy.Count)];
	    		Instantiate(CurrentEnemy, new Vector2(x, y), Quaternion.identity, GameObject.Find("Enemies").transform);
	    		Enemies[CurrentEnemy] = Enemies[CurrentEnemy] - 1;

	    		if(Enemies[CurrentEnemy] <= 0) {
	    			Enemies.Remove(CurrentEnemy);
	    			RandomEnemy.RemoveAt(RandomEnemy.IndexOf(CurrentEnemy));
	    		}
	    	}

	    	//Once all enemies have been killed, summon the boss
	    	else if (GameObject.Find("Enemies").transform.childCount == 0) {
	    		Instantiate(Boss, new Vector2(0.0f, 10.0f), Quaternion.identity, GameObject.Find("Enemies").transform);
	    		Boss = null;
	    	}

	    	//Stops the loop after boss has spawned
	    	else if (Boss == null) {
	    		break;
	    	}

	    	yield return new WaitForSeconds(1.0f);
    	}
    }

    //This is Jewel Wasp's ability
    IEnumerator WaspTroop() {

        float x = 0.0f;
        float y = 0.0f;

        while (true && GameObject.Find("Player(Clone)")) {

            //Making sure that there isn't too much enemy
            if (GameObject.Find("Enemies").transform.childCount < 31 && RandomEnemy.Count > 0) {

                //Randomizes where the enemies will be around the border
                if (Random.Range(0, 2) == 0) {
                    x = Random.Range(-12.0f, 12.0f);
                    y = Mathf.Sign(Random.Range(-1, 1)) * 12.0f;
                }

                else {
                    y = Random.Range(-12.0f, 12.0f);
                    x = Mathf.Sign(Random.Range(-1, 1)) * 12.0f;
                }

                //Instantiate the enemy
                GameObject CurrentEnemy = RandomEnemy[Random.Range(0, RandomEnemy.Count)];
                Instantiate(CurrentEnemy, new Vector2(x, y), Quaternion.identity, GameObject.Find("Enemies").transform);
                Enemies[CurrentEnemy] = Enemies[CurrentEnemy] - 1;

                if(Enemies[CurrentEnemy] <= 0) {
                    Enemies.Remove(CurrentEnemy);
                    RandomEnemy.RemoveAt(RandomEnemy.IndexOf(CurrentEnemy));
                }
            }

            //Once all enemies have been summoned, then stopped this coroutine
            else if (RandomEnemy.Count == 0) {
                break;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    //Game Over
    IEnumerator GameOver() {

    	float alpha = 0.0f;

    	GameObject.Find("Announcement").GetComponent<Text>().text = "Game Over";

    	//Fades in
    	while (alpha < 1.0f) {
    		GameObject.Find("Announcement").GetComponent<Text>().color = new Color (1.0f, 0.0f, 0.0f, alpha);
    		alpha = alpha + 0.01f;
    		yield return new WaitForSeconds(0.01f);
    	}

    	yield return new WaitForSeconds(2.5f);

    	//Fades out
    	while (alpha > 0.0f) {
    		GameObject.Find("Announcement").GetComponent<Text>().color = new Color (1.0f, 0.0f, 0.0f, alpha);
    		alpha = alpha - 0.01f;
    		yield return new WaitForSeconds(0.01f);
    	}

    	yield return new WaitForSeconds(2.0f);

    	//Will return player to home screen
        foreach (Transform child in GameObject.Find("Enemies").transform) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in GameObject.Find("EnemyUI").transform) {
            Destroy(child.gameObject);
        }

        if (GameObject.Find("Jewel Wasp(Clone)")) {
        	Destroy(GameObject.Find("Jewel Wasp(Clone)"));
        }

    	HomeScreen();
    }

    //"Cutscene" for final boss entrance
    IEnumerator FinaleEntrance() {

        //Instantiate the final boss and have the camera focus on it for a while
        GameObject FinalBoss = Instantiate(Wasp, new Vector2(0.0f, -20.0f), Quaternion.identity);
        Target = FinalBoss;

        //The final boss will fly up first
        while (FinalBoss.transform.position.y < 20.0f) {
            FinalBoss.transform.position = Vector2.MoveTowards(FinalBoss.transform.position, new Vector2(0.0f, 20.0f), 0.3f);
            yield return new WaitForSeconds(0.00000001f);
        }

        //Rotate to look down
        FinalBoss.transform.rotation = Quaternion.Euler(0, 0, 180);

        //The final boss will then fly down at the top border
        while (FinalBoss.transform.position.y > 11.0f) {
            FinalBoss.transform.position = Vector2.MoveTowards(FinalBoss.transform.position, new Vector2(0.0f, 11.0f), 0.15f);
            yield return new WaitForSeconds(0.00000001f);
        }

        //Boss will "screech" for a while
        FinalBoss.GetComponent<Animator>().SetInteger("Screech", 2);
        yield return new WaitForSeconds(0.5f);
        FinalBoss.GetComponent<JewelWasp>().Screeching();
        yield return new WaitForSeconds(2.0f);
        FinalBoss.GetComponent<Animator>().SetInteger("Screech", 0);

        //Return camera back to the player
        Target = GameObject.Find("Player(Clone)");

        //Begin the fight
        FinalBoss.GetComponent<BoxCollider2D>().enabled = true;
        FinalBoss.GetComponent<Enemy>().move = true;
    }

    //"Cutscene" for final boss exit
    IEnumerator FinaleExit() {

    	//Making sure player doesn't die
    	GameObject.Find("Player(Clone)").GetComponent<Player>().RestoreHealth(100);

        //Have the camera focus on the final boss for a while
        GameObject FinalBoss = GameObject.Find("Jewel Wasp(Clone)");
        FinalBoss.GetComponent<BoxCollider2D>().enabled = false;
        FinalBoss.GetComponent<Enemy>().move = false;
        Target = FinalBoss;

        //Looks at the player one last time
        float AngleRad = Mathf.Atan2(GameObject.Find("Player(Clone)").transform.position.y - FinalBoss.transform.position.y, GameObject.Find("Player(Clone)").transform.position.x - FinalBoss.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        FinalBoss.transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);

        //Boss will "screech" for a while
        FinalBoss.GetComponent<Animator>().SetInteger("Screech", 1);
        yield return new WaitForSeconds(1.0f);
        FinalBoss.GetComponent<Animator>().SetInteger("Screech", 2);
        yield return new WaitForSeconds(0.5f);
        FinalBoss.GetComponent<JewelWasp>().Screeching();
        yield return new WaitForSeconds(2.0f);
        FinalBoss.GetComponent<Animator>().SetInteger("Screech", 0);

        //Rotate to look up
        FinalBoss.transform.rotation = Quaternion.Euler(0, 0, 0);

        //The final boss will fly up
        while (FinalBoss.transform.position.y < 20.0f) {
            FinalBoss.transform.position = Vector2.MoveTowards(FinalBoss.transform.position, new Vector2(0.0f, 20.0f), 0.3f);
            yield return new WaitForSeconds(0.00000001f);
        }

        Destroy(FinalBoss);

        //Return camera back to the player
        Target = GameObject.Find("Player(Clone)");

        //Congratulate the player
        float alpha = 0.0f;

        GameObject.Find("Announcement").GetComponent<Text>().text = "You beat the game!!!";

        //Fades in
        while (alpha < 1.0f) {
            GameObject.Find("Announcement").GetComponent<Text>().color = new Color (1.0f, 1.0f, 0.0f, alpha);
            alpha = alpha + 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1.5f);

        //Fades out
        while (alpha > 0.0f) {
            GameObject.Find("Announcement").GetComponent<Text>().color = new Color (1.0f, 1.0f, 0.0f, alpha);
            alpha = alpha - 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        //Will return player to home screen
        HomeScreen();
    }
}
