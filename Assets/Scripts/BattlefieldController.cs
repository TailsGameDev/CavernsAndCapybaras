using UnityEngine;
using System;
using Object = UnityEngine.Object;

[Serializable]
public class BattlefieldController
{
    public Transform root;
    public RectTransform[] slots;
    
    private CardController[] _cardSlots;
    
    public CardController[] CardSlots => _cardSlots;
    
    public void Initialize()
    {
        _cardSlots = new CardController[slots.Length];
    }
    public void Clear()
    {
        for (int c = _cardSlots.Length - 1; c >= 0; c--)
        {
            if (_cardSlots[c] != null)
            {
                Object.Destroy(_cardSlots[c].gameObject);
            }
        }
    }

    public void PlaceCardInSlot(CardController cardToPlace, int s)
    {
        _cardSlots[s] = cardToPlace;
        cardToPlace.SetParent(slots[s], worldPositionStays: false);
        cardToPlace.SetPosition(slots[s].position);
        cardToPlace.ShowHorizontalView();
    }

    public bool IsFull()
    {
        for (int i = 0; i < _cardSlots.Length; i++)
        {
            if (_cardSlots[i] == null)
            {
                return false;
            }
        }
        return true;
    }
}
