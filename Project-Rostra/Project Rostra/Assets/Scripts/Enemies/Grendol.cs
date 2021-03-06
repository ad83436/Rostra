using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grendol : Enemy {

	[Header("Grendol Vars")]
	public float FirePillarDamageMultiplier;
	public float LightningDamageMultiplier;
	public float WindDamageMultiplier;
	public int StormLength;
	public float DefenceIncrease;
	public float MinHealAmount;
	public float MaxHealAmount;
	public float QuakeNormalMultiplier;
	public float QuakeStrongMultiplier;
	public int MaxNumberOfHeals;
	public float PercentChanceIncreaseOfStorm;
	public float BasePercentChanceIncreaseOfStorm;
	public Transform FirePillarTransform;

	// rain
	private bool isRaining = false;
	private int rainTurnsLeft = 0;
	private bool isWaitingForRain = false;
	private float defaultDefence;
	private float percentChance = 0f;

	// turn counter
	private int turnCount = 0;

	// temp
	private bool boulder = false;

	protected override void Start() {
		base.Start();
		defaultDefence = eDefence;
		percentChance = BasePercentChanceIncreaseOfStorm;
	}

	protected override void Update() {
		base.Update();
	}

	public override void EnemyTurn() {
		uiBTL.backdropHighlighter.gameObject.SetActive(true);
		uiBTL.DisableActivtyText();
		CheckForAilments();
		CheckForBuffs();

		if (currentState == EnemyState.waiting) {
			waitQTurns--;
			waitTurnsText.text = waitQTurns.ToString(); //Update the UI
			if (waitQTurns <= 0) {
				waitTurnsText.gameObject.SetActive(false); //Turn off the text. Don't forget to enable it when the enemy goes to waiting state
				currentState = EnemyState.idle; //Change the state

				if (isWaitingForRain) {
					Begin_Storm();
				}

				EndTurn();
			} else {
				//End the turn
				EndTurn();
			}
		} else if (MaxNumberOfHeals != 0 && turnCount % 4 == 3) { // always heal every 4 turns and while still under 10 heals

			Begin_Heal();

		} else if (!isRaining) {
			// not raining
			// roll attack
			float roll = Random.Range(0f, 100f);
			percentChance += PercentChanceIncreaseOfStorm;

			if (roll <= 40f) {
				// Fire Pillar skill
				Begin_FirePillar();
			} else if (roll <= 60f) {
				// wind skill
				Begin_Wind();
			} else if (roll <= 60f + percentChance && MaxNumberOfHeals != 0 ? (turnCount % 4) < 2 : true) {
				// storm skill
				Charge_Storm();
			} else {
				// Dumb attack
				DumbAttack();
			}
		} else {
			// is raining
			// roll attack
			float roll = Random.Range(0f, 100f);
			rainTurnsLeft--;

			if (rainTurnsLeft == 0) {
				Begin_Wind();
			} else if (roll <= 50f) {
				// Dumb attack
				DumbAttack();
			} else if (roll <= 75f) {
				// wind skill
				Begin_Wind();
			} else {
				// lightning skill
				Begin_Lightning();
			}

			if (rainTurnsLeft == 0) {
				End_Storm();
			}

		}

		turnCount++;
	}

	// this can be called from the animation
	private void EndTurn() {
		uiBTL.EndTurn();
	}

	private void AttackSound() {
		audioManager.PlayThisEffect("GAttack");
	}

	#region SKILLS

	// do_skill will be called from animations

	////////////////////////////////////////////////////////////////////////////////////////////////
	// fire pillar skill

	#region 
	private void Begin_FirePillar() {
		// figure out what player to attack
		attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;

		// set the fire animation
		animator.SetTrigger("Fire");

	}

	private void Do_FirePillar() {
		//objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);

		attackThisPlayer.TakeDamage(eAttack * FirePillarDamageMultiplier * 0.5f); // multiply by 0.5 because the attack hits twice

		FirePillarTransform.position = attackThisPlayer.transform.position + Vector3.down * 2f; // set to player position but with an offset
		animator.SetTrigger("Pillar");

		// play sound
		audioManager.PlayThisEffect("GFire");

		uiBTL.UpdateActivityText("HellStorm");
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// lightning skill

	#region 
	private void Begin_Lightning() {
		// figure out what player to attack
		attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;

		animator.SetTrigger("Lightning");
		// TODO: set lightning and animation from here
	}

	private void Do_Lightning() {
		objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);

		attackThisPlayer.TakeDamage(eAttack * LightningDamageMultiplier);

		uiBTL.UpdateActivityText("Lightning Strike");
	}

	private void PlaySound_Lightning() {
		audioManager.PlayThisEffect("GLightning");
	}

	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Tornado/Hurricane skill

	#region 

	private void Begin_Wind() {
		animator.SetTrigger("Wind");
		audioManager.PlayThisEffect("GWind");

	}

	private void Do_Wind() {
		// attack player 0
		attackThisPlayer = battleManager.players[0].playerReference;
		objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
		attackThisPlayer.TakeDamage(eAttack * WindDamageMultiplier);

		// attack player 1
		attackThisPlayer = battleManager.players[1].playerReference;
		objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
		attackThisPlayer.TakeDamage(eAttack * WindDamageMultiplier);

		// attack player 2
		attackThisPlayer = battleManager.players[2].playerReference;
		objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
		attackThisPlayer.TakeDamage(eAttack * WindDamageMultiplier);

		// attack player 3
		attackThisPlayer = battleManager.players[3].playerReference;
		objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
		attackThisPlayer.TakeDamage(eAttack * WindDamageMultiplier);

		uiBTL.UpdateActivityText("Hurricane");
	}

	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Storm skill

	#region 
	private void Charge_Storm() {
		percentChance = BasePercentChanceIncreaseOfStorm;

		animator.SetTrigger("Storm");
		animator.SetBool("IsWaiting", true);

		currentState = EnemyState.waiting;
		waitQTurns = 1;
		waitTurnsText.gameObject.SetActive(true);
		waitTurnsText.text = waitQTurns.ToString();
		isWaitingForRain = true;

		EndTurn();
	}

	private void ShowStormText() {
		uiBTL.UpdateActivityText("SuperStorm");
	}

	private void Begin_Storm() {
		animator.SetBool("IsWaiting", false);
		animator.SetBool("ShowClouds", true);

		isRaining = true;
		rainTurnsLeft = StormLength + Random.Range(-1, 2); // gives a amount that is +1, 0 or -1 of the normal storm length

		// set his defence
		eDefence = defaultDefence + DefenceIncrease;

	}

	private void End_Storm() {
		// TODO: disable animations
		isRaining = true;
		eDefence = defaultDefence;
		animator.SetBool("ShowClouds", false);
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Heal skill

	#region 
	private void Begin_Heal() {
		animator.SetTrigger("Heal");
		MaxNumberOfHeals--;
		audioManager.PlayThisEffect("GHeal");
	}

	private void Do_Heal() {
		Heal(Random.Range(MinHealAmount, MaxHealAmount));
		healthObject.SetActive(true);
		uiBTL.UpdateActivityText("Heal");
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Earthquake skill ( UNUSED )

	#region 
	private void Begin_Quake() {
		// TODO: play different animations according to the option
		// figure out which player gets hit by the boulder
		if (Random.Range(0f, 100f) < 20f) {
			attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;
			boulder = true; // TODO: remove afterwords
		} else {
			boulder = false; // TODO: remove afterwords
			attackThisPlayer = null;
		}
	}

	private void Do_BaseQuake() {
		// attack player 0
		objPooler.SpawnFromPool("EnemyNormalAttack",
			battleManager.players[0].playerReference.gameObject.transform.position, gameObject.transform.rotation);
		battleManager.players[0].playerReference.TakeDamage(eAttack * QuakeNormalMultiplier);

		// attack player 1
		objPooler.SpawnFromPool("EnemyNormalAttack",
			battleManager.players[1].playerReference.gameObject.transform.position, gameObject.transform.rotation);
		attackThisPlayer.TakeDamage(eAttack * QuakeNormalMultiplier);

		// attack player 2
		objPooler.SpawnFromPool("EnemyNormalAttack",
			battleManager.players[2].playerReference.gameObject.transform.position, gameObject.transform.rotation);
		battleManager.players[2].playerReference.TakeDamage(eAttack * QuakeNormalMultiplier);

		// attack player 3
		objPooler.SpawnFromPool("EnemyNormalAttack",
			battleManager.players[3].playerReference.gameObject.transform.position, gameObject.transform.rotation);
		battleManager.players[3].playerReference.TakeDamage(eAttack * QuakeNormalMultiplier);
	}

	private void Do_StrongQuake() {
		// strong attack
		objPooler.SpawnFromPool("EnemyNormalAttack",
			battleManager.players[2].playerReference.gameObject.transform.position, gameObject.transform.rotation);
		battleManager.players[2].playerReference.TakeDamage(eAttack * QuakeStrongMultiplier);
	}
	#endregion

	#endregion

	// Im going to outline the possible skills here
	//
	// After a certain number of turns of intense battle Grendol will become weaker and wont be able to heal himself.
	// It wont be impossible to kill Grendol early but it should be difficult.
	// Ice stites: 
	//		- Shoots two ice stalagtites that hit two players and have a chance of reducing thier defence
	//
	// Earthquake: 
	//		- All players are damaged slightly and a there's chance of a boulder falling on top


	// completed skills:
	//
	// Storm:
	//		- Creates a storm after waiting 1 turn. Increases defence.
	//			Only usable after 'n1' turns. And lasts a couple turns.
	// Tornado/Hurricane: ++ got it
	//		- A tornado will hit all players, doing damage to all of them.
	// Lightning: ++ got it
	//		- Same as fire pillar but is used during a storm and deals more damage. Used more often.
	// Heal:
	//		- Will always use this every 'n2' number of turns. Instantly heals a large portion of health
	//			Wont be able to use this skill after 'n0' number of turns
	// Fire Pillar: ++ got it
	//		- Hits one player with a fire pillar/something and deals a lot of damage but is used rarely.
	//			Cant be used during a storm.
	//
}
