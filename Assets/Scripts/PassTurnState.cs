using System;

public class PassTurnState : BattleState
{
    private DuelistController _cachedWinner;
    private readonly Action<DuelistController> _onBattleFinished;
    
    public PassTurnState(BattleController battleController, Action<DuelistController> onBattleFinished) : base(battleController)
    {
        _onBattleFinished = onBattleFinished;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();

        _cachedWinner = null;
    }
    
    public override void Update()
    {
        base.Update();
        
        if (battleController.enemyDuelist.Vitality <= 0)
        {
            _cachedWinner = battleController.playerDuelist;
        }
        else if (battleController.playerDuelist.Vitality <= 0)
        {
            _cachedWinner = battleController.enemyDuelist;
        }

        if (_cachedWinner != null)
        {
            _onBattleFinished?.Invoke(_cachedWinner);
        }
        else
        {
            battleController.SwapDuelists();
        }
    }

    public override BattleState GetNextState()
    {
        return (_cachedWinner == null) ? battleController.DrawCardsState : battleController.FinalState;
    }
}