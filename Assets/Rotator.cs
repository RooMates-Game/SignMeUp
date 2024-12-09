using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private float flipSpeed = 20f; // Angular velocity

    private bool isFlipped = false; // Tracks if the card is flipped
    private bool isFlipping = false; // Prevents multiple flips at once

    public void OnMouseDown()
    {
        if (!isFlipping) // Make sure the card is not already flipping
        {
            FlipCard();
        }
    }

    private void FlipCard()
    {
        isFlipping = true;
        float rotationY = 0f; // Track the current Y rotation of the card

        // Rotate the card until it's flipped 180 degrees
        while (rotationY <= 180f) 
        {
            rotationY += flipSpeed * Time.deltaTime; // Increment the rotation
            transform.Rotate(Vector3.up * flipSpeed * Time.deltaTime); // Rotate the card
        }

        // After the loop ends, ensure the final rotation is exactly 180
        // transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Mark the card as flipped
        isFlipped = !isFlipped;
        isFlipping = false; // Allow for another flip after this one is done
    }
}
