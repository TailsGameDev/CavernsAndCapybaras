using System;

public class GameOverScreen : ScreenController
{
    private bool _hasClickedGoToMenuButton;

    public override void ShowAsScreen()
    {
        base.ShowAsScreen();

        _hasClickedGoToMenuButton = false;
    }
    
    public void OnGoToMenuButtonClick()
    {
        _hasClickedGoToMenuButton = true;
    }

    public override ScreenId GetNextScreenId()
    {
        return _hasClickedGoToMenuButton ? ScreenId.MAIN_MENU : screenId;
    }
}