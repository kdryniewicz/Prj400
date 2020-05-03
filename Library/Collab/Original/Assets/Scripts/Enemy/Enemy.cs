using UnityEngine;
using System.Collections;

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


		public float recoverTime = 1f;
		public bool invincible = false;

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


		}


		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.tag == "Weapon")
			{
				if (!invincible)
				{
					//other.gameObject.GetComponent<Animator>().SetTrigger("EnemyHurt");
					gameObject.GetComponent<Enemy>().EnemyStats.Health -= other.gameObject.GetComponentInParent<Player>().PlayerStats.Damage;
					StartCoroutine(DisplayDamage(other.gameObject));

				}

			}
		}

		IEnumerator DisplayDamage(GameObject other)
		{
            invincible = true;

            GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<Rigidbody2D>().velocity = new Vector2((transform.position.x - other.gameObject.GetComponentInParent<Player>().transform.position.x) * 25, GetComponent<Rigidbody2D>().velocity.y);

            yield return new WaitForSeconds(recoverTime);
            GetComponent<SpriteRenderer>().color = Color.white;
            invincible = false;

        }

	}
	
}
