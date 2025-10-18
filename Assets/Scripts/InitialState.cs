using UnityEngine;

public class InitialState : BattleState
{
    public InitialState(BattleController battleController) : base(battleController)
    {
    }
        
    public override BattleState GetNextState()
    {
        return battleController.DrawCardsState;
    }
}