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
    public Transform movingCardsParent;
    
    public DeckData enemyTestDeck;
    public DeckData playerTestDeck;

    public float aiMovementDuration;

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

        playerDuelist.Initialize(cardPrefab, playerTestDeck, movingCardsParent, OnCardReleased);
        enemyDuelist.Initialize(cardPrefab, enemyTestDeck, movingCardsParent, OnCardReleased);
        
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
        
        _nextScreenId = screenId;
    }

    public override void Close()
    {
        base.Close();
        
        playerDuelist.Clear();
        enemyDuelist.Clear();
    }

    private void Update()
    {
        // FSM Loop
        _aiController.Update(aiMovementDuration);
        
        _currentState.Update();
        BattleState nextState = _currentState.GetNextState();
        if (nextState != _currentState)
        {
            // Debug.Log("is player: "+(_currentDuelist == playerDuelist)
            //     + "; state transition "+_currentState.GetType().Name+" to: "+nextState.GetType().Name, this);
            _currentState.OnExitState();
            nextState.OnEnterState();
            hintText.text = nextState.GetFriendlyName()+ "\n"
                + ((_currentDuelist == playerDuelist) ? "Sua vez" : "Vez do inimigo");
            _currentState = nextState;
        }

        // Reset the released cards position by default
        if (_cachedCardToResetPosition != null)
        {
            _cachedCardToResetPosition.ResetPosition();
            _cachedCardToResetPosition = null;
        }
        
        // Update duelists as they have internal tweens
        playerDuelist.Update();
        enemyDuelist.Update();
    }

    public void SwapDuelists()
    {
        _currentDuelist = (_currentDuelist == playerDuelist) ? enemyDuelist : playerDuelist;
    }

    public DuelistController GetOpponentDuelist()
    {
        return (_currentDuelist == playerDuelist) ? enemyDuelist : playerDuelist;
    }

    public void OnCardReleased(CardController card)
    {
        _currentState.hasAnyCardBeenReleasedThisFrame = true;
        _cachedCardToResetPosition = card;
    }

    private void OnBattleFinished(DuelistController winner)
    {
        _nextScreenId = (winner == playerDuelist) ? ScreenId.BATTLE_REWARDS : ScreenId.GAME_OVER;
    }
    
    public override ScreenId GetNextScreenId()
    {
        return _nextScreenId;
    }

    public bool IsPlayerTurn()
    {
        return (_currentDuelist == playerDuelist);
    }
}