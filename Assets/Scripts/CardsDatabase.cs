using UnityEngine;

public class CardsDatabase : ScriptableObject
{
    public CardData[] cardsData;
    
    public CardData GetCardDataCopy(CardId cardId)
    {
        foreach (var cardData in cardsData)
        {
            if (cardData.cardId == cardId)
            {
                return cardData.GetCopy();
            }
        }
        
        Debug.LogError("Card not found in database: " + cardId);
        
        return null;
    }
}

[System.Serializable]
public class CardData
{
    public CardId cardId;
    public int attack;
    public int vitality;
    public SkillId skillId;
    public Sprite verticalCharacterArt;
    public Sprite horizontalCharacterArt;

    public CardData GetCopy()
    {
        CardData copy = new CardData();
        copy.cardId = this.cardId;
        copy.attack = this.attack;
        copy.vitality = this.vitality;
        copy.skillId = this.skillId;
        copy.verticalCharacterArt = this.verticalCharacterArt;
        copy.horizontalCharacterArt = this.horizontalCharacterArt;
        return copy;
    }
}

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