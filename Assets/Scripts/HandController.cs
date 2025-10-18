using System;
using UnityEngine;

[Serializable]
public class HandController
{
    public Transform root;
    public RectTransform[] slots;
    
    private CardController[] _cards;
    
    public CardController[] Cards => _cards;
}
