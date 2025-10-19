using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceCardsState : BattleState
{
    private bool _hasPastFirstTurn;
    
    public PlaceCardsState(BattleController battleController) : base(battleController)
    {
    }

    public override void Update()
    {
        base.Update();

        if (hasAnyCardBeenReleasedThisFrame)
        {
            // Get references
            DuelistController duelist = battleController.CurrentDuelist;
            DeckController deck = duelist.deckController;
            HandController hand = duelist.handController;
            BattlefieldController battlefield = duelist.battlefieldController;
            CardController[] handCardSlots = hand.CardSlots;

            // Detect if a card is near a slot to place it there
            for (int h = 0; h < handCardSlots.Length; h++)
            {
                for (int b = 0; b < battlefield.CardSlots.Length; b++)
                {
                    CardController battlefieldCardSlot = battlefield.CardSlots[b];
                    if (battlefieldCardSlot == null)
                    {
                        CardController cardInHand = handCardSlots[h];
                        if (cardInHand != null)
                        {
                            float sqrDistance = (cardInHand.transform.position - battlefield.slots[b].position).sqrMagnitude;
                            const float SQR_DISTANCE_THRESHOLD = 0.25f;
                            if (sqrDistance < SQR_DISTANCE_THRESHOLD)
                            {
                                // Take the card from hand and place it on the battlefield
                                // Also refill the hand with a new drawn card from the deck
                                battlefield.PlaceCardInSlot(hand.TakeCardFromSlot(h), b);
                                hand.GiveCardToSlot(card: deck.DrawCard(), slotIndex: h);
                            }
                        }
                    }
                }
            }
        }
        
        hasAnyCardBeenReleasedThisFrame = false;
    }

    public override void OnExitState()
    {
        _hasPastFirstTurn = true;
    }
    
    public override BattleState GetNextState()
    {
        bool isBattlefieldFull = battleController.CurrentDuelist.battlefieldController.IsFull();
        if (isBattlefieldFull)
        {
            if (_hasPastFirstTurn)
            {
                // Pass state when the battlefield is full
                return battleController.AttackState;
            }
            else
            {
                // As it's the first turn, whenever cards are placed, the enemy should pass the turn
                return battleController.PassTurnState;
            }
        }
        else
        {
            return this;
        }
    }

    public override string GetFriendlyName()
    {
        return "Posicione as cartas";
    }
}
