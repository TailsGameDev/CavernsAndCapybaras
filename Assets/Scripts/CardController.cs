using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
    public PlayerControls controls;
    private bool isDragging;
    private Vector2 touchPosition;
    
    public void Awake()
    {
        cardView.root = transform;
    }

    public void Update()
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
        cardView.OnPointerDown();
        isDragging = false;
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
    public CardLayout verticalLayout;
    public CardLayout horizontalLayout;


    private bool isDragging;
    private Vector2 touchPosition;
    private Vector2 cachedPosition;
    
    public void OnPointerDown()
    {
        isDragging = true;

        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        cachedPosition = root.position;
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
}