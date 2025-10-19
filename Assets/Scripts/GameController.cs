using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public ScreenController[] screenControllers;
    
    private ScreenController _currentScreenController;
    private List<ScreenController> _popupControllers;
    
    private void Awake()
    {
        _popupControllers = new List<ScreenController>();
        
        foreach (var screenController in screenControllers)
        {
            screenController.Initialize(this);
        }
        
        _currentScreenController = screenControllers[0];
        _currentScreenController.ShowAsScreen();
    }

    private void Update()
    {
        ScreenId nextScreenId = _currentScreenController.GetNextScreenId();
        if (nextScreenId != _currentScreenController.screenId)
        {
            _currentScreenController.Close();
            ScreenController nextScreenController = GetScreenControllerById(nextScreenId);
            nextScreenController.ShowAsScreen();
            _currentScreenController = nextScreenController;
        }
    }
    
    private ScreenController GetScreenControllerById(ScreenId screenId)
    {
        foreach (var screenController in screenControllers)
        {
            if (screenController.screenId == screenId)
            {
                return screenController;
            }
        }

        Debug.LogError("[GameController] ScreenController not found for ScreenId: " + screenId);
        return null;
    }
}
