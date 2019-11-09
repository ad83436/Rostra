using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grendol : Enemy {
	public override void AddEnemyToBattle() {
		base.AddEnemyToBattle();
	}

	public override void EnemyTurn() {
		base.EnemyTurn();

		if (currentState != EnemyState.waiting) {

		}
	}

	protected override void Start() {
		base.Start();
	}

	protected override void Update() {
		base.Update();
	}
	
	// Im going to outline the possible skills here
	//
	// After a certain number of turns of intense battle Grendol will become weaker and wont be able to heal himself.
	// It wont be impossible to kill Grendol early but it should be difficult.
	//
	// 'n0' = larger than both 'n1' and 'n2'
	// 'n1' = more than 'n2'
	// 'n2' = often enough to make it hard to kill Grendol. 
	//
	// Storm:
	//		- Creates a storm after waiting 1 turn. Increases defence.
	//			Only usable after 'n1' turns. And lasts a couple turns.
	//
	// Heal:
	//		- Will always use this every 'n2' number of turns. Instantly heals a large portion of health
	//			Wont be able to use this skill after 'n0' number of turns
	//
	// Fire Pillar:
	//		- Hits one player with a fire pillar/ or fire something and deals a lot of damage but is used rarely.
	//			Cant be used during a storm.
	//
	// Lightning:
	//		- Same as fire pillar but is used during a storm and deals more damage. Used more often.
	//
	// Ice stites:
	//		- Shoots two ice stalagtites that hit two players and have a chance of reducing thier defence
	//
	// Tornado/Hurricane:
	//		- A tornado will hit all players, doing damage to all of them.
	//
	// Earthquake: 
	//		- All players are damaged slightly and a there's chance of a boulder falling on top
}
