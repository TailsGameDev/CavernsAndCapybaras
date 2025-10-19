using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BattleController : ScreenController
{
    public CardController cardPrefab;

    [FormerlySerializedAs("enemy")] public DuelistController enemyDuelist;
    [FormerlySerializedAs("player")] public DuelistController playerDuelist;

    public TextMeshProUGUI hintText;
    
    public DeckData enemyTestDeck;
    public DeckData playerTestDeck;

    private DuelistController _currentDuelist;
    private DuelistController _opponentDuelist;

    private InitialState _initialState;
    private DrawCardsState _drawCardsState;
    private PlaceCardsState _placeCardsState;
    // private RepositionState _repositionState;
    private AttackState _attackState;
    private PassTurnState _passTurnState;
    private BattleState _finalState;

    private BattleState _currentState;
    
    private AIController _aiController;

    private CardController _cachedCardToResetPosition;
    private ScreenId _nextScreenId;

    public DuelistController CurrentDuelist => _currentDuelist;
    public DuelistController OpponentDuelist => _opponentDuelist;

    public DrawCardsState DrawCardsState => _drawCardsState;
    public PlaceCardsState PlaceCardsState => _placeCardsState;
    // public RepositionState RepositionState => _repositionState;
    public AttackState AttackState => _attackState;
    public PassTurnState PassTurnState => _passTurnState;
    
    public BattleState CurrentState => _currentState;
    public BattleState FinalState => _finalState;


    public override void ShowAsScreen()
    {
        base.ShowAsScreen();

        playerDuelist.Initialize(cardPrefab, playerTestDeck, OnCardReleased);
        enemyDuelist.Initialize(cardPrefab, enemyTestDeck, OnCardReleased);
        
        _aiController = new AIController(this);
        
        // Instantiate battle states
        _initialState = new InitialState(this);
        _drawCardsState = new DrawCardsState(this);
        _placeCardsState = new PlaceCardsState(this);
        // _repositionState = new RepositionState(this);
        _attackState = new AttackState(this);
        _passTurnState = new PassTurnState(this, OnBattleFinished);
        _finalState = new BattleState(this);
        
        _initialState.OnEnterState();
        _currentState = _initialState;

        _currentDuelist = enemyDuelist;
        
        _nextScreenId = ScreenId.BATTLE;
    }

    private void Update()
    {
        // FSM Loop
        _aiController.Update();
        
        _currentState.Update();
        BattleState nextState = _currentState.GetNextState();
        if (nextState != _currentState)
        {
            // Debug.Log("is player: "+(_currentDuelist == playerDuelist)
            //     + "; state transition "+_currentState.GetType().Name+" to: "+nextState.GetType().Name, this);
            _currentState.OnExitState();
            nextState.OnEnterState();
            hintText.text = nextState.GetType().Name+ "\n"
                + ((_currentDuelist == playerDuelist) ? "Your turn" : "Enemy's turn");
            _currentState = nextState;
        }

        if (_cachedCardToResetPosition != null)
        {
            _cachedCardToResetPosition.ResetPosition();
            _cachedCardToResetPosition = null;
        }
    }

    public void SwapDuelists()
    {
        _currentDuelist = (_currentDuelist == playerDuelist) ? enemyDuelist : playerDuelist;
    }
    
    // public bool IsPlayerTurn()
    // {
    //     return (_currentDuelist == playerDuelist);
    // }
    public DuelistController GetOpponentDuelist()
    {
        return (_currentDuelist == playerDuelist) ? enemyDuelist : playerDuelist;
    }

    public bool HasReleaseCardInput()
    {
        // Consider AI turn as automatic release card input as it places cards really fast
        return (_currentDuelist == playerDuelist) || Mouse.current.leftButton.wasReleasedThisFrame || (Touchscreen.current != null
            && Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Ended);
    }

    public void OnCardReleased(CardController card)
    {
        _currentState.hasAnyCardBeenReleasedThisFrame = true;
        _cachedCardToResetPosition = card;
    }

    private void OnBattleFinished(DuelistController winner)
    {
        _nextScreenId = (winner == playerDuelist) ? ScreenId.BATTLE_REWARDS : ScreenId.MAIN_MENU;
    }
    
    public override ScreenId GetNextScreenId()
    {
        return _nextScreenId;
    }
}