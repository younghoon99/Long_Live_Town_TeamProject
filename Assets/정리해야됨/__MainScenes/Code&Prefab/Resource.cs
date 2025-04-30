using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public string objectName; // Name of the resource
    public Sprite sprite; // Sprite representing the resource
    public int value; // Value of the resource

    private void Start()
    {
                // Initialize the resource with default values if needed
        objectName = "DefaultResource";
        sprite = null; // Assign a default sprite if necessary
        
    }
    public void SetResourceName(string name)
    {
        objectName = name;
    }
    public void SetResourceSprite(Sprite newSprite)
    {
        sprite = newSprite;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }
    public void SetResourceValue(int newValue)
    {
        value = newValue;
    }
}
