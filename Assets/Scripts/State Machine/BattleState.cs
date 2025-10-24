using UnityEngine;

public class BattleState
{
    public bool hasAnyCardBeenReleasedThisFrame;
    protected BattleController battleController;

    public BattleState(BattleController battleController)
    {
        this.battleController = battleController;
    }

    public virtual void OnEnterState()
    {
    }
    public virtual void OnExitState()
    {
    }
    public virtual void Update()
    {
    }
    public virtual BattleState GetNextState()
    {
        return this;
    }

    public virtual string GetFriendlyName()
    {
        return this.GetType().Name;
    }
}