using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Object = System.Object;

public class CardController : MonoBehaviour
{
    public CardsDatabase cardsDatabase;
    public CardView cardView;
    private bool isDragging;
    private Vector2 touchPosition;

    private CardData _currentCardData;
    
    private Action<CardController> _onCardReleased;

    public void Initialize(CardId cardId, Transform movingCardsParent, Action<CardController> onCardReleased)
    {
        _onCardReleased = onCardReleased;

        _currentCardData = cardsDatabase.GetCardDataCopy(cardId);
        cardView.Initialize(movingCardsParent);
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

    public void SetParent(Transform parent, bool worldPositionStays)
    {
        cardView.SetParent(parent, worldPositionStays);
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
        if (defender._currentCardData.vitality > 0)
        {
            defender.cardView.RefreshCardView(defender._currentCardData);
        }
        else
        {
            DestroyImmediate(defender.gameObject);
        }
        cardView.ResetPosition();
    }

    public void ResetPosition()
    {
        cardView.ResetPosition();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public int GetAttack()
    {
        return _currentCardData.attack;
    }

    public string GetFriendlyName()
    {
        return _currentCardData.skillId.ToString();
    }

    public void DestroySelf()
    {
        GameObject.DestroyImmediate(gameObject);
    }
}

[Serializable]
public class CardView
{
    [Serializable]
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
            
            // Hide NONE skill
            skillText.transform.parent.gameObject.SetActive(skillId != SkillId.NONE);
        }
        public void SetCharacterArt(Sprite art)
        {
            characterArt.sprite = art;
        }

        public void SetName(string cardDataFriendlyName)
        {
            skillText.text = cardDataFriendlyName;
        }
    }

    public Transform root;
    [FormerlySerializedAs("verticalLayout")] public CardLayout verticalView;
    [FormerlySerializedAs("horizontalLayout")] public CardLayout horizontalView;
    public float rotationSpeedX;
    public float maxRotationAngleX;
    public float rotationSpeedY;
    public float maxRotationAngleY;


    private bool _isDragging;
    private Vector2 _touchPosition;
    private Vector2 _cachedPosition;
    private Transform _cachedPreviousParent;
    private Transform _movingCardsParent;


    public void Initialize(Transform movingCardsParent)
    {
        _movingCardsParent = movingCardsParent;
    }
    
    public void RefreshCardView(CardData cardData)
    {
        verticalView.SetAttack(cardData.attack);
        verticalView.SetVitality(cardData.vitality);
        // verticalView.SetSkill(cardData.skillId);
        verticalView.SetName(cardData.friendlyName);
        verticalView.SetCharacterArt(cardData.verticalCharacterArt);
        
        horizontalView.SetAttack(cardData.attack);
        horizontalView.SetVitality(cardData.vitality);
        // horizontalView.SetSkill(cardData.skillId);
        horizontalView.SetName(cardData.friendlyName);
        horizontalView.SetCharacterArt(cardData.horizontalCharacterArt);

        root.gameObject.name = cardData.cardId.ToString();
    }
    
    public void OnPointerDown()
    {
        _isDragging = true;

        _touchPosition = GetPointerPosition();
        _cachedPosition = root.position;

        _cachedPreviousParent = root.parent;
        // TODO: Try to do the trick without moving the _movingCardsParent
        _movingCardsParent.position = root.parent.position;
        SetParent(_movingCardsParent, worldPositionStays: true);
    }
    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.isInProgress)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
    
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }

    public void OnPointerUp()
    {
        // Set back to the previous parent just in case, knowing that the controllers will set another parent if needed
        SetParent(_cachedPreviousParent, worldPositionStays: true);
        _cachedPreviousParent = null;
        _isDragging = false;
    }
    public void UpdateDrag()
    {
        // Convert screen positions to Canvas space
        Vector2 newTouchPosition = GetPointerPosition();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform, newTouchPosition, Camera.main,
            out Vector2 currentLocalPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform, _touchPosition, Camera.main,
            out Vector2 previousLocalPosition);

        // Calculate delta in canvas space
        Vector2 positionDelta = currentLocalPosition - previousLocalPosition;
        
        // Update rotation for tilt effect
        float rotationX = Mathf.Clamp(positionDelta.y * rotationSpeedX, min: -maxRotationAngleX, max: maxRotationAngleX);
        float rotationY = Mathf.Clamp(positionDelta.x * rotationSpeedY, min: -maxRotationAngleY, max: maxRotationAngleY);
        root.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);

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

    public void SetParent(Transform newParent, bool worldPositionStays)
    {
        root.SetParent(newParent, worldPositionStays);
    }
}