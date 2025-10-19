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
            if (battleController.CurrentState == battleController.PlaceCardsState)
            {
                DuelistController duelist = battleController.CurrentDuelist;
                BattlefieldController battlefield = duelist.battlefieldController;
                HandController hand = duelist.handController;
                CardController[] handCardSlots = hand.CardSlots;
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
            else if (battleController.CurrentState == battleController.AttackState)
            {
                // Get stronger AI attacker
                BattlefieldController aiBattlefield = battleController.enemyDuelist.battlefieldController;
                CardController[] aiCards = aiBattlefield.CardSlots;
                CardController strongerAICard = aiCards[0];
                for (int a = 0; a < aiCards.Length; a++)
                {
                    if (aiCards[a].GetAttack() > strongerAICard.GetAttack())
                    {
                        strongerAICard = aiCards[a];
                    }
                }
                
                // Get the stronger player defender to be the target
                BattlefieldController playerBattlefield = battleController.playerDuelist.battlefieldController;
                CardController[] playerCards = playerBattlefield.CardSlots;
                CardController strongerPlayerCard = playerCards[0];
                for (int a = 0; a < playerCards.Length; a++)
                {
                    if (playerCards[a].GetAttack() > strongerPlayerCard.GetAttack())
                    {
                        strongerPlayerCard = playerCards[a];
                    }
                }

                // Simulate card release on top of the target, as it's the AI dragging and dropping the cards
                // TODO: Protection
                strongerAICard.SetPosition(strongerPlayerCard.GetPosition());
                battleController.OnCardReleased(card: aiBattlefield.CardSlots[0]);
            }
        }
    }
}
