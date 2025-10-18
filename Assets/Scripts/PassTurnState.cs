public class PassTurnState : BattleState
{
    public PassTurnState(BattleController battleController) : base(battleController)
    {
    }

    public override void Update()
    {
        base.Update();

        battleController.SwapDuelists();
    }

    public override BattleState GetNextState()
    {
        return battleController.DrawCardsState;
    }
}