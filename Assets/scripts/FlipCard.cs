using UnityEngine;

public class CardFlip : MonoBehaviour
{
    [SerializeField] private GameObject frontFace; // Assign the front face (Image GameObject)
    [SerializeField] private GameObject backFace;  // Assign the back face (SquareBackCover GameObject)
    private bool isFlipped = false; // Tracks if the card is flipped
    private bool isFlipping = false; // Prevents multiple flips at once
    [SerializeField] private float flipSpeed = 5f; // Speed of the flip animation

    void OnMouseDown()
    {
        if (!isFlipping) // Prevent double-clicking issues
        {
            StartCoroutine(FlipCard());
        }
    }

    private System.Collections.IEnumerator FlipCard()
    {
        isFlipping = true;

        // Initially set visibility based on the current state
        frontFace.SetActive(!isFlipped);
        backFace.SetActive(isFlipped);

        // Rotate the card 180 degrees over time
        float rotationY = 0f;
        while (rotationY < 180f)
        {
            float step = flipSpeed * Time.deltaTime * 90f; // Control rotation speed
            transform.Rotate(0, step, 0);  // Rotate the whole card around the Y-axis
            rotationY += step;

            // Swap visibility at the halfway point (90 degrees)
            if (rotationY >= 90f && rotationY - step < 90f)
            {
                // Swap the front and back faces based on the flipped state
                frontFace.SetActive(isFlipped);  // Show the back face when flipped
                backFace.SetActive(!isFlipped);  // Hide the front face when flipped
            }

            yield return null;  // Wait until the next frame before continuing
        }

        // Reset rotation and flip state
        transform.eulerAngles = new Vector3(0, isFlipped ? 180 : 0, 0);
        isFlipped = !isFlipped;  // Toggle the flip state
        isFlipping = false;  // Allow the next flip
    }
}
