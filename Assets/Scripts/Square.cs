using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public SpriteRenderer icon; // Reference to the tile's icon
    public int tileId; // Unique ID to identify matching pairs
    public bool isEmpty = false;

    public bool isRevealed = false;
    public BoxCollider2D boxCollider2D;
    private void OnEnable()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    public void RevealTile()
    {
        if (!isRevealed && !isEmpty)
        {
            isRevealed = true;
            icon.enabled = true;
            GameManager.instance.OnTileSelected(this);
        }
    }

    public void HideTile()
    {
        isRevealed = false;
        icon.enabled = false;
        boxCollider2D.enabled = false;
        isEmpty = true;
    }
    public void HighlightTile()
    {
        // Example: Change tile color temporarily
        icon.color = Color.yellow;
        StartCoroutine(ResetHighlight());
    }

    private IEnumerator ResetHighlight()
    {
        yield return new WaitForSeconds(0.2f);
        icon.color = Color.white;
        isRevealed = false;
    }
   
    public void ResetFlag(bool isReveale)
    {
        isRevealed = isReveale;
    }
}
