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
            screenController.Initialize();
        }
        
        _currentScreenController = screenControllers[0];
        _currentScreenController.ShowAsScreen();
    }
}
