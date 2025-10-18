using System;
using UnityEngine;

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
    
    public CardController TakeCardFromSlot(int slotIndex)
    {
        CardController card = _cardSlots[slotIndex];
        _cardSlots[slotIndex] = null;
        return card;
    }
}
