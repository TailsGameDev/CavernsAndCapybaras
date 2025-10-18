using UnityEngine;

public class AIController
{
    private BattleController battleController;

    public AIController(BattleController battleController)
    {
        this.battleController = battleController;
    }

    public void Update()
    {
        if (battleController.CurrentDuelist == battleController.enemyDuelist)
        {
            // Position cards in battlefield places so PlaceCardState can execute properly
            
            DuelistController duelist = battleController.CurrentDuelist;
            BattlefieldController battlefield = duelist.battlefieldController;
            HandController hand = duelist.handController;
            CardController[] handCardSlots = hand.CardSlots;
            
            if (battleController.CurrentState == battleController.PlaceCardsState)
            {
                for (int b = 0; b < battlefield.CardSlots.Length; b++)
                {
                    CardController battlefieldCardSlot = battlefield.CardSlots[b];
                    if (battlefieldCardSlot == null)
                    {
                        CardController cardInHand = handCardSlots[b];
                        if (cardInHand != null)
                        {
                            cardInHand.SetPosition(battlefield.slots[b].position);
                            
                            // Simulate card release, as it's the AI dragging and dropping the cards
                            battleController.OnCardReleased(card: battlefield.CardSlots[0]);
                        }
                        else
                        {
                            Debug.LogError("[AIController] No card in hand slot " + b);
                        }
                    }
                }
            }
        }
    }
}
