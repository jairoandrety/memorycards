using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MemoryCardsSettings", menuName = "MemoryCards/Settings")]
public class MemoryCardsSettings : ScriptableObject
{
    public int minNumCards = 0;
    public int maxNumCards = 0;

    public Color colorHide;
    public Color colorSelected;
    public Color colorShow;

    public List<Sprite> sprites = new List<Sprite>();

    public Sprite GetSpriteByNumber(int number)
    {
        if (sprites.Count == 0)
            return null;
        if(number < 1 || number >= sprites.Count)
            return null;
    
        return sprites[number - 1];
    }
}