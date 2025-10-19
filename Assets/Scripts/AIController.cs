using UnityEngine;

public class AIController
{
    private class CardTween
    {
        private CardController _card;
        private bool _isTweening;
        private float _startTime;
        private float _timeToEnd;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private BattleController _battleController;

        public bool IsTweening => _isTweening;

        public CardTween(BattleController battleController)
        {
            _battleController = battleController;
        }
        
        public void StartTween(CardController card, Vector3 targetPosition, float duration)
        {
            _card = card;
            
            _startPosition = card.GetPosition();
            _targetPosition = targetPosition;

            _startTime = Time.time;
            _timeToEnd = Time.time + duration;
            
            card.SetParent(_battleController.movingCardsParent, worldPositionStays: true);
            
            _isTweening = true;
        }

        public void UpdateTween()
        {
            if (_card != null)
            {
                if (Time.time < _timeToEnd)
                {
                    // Calculate and set quadratic easing
                    float duration = _timeToEnd - _startTime;
                    float elapsed = Time.time - _startTime;
                    float progress = elapsed / duration;
                    float easedProgress = progress * progress;
                    Vector3 newPosition = Vector3.Lerp(_startPosition, _targetPosition, easedProgress);
                    _card.SetPosition(newPosition);
                }
                else
                {
                    _card.SetPosition(_targetPosition);
                    FinishTweenAndReleaseCard();
                }
            }
            else
            {
                _isTweening = false;
            }
        }

        private void FinishTweenAndReleaseCard()
        {
            _isTweening = false;

            // Simulate card release, as it's the AI dragging and dropping the cards
            _battleController.OnCardReleased(card: _card);
        }
    }
    
    private readonly BattleController _battleController;

    private readonly CardTween _cachedCardTween;

    public AIController(BattleController battleController)
    {
        _battleController = battleController;
        
        _cachedCardTween = new CardTween(battleController);
    }

    public void Update(float movementDuration)
    {
        if (_battleController.CurrentDuelist == _battleController.enemyDuelist)
        {
            if (_cachedCardTween.IsTweening)
            {
                _cachedCardTween.UpdateTween();
            }
            else if (_battleController.CurrentState == _battleController.PlaceCardsState)
            {
                // Position cards in battlefield places so PlaceCardState can execute properly

                DuelistController duelist = _battleController.CurrentDuelist;
                BattlefieldController battlefield = duelist.battlefieldController;
                HandController hand = duelist.handController;
                CardController[] handCardSlots = hand.CardSlots;
                for (int b = 0; b < battlefield.CardSlots.Length; b++)
                {
                    CardController battlefieldCardSlot = battlefield.CardSlots[b];
                    if (battlefieldCardSlot == null)
                    {
                        CardController cardInHand = handCardSlots[b];
                        if (cardInHand != null)
                        {
                            _cachedCardTween.StartTween(cardInHand, battlefield.slots[b].position, movementDuration);
                            
                            break;
                        }
                        else
                        {
                            Debug.LogError("[AIController] No card in hand slot " + b);
                        }
                    }
                }
            }
            else if (_battleController.CurrentState == _battleController.AttackState)
            {
                // Get stronger AI attacker
                BattlefieldController aiBattlefield = _battleController.enemyDuelist.battlefieldController;
                CardController[] aiCards = aiBattlefield.CardSlots;
                CardController strongerAICard = aiCards[0];
                for (int a = 0; a < aiCards.Length; a++)
                {
                    if (aiCards[a].GetAttack() > strongerAICard.GetAttack())
                    {
                        strongerAICard = aiCards[a];
                    }
                }
                
                // Get the stronger player defender to be the target
                BattlefieldController playerBattlefield = _battleController.playerDuelist.battlefieldController;
                CardController[] playerCards = playerBattlefield.CardSlots;
                CardController strongerPlayerCard = playerCards[0];
                for (int a = 0; a < playerCards.Length; a++)
                {
                    if (playerCards[a].GetAttack() > strongerPlayerCard.GetAttack())
                    {
                        strongerPlayerCard = playerCards[a];
                    }
                }

                // Start tween to move the attacker card smoothly towards its target, and restore the parent on complete
                // TODO: Protection
                _cachedCardTween.StartTween(strongerAICard, strongerPlayerCard.GetPosition(), movementDuration);
            }
        }
    }
}