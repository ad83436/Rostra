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

	// rain
	private bool isRaining = false;
	private int rainTurnsLeft = 0;
	private bool isWaitingForRain = false;
	private float defaultDefence;

	// turn counter
	private int turnCount = 0;

	// temp
	private bool boulder = false;

	protected override void Start() {
		base.Start();
		defaultDefence = eDefence;
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
		} else if (turnCount % 4 == 3) { // always heal every 4 turns

			Begin_Heal();
			Do_Heal();
			EndTurn();

			print("I AM HEALING!!");

		} else if (!isRaining) {
			// not raining
			// roll attack
			float roll = Random.Range(0f, 100f);
			//float roll = 50f;

			if (roll <= 10f) {
				// Earthquake
				Begin_Quake();
				Do_BaseQuake();
				if (boulder) Do_StrongQuake();
			} else if (roll <= 20f) {
				// Fire Pillar skill
				// TODO: Remove Do and end functions after adding animation
				Begin_FirePillar();
				Do_FirePillar();
				EndTurn();
			} else if (roll <= 30f) {
				// wind skill
				// TODO: Remove Do and end functions after adding animation
				Begin_Wind();
				Do_Wind();
				EndTurn();
			} else if (roll <= 40f) {
				// storm skill
				Charge_Storm();
				EndTurn();
			} else {
				// Dumb attack
				// TODO: Add animation and have animation call function
				DumbAttack();
				CompleteAttack();
			}
		} else {
			// is raining
			// roll attack
			float roll = Random.Range(0f, 100f);

			if (roll <= 0f) {
				// Dumb attack
				// TODO: Add animation and have animation call function
				DumbAttack();
				CompleteAttack();
			} else {
				print("Im stuff");
				// lightning skill
				// TODO: Remove Do and end functions after adding animation
				Begin_Lightning();
				Do_Lightning();
				EndTurn();
			}

			rainTurnsLeft--;
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

	#region SKILLS

	// do_skill will be called from animations

	////////////////////////////////////////////////////////////////////////////////////////////////
	// fire pillar skill

	#region 
	private void Begin_FirePillar() {
		// figure out what player to attack
		attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;

		// TODO: set pillar location and animation from here
	}

	private void Do_FirePillar() {
		// TODO: change to spawn pillar on player
		objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);

		attackThisPlayer.TakeDamage(eAttack * FirePillarDamageMultiplier);
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// lightning skill

	#region 
	private void Begin_Lightning() {
		// figure out what player to attack
		attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;

		// TODO: set lightning and animation from here
	}

	private void Do_Lightning() {
		// TODO: change to spawn lighting on player
		objPooler.SpawnFromPool("EnemyNormalAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);

		attackThisPlayer.TakeDamage(eAttack * LightningDamageMultiplier);
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Tornado/Hurricane skill

	#region 
	private void Begin_Wind() {
		// TODO: setup animation
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
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Storm skill

	#region 
	private void Charge_Storm() {
		currentState = EnemyState.waiting;
		waitQTurns = 2;
		waitTurnsText.gameObject.SetActive(true);
		waitTurnsText.text = waitQTurns.ToString();
		isWaitingForRain = true;
	}

	private void Begin_Storm() {
		// TODO: add animation

		isRaining = true;
		rainTurnsLeft = StormLength + Random.Range(-1, 2); // gives a amount that is +1, 0 or -1 of the normal storm length

		// set his defence
		eDefence = defaultDefence + DefenceIncrease;
	}

	private void End_Storm() {
		// TODO: disable animations
		isRaining = true;
		eDefence = defaultDefence;
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Heal skill

	#region 
	private void Begin_Heal() {
		// TODO: add animation
	}

	private void Do_Heal() {
		Heal(Random.Range(MinHealAmount, MaxHealAmount));
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////
	// Earthquake skill

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


	//
	// 'n0' = larger than both 'n1' and 'n2'
	// 'n1' = more than 'n2'
	// 'n2' = often enough to make it hard to kill Grendol. 
	//
	// completed skills:
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
