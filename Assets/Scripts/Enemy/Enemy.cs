using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralGeneration
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MonoBehaviour
	{
		public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.


		private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target;                           //Transform to attempt to move toward each turn.

		public Statistics EnemyStats;
		public float DetectionRadius;
		public Vector2 OrigPos;
		public Rigidbody2D rb2d;
		public float DelayToReturn;
		public float delayBetweenAttacks;
		public bool isAttacking;

		public float recoverTime = 1f;
		public bool cantMove = false;
		public float bounceBackRate = 5f;
		public GameObject Effect;
        public string enemyName;

		//Start overrides the virtual Start function of the base class.
		void Start()
		{
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList(this);
            
			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator>();

			//Find the Player GameObject using it's tag and store a reference to its transform component.
			target = GameObject.FindGameObjectWithTag("Player").transform;

			OrigPos = transform.position;

			rb2d = GetComponent<Rigidbody2D>();
            Effect = GameManager.instance.effectsMgr;
            enemyName = gameObject.name;
		}





        private void CheckTasks()
        {
            if(Task_Handler.tasks_instance != null)
            {
                if(Task_Handler.tasks_instance.tasks != null)
                {
                    //To see if enemy is part of a task, that player needs to do, we first get the list of tasks.
                    foreach (Task t in Task_Handler.tasks_instance.tasks)
                    {
                        //First we check if the task we're iterating is a Kill type since that's only one involving enemies
                        if (t.goalToComplete.goalToDo == Category.Kill)
                        {
                            if ((t.goalToComplete.whatToAchieve.SpawnableItem.name + "(Clone)").ToUpper() == enemyName.ToUpper())
                            {
                                t.goalToComplete.current++;
                            }
                        }
                    }
                }
            }
        }
		private void FixedUpdate()
		{
			//Check Enemy's health if he's still alive
			if (EnemyStats.Health <= 0)
			{
				//Create an effect object for animation
				Instantiate(Effect, transform.position, transform.rotation);

				if (GameObject.FindWithTag("Effect") != null)
				{
					GameObject.FindWithTag("Effect").GetComponent<Animator>().SetTrigger("Explosion");
                    CheckTasks();
					//Get rid of enemy object
					Destroy(gameObject);

				}
				else
				{
					Debug.Log("No effects object!");
				}
			}

			//Enemy movement and following mechanic:

			//Check if player has entered enemy's detection radius (Set above)
			if (CheckPlayerDetected())
			{
				//Check if Enemy's distance to player is above certain value in order to make it stop once it's close enough to player (in order to attack)
				if(Vector2.Distance(rb2d.position, target.position) > 0.98)
					{
						//Move Enemy's position.
						rb2d.MovePosition(Vector2.MoveTowards(transform.position, target.position, (EnemyStats.Speed * Time.deltaTime)));
						//Debug.Log(string.Format("Player was detected inside detection radius of {0}", gameObject.name));
					}
					if(Vector2.Distance(rb2d.position, target.position) <= 0.98)
					{
						if(!isAttacking)
						{
							
							StartCoroutine(PlayerDamage());
						}
					}
				}
			else
			{
				//Once player leaves, make it return to original position.
				//TODO: make sure enemy cannot leave its room.
				StartCoroutine(ReturnToPosition());
			}
		}

		IEnumerator PlayerDamage()
		{
			isAttacking = true;
			animator.SetTrigger("enemyAttack");
			target.GetComponent<Player>().PlayerStats.Health -= EnemyStats.Damage;
			target.GetComponentInChildren<Animator>().SetTrigger("playerHurt");
			yield return new WaitForSeconds(delayBetweenAttacks);
			isAttacking = false;
		}

		IEnumerator ReturnToPosition()
		{
			//Check if Enemy has moved at all.
			if (Vector2.Distance(rb2d.position, OrigPos) > 1)
			{
				//Give enemy a little time to make sure player has really left.
				yield return new WaitForSeconds(DelayToReturn);

				//Check to make sure if player has not entered the detection radius again in the meantime.
				if (!CheckPlayerDetected())
				{
					//Debug.Log("Player's gone, returning to original position!");
					//Go back to original position.
					rb2d.MovePosition(Vector2.MoveTowards(transform.position, OrigPos, (EnemyStats.Speed * Time.deltaTime)));
				}
				else
				{
					//Player came back so break the returning in order to start following player again.
					yield break;
				}
			}
		}
		
		bool CheckPlayerDetected()
		{
            if(GameObject.FindWithTag("Player") != null)
            {
                ContactFilter2D filter = new ContactFilter2D();

                List<Collider2D> results = new List<Collider2D>();
                if (Physics2D.OverlapCircle(transform.position, DetectionRadius, filter, results) > 0)
                {
                    foreach (Collider2D result in results)
                    {
                        if (result.CompareTag("Player"))
                        {
                            return true;
                        }
                    }
                }
            }
			return false;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("Wall"))
			{
				StartCoroutine(BounceBack(collision.gameObject));
			}

		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			
			if (other.gameObject.CompareTag("Weapon"))
			{
					//other.gameObject.GetComponent<Animator>().SetTrigger("EnemyHurt");
					gameObject.GetComponent<Enemy>().EnemyStats.Health -= other.gameObject.GetComponentInParent<Player>().PlayerStats.Damage;
					StartCoroutine(DisplayDamage(other.gameObject.GetComponentInParent<Player>().gameObject));

			}
             else if (other.gameObject.CompareTag("Fireball"))
            {
                //other.gameObject.GetComponent<Animator>().SetTrigger("EnemyHurt");
                gameObject.GetComponent<Enemy>().EnemyStats.Health -= other.gameObject.GetComponentInParent<Fireball>().projStats.Damage;
                other.gameObject.GetComponent<Fireball>().OnHit();
                StartCoroutine(DisplayDamage(other.gameObject));

            }
        }
		IEnumerator BounceBack(GameObject other)
		{
			cantMove = true;

			rb2d.velocity = new Vector2((transform.position.x - other.transform.position.x) * bounceBackRate, rb2d.velocity.y);

			yield return new WaitForSeconds(recoverTime);
			cantMove = false;

			rb2d.velocity = Vector2.zero;
		}
		IEnumerator DisplayDamage(GameObject other)
		{
			cantMove = true;

			GetComponent<SpriteRenderer>().color = Color.red;
			rb2d.velocity = new Vector2((transform.position.x - other.transform.position.x) * bounceBackRate, rb2d.velocity.y);


			yield return new WaitForSeconds(recoverTime);
			GetComponent<SpriteRenderer>().color = Color.white;
			cantMove = false;

			rb2d.velocity = Vector2.zero;
		}

		IEnumerator DisplayDamage(GameObject other, GameObject Target)
		{
			cantMove = true;

			target.GetComponentInChildren<SpriteRenderer>().color = Color.red;
			target.GetComponent<Rigidbody2D>().velocity = new Vector2((target.GetComponent<Rigidbody2D>().transform.position.x - other.transform.position.x) * bounceBackRate, target.GetComponent<Rigidbody2D>().velocity.y);


			yield return new WaitForSeconds(recoverTime);
			target.GetComponentInChildren<SpriteRenderer>().color = Color.white;
			cantMove = false;

			target.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}

	}
	
}
