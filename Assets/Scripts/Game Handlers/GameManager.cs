using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProceduralGeneration
{
	using System.Collections.Generic;       //Allows us to use Lists. 
    using TMPro;
    using UnityEngine.UI;                   //Allows us to use UI.

    public class GameManager : MonoBehaviour
    {
        public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
        public float turnDelay = 0f;                            //Delay between each Player turn.
        public int playerFoodPoints = 100;                      //Starting value for Player food points.
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        private BoardCreator boardScript;                       //Store a reference to our BoardManager which will set up the level.

        private TextMeshProUGUI levelText;                      //Text to display current level number.
        private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
                                                                //private BoardManager boardScript;						
        public int level = 1;                                   //Current level number, expressed in game as "Day 1".
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.
        public bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.


        public bool stairsExist;                                  //Used to check if stairs have been placed in dungeon.
        public GameObject Stairs;                                 //GameObject that player will have to find in order to progress to next floor.

        public GameObject effectsMgr;

        public TextMeshProUGUI txDebug;
        public bool gameOver;
        
        public int TotalScore = 0;
        public int enemiesKilled = 0;
        void CleanUpScene()
        {
            enemies = new List<Enemy>();


            InitGame();

            //Destroy(GameObject.FindWithTag())
        }

     
        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Assign enemies to a new List of Enemy objects.
            enemies = new List<Enemy>();

            //Get a component reference to the attached BoardManager script
            boardScript = GetComponent<BoardCreator>();

            //Get a reference to our image LevelImage by finding it by name.
            levelImage = GameObject.FindWithTag("LvImg");

            //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
            levelText = levelImage.GetComponentInChildren<TextMeshProUGUI>();

            //txDebug = GameObject.FindWithTag("txDebug").GetComponent<TextMeshProUGUI>();

            //Call the InitGame function to initialize the first level 
            InitGame();
        }



        private void Start()
        {
           // Invoke("GenerateTasks", levelStartDelay);

        }


        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;
            Debug.LogWarning("OnSceneLoaded was called!");
            if(SceneManager.GetActiveScene().name == "GameWorld")
            {
                instance.InitGame();
            }
            
        }

        public void AddObjective(GameObject item)
        {
            boardScript.objectivesToSpawn.Add(item);
        }

        //Initializes the game for each level.
        void InitGame()
        {
            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            doingSetup = true;


            //Clear any Enemy objects in our List to prepare for next level.
            enemies.Clear();
            enemiesKilled = 0;
            //Set the text of levelText to the string "Day" and append the current level number.
            levelText.text = "Floor: " + "F" + level;

            //Set levelImage to active blocking player's view of the game board during setup.
            levelImage.SetActive(true);

            //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
            Invoke("HideLevelImage", levelStartDelay);

            //if (level > 1)
            //{
            //    boardScript.numRooms.m_Max += (level);
            //}

            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardScript.SetupFloor();
        }





		//Restart reloads the scene when called.
		private void Restart()
		{
			CleanUpScene();
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
			//and not load all the scene object in the current scene.
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}

		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);
			
			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
		}
		
		//Update is called every frame.
		void FixedUpdate()
		{
            //         if(Stairs != null)
            //CheckForExit();

            //         txDebug.text = string.Format("Mouse X: {0}\nMouse Y: {1}", Input.mousePosition.x, Input.mousePosition.y);
            if (gameOver)
            {
                Debug.Log("Game is over!");
                if (Input.GetKeyUp(KeyCode.R))
                {
                    Debug.Log("Game over, game restarts!");
                    gameOver = false;
                    Invoke("Restart", 0.1f);
                }
            }
        }

		public void CheckForExit()
		{
			foreach(Task t in Task_Handler.tasks_instance.tasks)
			{
				if (t.isLast && t.isActive)
				{
					Stairs.SetActive(true);
				}
			}
		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
            if(!gameOver)
                gameOver = true;

			//Set levelText to display number of levels passed and game over message
			levelText.text = "After " + level + " floors, you've died.\nPress 'R' Button to Restart.";
			
			//Enable black background image gameObject.
			levelImage.SetActive(true);
			
			//Disable this GameManager.
			enabled = false;
		}
		
		
	}
}

