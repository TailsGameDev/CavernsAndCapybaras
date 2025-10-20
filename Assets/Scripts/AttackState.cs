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
        
        // Resolve wave movement of hand cards
        if (battleController.IsPlayerTurn())
        {
            CardController[] attackerCards = battleController.CurrentDuelist.battlefieldController.CardSlots;
            // Check if any card is being held
            bool hasHeldCard = false;
            for (int c = 0; c < attackerCards.Length; c++)
            {
                CardController card = attackerCards[c];
                bool isCardCompletellyInSlot = card.transform.parent == battleController.CurrentDuelist.battlefieldController.slots[c];
                if ( ! isCardCompletellyInSlot )
                {
                    hasHeldCard = true;
                    break;
                }
            }

            // Detach the enemy battlefield if any card is being held
            battleController.enemyDuelist.battlefieldController.SetPlayerBattlefieldHighlight(hasHeldCard);
            
            CardController[] playerBattlefieldCards = battleController.playerDuelist.battlefieldController.CardSlots;
            if (hasHeldCard)
            {
                // Reset position of cards that are not being held
                for (int c = 0; c < playerBattlefieldCards.Length; c++)
                {
                    CardController card = playerBattlefieldCards[c];
                    bool isCardCompletellyInSlot = card.transform.parent == battleController.playerDuelist.battlefieldController.slots[c];
                    if (isCardCompletellyInSlot)
                    {
                        card.ResetPosition();
                    }
                }
            }
            else
            {
                // Make cards do a smooth wave movement
                for (int c = 0; c < playerBattlefieldCards.Length; c++)
                {
                    // 90 degrees offset per card: 0, 90, 180, 270
                    float radians = (Mathf.PI / 2f) * c;
                        
                    // Move the card's transform up/down
                    const float AMPLITUDE = 10.0f;
                    const float SPEED = 3.0f;
                    float yOffset = Mathf.Sin(Time.time * SPEED + radians) * AMPLITUDE;
                    playerBattlefieldCards[c].transform.localPosition = new Vector3(0.0f, yOffset, 0.0f);
                }
            }
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
        
        // Reset all alive player battlefield card positions
        CardController[] playerBattlefieldCards = battleController.playerDuelist.battlefieldController.CardSlots;
        for (int c = 0; c < playerBattlefieldCards.Length; c++)
        {
            CardController card = playerBattlefieldCards[c];
            if (card != null)
            {
                card.ResetPosition();
            }
        }
        
        // Reset battlefield highlight
        battleController.enemyDuelist.battlefieldController.SetPlayerBattlefieldHighlight(highlight: false);
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
