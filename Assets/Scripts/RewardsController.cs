using System;
using UnityEngine;

public class RewardsController : ScreenController
{
    private ScreenId _nextScreenId;

    public override void ShowAsScreen()
    {
        base.ShowAsScreen();
        
        _nextScreenId = screenId;
    }
    
    public void OnNextBattleButtonClick()
    {
        _nextScreenId = ScreenId.BATTLE;
    }
    
    public override ScreenId GetNextScreenId()
    {
        return _nextScreenId;
    }
}
