using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class HandController
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
            _cardSlots[c].DestroySelf();
        }
    }
    
    public CardController TakeCardFromSlot(int slotIndex)
    {
        CardController card = _cardSlots[slotIndex];
        _cardSlots[slotIndex] = null;
        return card;
    }
    
    public void GiveCardToSlot(CardController card, int slotIndex)
    {
        _cardSlots[slotIndex] = card;
        card.SetParent(slots[slotIndex], worldPositionStays: false);
    }
}
