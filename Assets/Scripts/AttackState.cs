using UnityEngine;
using static BattleController;

public class AttackState : BattleState
{
    private bool _hasAttackedThisTurn;
    
    public AttackState(BattleController battleController) : base(battleController)
    {
    }

    public override void OnEnterState()
    {
        _hasAttackedThisTurn = false;
    }
    
    public override void Update()
    {
        base.Update();

        CardController defender = null;
        
        if (hasAnyCardBeenReleasedThisFrame)
        {
            // Get references
            CardController[] attackerCards = battleController.CurrentDuelist.battlefieldController.CardSlots;
            CardController[] defenderCards = battleController.GetOpponentDuelist().battlefieldController.CardSlots;
            
            // Detect if an attacker is near a defender to perform the attack
            for (int a = 0; (!_hasAttackedThisTurn) && (a < attackerCards.Length); a++)
            {
                for (int d = 0; d < defenderCards.Length; d++)
                {
                    CardController attackerCard = attackerCards[a];
                    CardController defenderCard = defenderCards[d];
                    float sqrDistance = (defenderCard.transform.position - attackerCard.transform.position).sqrMagnitude;
                    const float SQR_DISTANCE_THRESHOLD = 0.25f;
                    if (sqrDistance < SQR_DISTANCE_THRESHOLD)
                    {
                        attackerCard.ExecuteAttack(defenderCard);

                        defender = defenderCard;
                        
                        _hasAttackedThisTurn = true;

                        break;
                    }
                }
            }
        }

        // Deal damage to the duelist if any card has died
        if (_hasAttackedThisTurn && (defender == null))
        {
            const int DAMAGE_PER_CARD_DEATH = 1;
            battleController.GetOpponentDuelist().TakeDamage(damage: DAMAGE_PER_CARD_DEATH);
        }
        
        hasAnyCardBeenReleasedThisFrame = false;
    }

    public override void OnExitState()
    {
        // Reset all attacker cards to their battlefield slot parents,
        // as if it's the AI playing, it wouldn't happen naturally
        BattlefieldController attackerBattlefield = battleController.CurrentDuelist.battlefieldController;
        for (int c = 0; c < attackerBattlefield.CardSlots.Length; c++)
        {
            CardController attackerCard = attackerBattlefield.CardSlots[c];
            if (attackerCard != null)
            {
                attackerCard.SetParent(attackerBattlefield.slots[c].transform, worldPositionStays: false);
            }
        }
    }
    
    public override BattleState GetNextState()
    {
        if (_hasAttackedThisTurn)
        {
            return battleController.PassTurnState;
        }
        return this;
    }

    public override string GetFriendlyName()
    {
        return "Modo de Ataque";
    }
}
