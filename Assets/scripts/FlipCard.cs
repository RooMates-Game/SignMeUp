using System.Collections;
using UnityEngine;

public class FlipCard : MonoBehaviour
{
    private SpriteRenderer rend;

    [SerializeField]
    private Sprite faceSprite, backSprite;
    [SerializeField] private float delayMatch, delayNoMatch;

    private bool coroutineAllowed, facedUp;

    // Static variable to track the first selected card
    public static GameObject selectedCard;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = backSprite;
        coroutineAllowed = true;
        facedUp = false;

        // Ensure the face sprite is scaled properly
        AdjustFaceSpriteScale();
    }

    private void OnMouseDown()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(RotateCard());

            if (!facedUp && selectedCard == null)
            {
                // First card selected
                selectedCard = this.gameObject;
            }
            else if (!facedUp && selectedCard != null)
            {
                // Second card selected
                if (selectedCard.CompareTag(this.tag))
                {
                    // Cards match, wait before destroying both
                    Debug.Log("Match Found!");
                    
                    // Wait for a few seconds before destroying the cards
                    StartCoroutine(WaitBeforeDestroy(selectedCard, this.gameObject, delayMatch));
                }
                else
                {
                    // No match, reset both cards
                    Debug.Log("No Match!");
                    StartCoroutine(ResetCard(selectedCard));
                    StartCoroutine(ResetCard(this.gameObject));
                }
                selectedCard = null; // Reset for the next pair
            }
        }
    }

    private IEnumerator RotateCard()
    {
        coroutineAllowed = false;

        if (!facedUp)
        {
            for (float i = 0f; i >= -180f; i -= 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == -90f)
                {
                    rend.sprite = faceSprite;
                }
                yield return new WaitForSeconds(delayNoMatch);
            }
        }
        else if (facedUp)
        {
            for (float i = -180f; i <= 0f; i += 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == -90f)
                {
                    rend.sprite = backSprite;
                }
                yield return new WaitForSeconds(delayNoMatch);
            }
        }

        coroutineAllowed = true;
        facedUp = !facedUp;
    }

    private IEnumerator ResetCard(GameObject card)
    {
        // Delay before resetting
        yield return new WaitForSeconds(1f);

        FlipCard cardScript = card.GetComponent<FlipCard>();
        if (cardScript != null)
        {
            cardScript.StartCoroutine(cardScript.RotateCard());
        }
    }

    private IEnumerator WaitBeforeDestroy(GameObject card1, GameObject card2, float delay)
    {
        // Wait for the specified delay before destroying the cards
        yield return new WaitForSeconds(delay);

        Destroy(card1);
        Destroy(card2);
    }

    private void AdjustFaceSpriteScale()
    {
        // Get the size of both sprites
        Vector2 backSize = backSprite.bounds.size;
        Vector2 faceSize = faceSprite.bounds.size;

        // Calculate the scale adjustment to match backSprite
        float scaleX = backSize.x / faceSize.x;
        float scaleY = backSize.y / faceSize.y;

        // Apply scale to the GameObject's transform
        transform.localScale = new Vector3(scaleX * transform.localScale.x, scaleY * transform.localScale.y, transform.localScale.z);
    }
}
