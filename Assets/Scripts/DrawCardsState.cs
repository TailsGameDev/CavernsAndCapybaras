using UnityEngine;

public class DrawCardsState : BattleController.BattleState
{
    public DrawCardsState(BattleController battleController) : base(battleController)
    {
    }

    public override void Update()
    {
        base.Update();

        DuelistController duelist = battleController.CurrentDuelist;
        DeckController deckController = duelist.deckController;

        for (int h = 0; h < 5; h++)
        {
            // Draw card from deck to hand
        }
    }

    public override BattleController.BattleState GetNextState()
    {
        return battleController.DrawCardsStateState;
    }
}
