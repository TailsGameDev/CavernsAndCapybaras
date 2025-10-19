using UnityEngine;

public class MainMenuScreen : ScreenController
{
    private ScreenId _nextScreenId;

    public override void ShowAsScreen()
    {
        base.ShowAsScreen();
        
        _nextScreenId = ScreenId.MAIN_MENU;
    }

    public void OnPlayButtonClick()
    {
        _nextScreenId = ScreenId.BATTLE;
    }
    
    public override ScreenId GetNextScreenId()
    {
        return _nextScreenId;
    }
}
