using System.Collections;
using UnityEngine;

public class FlipCard : MonoBehaviour
{
    private SpriteRenderer rend;

    [SerializeField]
    private Sprite faceSprite, backSprite;
    [SerializeField] private float delayMatch, delayNoMatch;

    private bool coroutineAllowed, facedUp, isLocked;

    // Static variable to track the first selected card
    public static GameObject selectedCard;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = backSprite;
        coroutineAllowed = true;
        facedUp = false;
        isLocked = false;

        // Ensure the face sprite is scaled properly
        AdjustFaceSpriteScale();
    }

    private void OnMouseDown()
    {
        // Ignore clicks if the card is locked or a coroutine is running
        if (isLocked || !coroutineAllowed) return;

        StartCoroutine(RotateCard());

        if (!facedUp && selectedCard == null)
        {
            // First card selected
            selectedCard = this.gameObject;
            isLocked = true; // Lock this card so it cannot be clicked again
        }
        else if (!facedUp && selectedCard != null)
        {
            // Second card selected
            FlipCard firstCardScript = selectedCard.GetComponent<FlipCard>();

            if (selectedCard.CompareTag(this.tag))
            {
                // Cards match, wait before destroying both
                Debug.Log("Match Found!");
                StartCoroutine(WaitBeforeDestroy(selectedCard, this.gameObject, delayMatch));
            }
            else
            {
                // No match, reset both cards
                Debug.Log("No Match!");
                StartCoroutine(ResetCard(selectedCard));
                StartCoroutine(ResetCard(this.gameObject));
            }

            firstCardScript.isLocked = false; // Unlock the first card
            selectedCard = null; // Reset for the next pair
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
            cardScript.isLocked = false; // Unlock the card so it can be selected again
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
