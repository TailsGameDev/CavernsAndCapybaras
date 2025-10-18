using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;

public class BattleController : MonoBehaviour
{
    public class BattleState
    {
        protected BattleController battleController;

        public BattleState(BattleController battleController)
        {
            this.battleController = battleController;
        }

        public virtual void OnEnterState()
        {
        }
        public void OnExitState()
        {
        }
        public virtual void Update()
        {
        }
        public virtual BattleState GetNextState()
        {
            return this;
        }
    }

    public class InitialState : BattleState
    {
        public InitialState(BattleController battleController) : base(battleController)
        {
        }
        
        public override BattleState GetNextState()
        {
            return battleController._drawCardsState;
        }
    }
    
    
    
    public CardController cardPrefab;
    
    public DuelistController player;
    public DuelistController enemy;

    public DeckData playerTestDeck;
    public DeckData enemyTestDeck;

    private DuelistController _currentDuelist;
    private DuelistController _opponentDuelist;

    private BattleState _initialState;
    private DrawCardsState _drawCardsState;
    private BattleState _placeCardsState;
    private BattleState _repositionState;
    private BattleState _attackState;
    
    private BattleState _currentState;
    
    public DuelistController CurrentDuelist => _currentDuelist;
    public DuelistController OpponentDuelist => _opponentDuelist;
    
    public DrawCardsState DrawCardsStateState => _drawCardsState;
    
    private void Awake()
    {
        player.Initialize(cardPrefab, playerTestDeck);
        enemy.Initialize(cardPrefab, enemyTestDeck);
        
        _initialState = new BattleState(this);
        _initialState.OnEnterState();
        _currentState = _initialState;
    }

    private void Update()
    {
        // FSM Loop
        _currentState.Update();
        BattleState nextState = _currentState.GetNextState();
        if (nextState != _currentState)
        {
            _currentState.OnExitState();
            _currentState = nextState;
            nextState.OnEnterState();
        }
    }
}