using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace ProceduralGeneration
{
	public class Player : MonoBehaviour
    {
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
            //Get a component reference to the Player's animator component
            animator = GetComponentInChildren<Animator>();

            //Get the current food point total stored in GameManager.instance between levels.

            //Set the foodText to reflect the current player food total.
            //foodText.text = "Food: " + food;

        }


        void setPlayerStats()
        {
            PlayerStats = new Statistics()
            {
                MaxHealth = 5,
                MaxMana = 5,
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


        private void Update()
        {

        }





        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {


            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "Exit")
            {
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                Invoke("Restart", restartLevelDelay);

                //Disable the player object since level is over.
                enabled = false;
            }

            //Check if the tag of the trigger collided with is Food.
            else if (other.tag == "Food")
            {
               
            }

            //Check if the tag of the trigger collided with is Soda.
            else if (other.tag == "Soda")
            {

            }
        }


        //Restart reloads the scene when called.
        private void Restart()
        {
            //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }


        //LoseFood is called when an enemy attacks the player.
        //It takes a parameter loss which specifies how many points to lose.
        public void LoseFood(int loss)
        {
            //Set the trigger for the player animator to transition to the playerHit animation.
            animator.SetTrigger("playerHurt");

           

            //Check to see if game has ended.
            CheckIfGameOver();
        }


        //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
        private void CheckIfGameOver()
        {
            ////Check if food point total is less than or equal to zero.
            //if (food <= 0)
            //{
            //    //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            //    SoundManager.instance.PlaySingle(gameOverSound);

            //    //Stop the background music.
            //    SoundManager.instance.musicSource.Stop();

            //    //Call the GameOver function of GameManager.
            //    GameManager.instance.GameOver();
            //}
        }
    }
}

