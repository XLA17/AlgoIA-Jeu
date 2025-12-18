using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpriteSorting : MonoBehaviour
{
    [SerializeField] private bool dynamique = false;
    [SerializeField] private Transform offsetMarker;
    [SerializeField] private SortableSprite[] sprites;
    private int baseOrder;
    private float yOffset;
    void Start()
    {
        if (offsetMarker != null)
            yOffset = transform.position.y - offsetMarker.position.y;
        else
            yOffset = 0;


        Sorting();

        if (dynamique)
            InvokeRepeating("Sorting", 0.1f, 0.1f);

    }

  
    
    void Update()
    {
    }

    private void Sorting()
    {
        baseOrder = transform.GetSortingOrder(yOffset);
        foreach(SortableSprite sprite in sprites)
        {
            sprite.sprite.sortingOrder = baseOrder + sprite.relativeOrder;
        }
    }

    [Serializable]
    public struct SortableSprite
    {
        public SpriteRenderer sprite;
        public int relativeOrder;
    }

}


public static class TransformSorting
{
    public static int GetSortingOrder(this Transform transform, float yOffset =0)
    {
        return -(int)((transform.position.y- yOffset) * 100);
    }
}
