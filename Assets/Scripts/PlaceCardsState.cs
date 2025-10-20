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

        // Get references
        DuelistController duelist = battleController.CurrentDuelist;
        DeckController deck = duelist.deckController;
        HandController hand = duelist.handController;
        BattlefieldController battlefield = duelist.battlefieldController;
        CardController[] handCardSlots = duelist.handController.CardSlots;
        
        if (hasAnyCardBeenReleasedThisFrame)
        {
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

        // Resolve wave movement of hand cards
        {
            // Check if any card is being held
            bool hasHeldCard = false;
            for (int c = 0; c < handCardSlots.Length; c++)
            {
                CardController card = handCardSlots[c];
                bool isCardCompletellyInSlot = card.transform.parent == hand.slots[c];
                if ( ! isCardCompletellyInSlot )
                {
                    hasHeldCard = true;
                    break;
                }
            }

            if (hasHeldCard)
            {
                // Reset position of cards that are not being held
                for (int c = 0; c < handCardSlots.Length; c++)
                {
                    CardController card = handCardSlots[c];
                    bool isCardCompletellyInSlot = card.transform.parent == hand.slots[c];
                    if (isCardCompletellyInSlot)
                    {
                        card.ResetPosition();
                    }
                }
            }
            else
            {
                // Make cards do a smooth wave movement
                for (int c = 0; c < handCardSlots.Length; c++)
                {
                    // 90 degrees offset per card: 0, 90, 180, 270
                    float radians = (Mathf.PI / 2f) * c;
                        
                    // Move the card's transform up/down
                    const float AMPLITUDE = 10.0f;
                    const float SPEED = 3.0f;
                    float yOffset = Mathf.Sin(Time.time * SPEED + radians) * AMPLITUDE;
                    handCardSlots[c].transform.localPosition = new Vector3(0.0f, yOffset, 0.0f);
                }
            }
        }
        
        hasAnyCardBeenReleasedThisFrame = false;
    }

    public override void OnExitState()
    {
        _hasPastFirstTurn = true;

        // Reset all hand card positions
        CardController[] handCardSlots = battleController.CurrentDuelist.handController.CardSlots;
        for (int c = 0; c < handCardSlots.Length; c++)
        {
            handCardSlots[c].ResetPosition();
        }
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
