using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum SkillId
{
    NONE = 0,
    
    PIERCE = 1,
    BULKY = 2,
    STEAL = 3,
    SNEAKY = 4,
    BRUTAL = 5,
    BLESS = 6,
}

public enum CardId
{
    NONE = 0,
    
    CARD_01 = 1,
    CARD_02 = 2,
    CARD_03 = 3,
    CARD_04 = 4,
    CARD_05 = 5,
    CARD_06 = 6,
    CARD_07 = 7,
    CARD_08 = 8,
    CARD_09 = 9,
    CARD_10 = 10,
    CARD_11 = 11,
    CARD_12 = 12,
    CARD_13 = 13,
}

[System.Serializable]
public class CardData
{
    public int attack;
    public int vitality;
    public SkillId skillId;
}

public class CardController : MonoBehaviour
{
    public CardView cardView;
    private bool isDragging;
    private Vector2 touchPosition;

    private Action<CardController> _onCardReleased;

    public void Initialize(CardId cardId, Action<CardController> onCardReleased)
    {
        _onCardReleased = onCardReleased;
    }
    
    private void Awake()
    {
        cardView.root = transform;
    }

    private void Update()
    {
        if (isDragging)
        {
            cardView.UpdateDrag();
        }
    }

    public void OnPointerDown()
    {
        cardView.OnPointerDown();
        isDragging = true;
    }

    public void OnPointerUp()
    {
        cardView.OnPointerUp();
        isDragging = false;
        _onCardReleased?.Invoke(this);
    }

    // TODO: Delegate to CardView
    public void SetParent(Transform parent, bool worldPositionStays)
    {
        cardView.root.SetParent(parent, worldPositionStays);
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void ShowHorizontalView()
    {
        cardView.ShowHorizontalView();
    }
}

[System.Serializable]
public class CardView
{
    [System.Serializable]
    public class CardLayout
    {
        public GameObject root;
        public Image characterArt;
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI vitalityText;
        public TextMeshProUGUI skillText;
    }

    public Transform root;
    [FormerlySerializedAs("verticalLayout")] public CardLayout verticalView;
    [FormerlySerializedAs("horizontalLayout")] public CardLayout horizontalView;


    private bool isDragging;
    private Vector2 touchPosition;
    private Vector2 cachedPosition;
    private Vector3 originalPosition;
    
    public void OnPointerDown()
    {
        isDragging = true;

        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        cachedPosition = root.position;
        originalPosition = root.position;
    }
    public void OnPointerUp()
    {
        isDragging = false;
    }
    public void UpdateDrag()
    {
        // Convert screen positions to Canvas space
        Vector2 newTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform, newTouchPosition, Camera.main,
            out Vector2 currentLocalPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform, touchPosition, Camera.main,
            out Vector2 previousLocalPosition);

        // Calculate delta in canvas space
        Vector2 positionDelta = currentLocalPosition - previousLocalPosition;
            
        // Update position
        touchPosition = newTouchPosition;
        cachedPosition += positionDelta;
        root.localPosition = cachedPosition;
    }
    
    public void ShowHorizontalView()
    {
        verticalView.root.gameObject.SetActive(false);
        horizontalView.root.gameObject.SetActive(true);
    }
}