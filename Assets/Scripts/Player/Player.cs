using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace ProceduralGeneration
{
	public class Player : MonoBehaviour
    {
        public static Player playerInstance = null;				//Static instance of GameManager which allows it to be accessed by any other script.

        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
        public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
        public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
        public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
        public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
        public AudioClip gameOverSound;             //Audio clip to play when player dies.

        private Animator animator;                  //Used to store a reference to the Player's animator component.

        public Statistics PlayerStats;


#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif


        //Start overrides the Start function of MovingObject
        void Start()
        {

            //Check if instance already exists
            if (playerInstance == null)

                //if not, set instance to this
                playerInstance = this;

            //If instance already exists and it's not this:
            else if (playerInstance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Player Object.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Get a component reference to the Player's animator component
            animator = GetComponentInChildren<Animator>();

            //Get the current food point total stored in GameManager.instance between levels.

            //Set the foodText to reflect the current player food total.
            //foodText.text = "Food: " + food;
            setPlayerStats();


        }


        void setPlayerStats()
        {
            PlayerStats = new Statistics()
            {
                MaxHealth = 10,
                MaxMana = 4,
                Speed = 5f,
                Damage = 1f,
            };

            PlayerStats.Health = PlayerStats.MaxHealth;
            PlayerStats.Mana = PlayerStats.MaxMana;
        }

        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {
            //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
           // GameManager.instance.playerFoodPoints = food;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (gameObject.GetComponent<PlayerMovement>().playerLock)
            {
                GetComponent<PlayerMovement>().playerLock = false;
                enabled = true;
            }
        }

        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the tag of the trigger collided with is Exit.
            if (other.CompareTag("Exit"))
            {
                if (!GameManager.instance.doingSetup)
                {
                    //If player has reached the Boss level.
                    if (GameManager.instance.level == 3)
                    {
                        GetComponent<PlayerMovement>().playerLock = true;
                        GameManager.instance.GetComponent<BoardCreator>().enabled = false;
                        SceneManager.LoadScene("CABossRoom", LoadSceneMode.Single);
                        enabled = false;
                    }
                    else
                    {
                        GetComponent<PlayerMovement>().playerLock = true;
                        //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                        GameObject.FindWithTag("GameManager").GetComponent<GameManager>().Invoke("Restart", restartLevelDelay);
                        //Disable the player object since level is over.
                        enabled = false;

                    }
                }
            }
            else if (other.CompareTag("Collectable"))
            {
                Debug.Log("Collectible was found!");
                if (other.GetComponent<Collectible>().collType == CollectibleType.Collectible)
                {
                    GameManager.instance.TotalScore += other.GetComponent<Collectible>().score;
                    foreach (Task t in Task_Handler.tasks_instance.tasks)
                    {
                        if (t.goalToComplete.whatToAchieve.SpawnableItem == other.GetComponent<Collectible>().gameObject)
                        {
                            t.goalToComplete.current++;
                            break;
                        }
                    }

                }
                else if (other.GetComponent<Collectible>().collType == CollectibleType.HealthPotion)
                {
                    if (Player.playerInstance.PlayerStats.Health < Player.playerInstance.PlayerStats.MaxHealth + other.GetComponent<Collectible>().healAmount)
                    {
                        Player.playerInstance.PlayerStats.Health = Player.playerInstance.PlayerStats.MaxHealth;
                    }
                    else
                    {
                        Player.playerInstance.PlayerStats.Health += other.GetComponent<Collectible>().healAmount;
                    }
                }
                else if (other.GetComponent<Collectible>().collType == CollectibleType.ManaPotion)
                {
                    if (Player.playerInstance.PlayerStats.Mana < Player.playerInstance.PlayerStats.MaxMana + other.GetComponent<Collectible>().healAmount)
                    {
                        Player.playerInstance.PlayerStats.Mana = Player.playerInstance.PlayerStats.MaxMana;
                    }
                    else
                    {
                        Player.playerInstance.PlayerStats.Mana += other.GetComponent<Collectible>().healAmount;
                    }
                }
                Destroy(other.gameObject);

            }
        }

        //CheckIfGameOver checks if the player is out of health points and if so, ends the game.
        private void CheckIfGameOver()
        {
            // Check if food point total is less than or equal to zero.
            if (PlayerStats.Health <= 0)
            {
                
                //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
                SoundManager.instance.PlaySingle(gameOverSound);

                //Stop the background music.
                SoundManager.instance.musicSource.Stop();

                //Call the GameOver function of GameManager.
                GameManager.instance.GameOver();
            }
        }
    }
}

