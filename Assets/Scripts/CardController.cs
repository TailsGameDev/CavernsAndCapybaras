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

    private CardData _currentCardData;
    
    private Action<CardController> _onCardReleased;

    public void Initialize(CardId cardId, Action<CardController> onCardReleased)
    {
        _onCardReleased = onCardReleased;
        
        // TODO: Load card data from a database or scriptable object
        _currentCardData = new CardData();
        _currentCardData.attack = 5;
        _currentCardData.vitality = 10;
        _currentCardData.skillId = SkillId.NONE;
        
        cardView.RefreshCardView(_currentCardData);
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
    
    public void ExecuteAttack(CardController defender)
    {
        defender._currentCardData.vitality -= _currentCardData.attack;
        if (defender._currentCardData.vitality <= 0)
        {
            Destroy(defender.gameObject);
        }
        cardView.ResetPosition();
    }

    public void ResetPosition()
    {
        cardView.ResetPosition();
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
        
        public void SetAttack(int attack)
        {
            attackText.text = attack.ToString();
        }
        public void SetVitality(int vitality)
        {
            vitalityText.text = vitality.ToString();
        }
        public void SetSkill(SkillId skillId)
        {
            skillText.text = skillId.ToString();
        }
    }

    public Transform root;
    [FormerlySerializedAs("verticalLayout")] public CardLayout verticalView;
    [FormerlySerializedAs("horizontalLayout")] public CardLayout horizontalView;


    private bool _isDragging;
    private Vector2 _touchPosition;
    private Vector2 _cachedPosition;

    public void Initialize()
    {
        // TODO: Load initial data into views
    }

    public void RefreshCardView(CardData cardData)
    {
        verticalView.SetAttack(cardData.attack);
        verticalView.SetVitality(cardData.vitality);
        horizontalView.SetAttack(cardData.attack);
        horizontalView.SetVitality(cardData.vitality);
        verticalView.SetSkill(cardData.skillId);
    }
    
    public void OnPointerDown()
    {
        _isDragging = true;

        _touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        _cachedPosition = root.position;
    }
    public void OnPointerUp()
    {
        _isDragging = false;
    }
    public void UpdateDrag()
    {
        // Convert screen positions to Canvas space
        Vector2 newTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform, newTouchPosition, Camera.main,
            out Vector2 currentLocalPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform, _touchPosition, Camera.main,
            out Vector2 previousLocalPosition);

        // Calculate delta in canvas space
        Vector2 positionDelta = currentLocalPosition - previousLocalPosition;
            
        // Update position
        _touchPosition = newTouchPosition;
        _cachedPosition += positionDelta;
        root.localPosition = _cachedPosition;
    }
    
    public void ShowHorizontalView()
    {
        verticalView.root.gameObject.SetActive(false);
        horizontalView.root.gameObject.SetActive(true);
    }

    public void ResetPosition()
    {
        root.localPosition = Vector3.zero;
    }
}