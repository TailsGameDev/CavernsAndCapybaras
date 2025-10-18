public class DrawCardsState : BattleState
{
    public DrawCardsState(BattleController battleController) : base(battleController)
    {
    }

    public override void Update()
    {
        base.Update();

        DuelistController duelist = battleController.CurrentDuelist;
        DeckController deck = duelist.deckController;
        HandController hand = duelist.handController;
        CardController[] cardSlots = hand.CardSlots;
        
        // Draw card from deck to hand
        for (int c = 0; c < cardSlots.Length; c++)
        {
            if (cardSlots[c] == null)
            {
                hand.GiveCardToSlot(card: deck.DrawCard(), slotIndex: c);
            }
        }
    }
    
    public override BattleState GetNextState()
    {
        return battleController.PlaceCardsState;
    }
}
