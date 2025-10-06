using UnityEngine;
using System.Collections;

/// <summary>
/// Displays an animated "Inventory Full" message that floats up and fades out.
/// </summary>
public class InventoryFullMessage : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float floatSpeed = 0.5f;
    [SerializeField] private float popScale = 1.2f;

    private TextMesh textMesh;
    private float elapsedTime = 0f;

    /// <summary>
    /// Shows an "Inventory Full" message at the specified position.
    /// </summary>
    public static GameObject Show(Vector3 position, string message = "Inventory Full")
    {
        // Create GameObject
        GameObject messageObj = new GameObject("InventoryFullMessage");
        messageObj.transform.position = position;

        // Try to use TextMesh Pro if available, otherwise use legacy TextMesh
        InventoryFullMessage msgComponent = messageObj.AddComponent<InventoryFullMessage>();

        // Add TextMesh (legacy - always available)
        TextMesh textMesh = messageObj.AddComponent<TextMesh>();
        textMesh.text = message;
        textMesh.fontSize = 50; // Smaller font size
        textMesh.characterSize = 0.05f; // Scale down character size to match player size
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = new Color(1f, 0.3f, 0.3f, 0f); // Red, start transparent

        // Add MeshRenderer
        MeshRenderer meshRenderer = messageObj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = 100; // Render on top
        }

        msgComponent.textMesh = textMesh;

        Debug.Log($"Showing inventory full message at {position}");
        return messageObj;
    }

    private void Update()
    {
        if (textMesh == null)
        {
            Destroy(gameObject);
            return;
        }

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Scale animation (pop in, then normalize)
        float scaleT = Mathf.Clamp01(elapsedTime / 0.3f); // First 0.3 seconds
        float scale = Mathf.Lerp(0.8f, popScale, scaleT);
        if (elapsedTime > 0.3f)
        {
            scale = Mathf.Lerp(popScale, 1.0f, (elapsedTime - 0.3f) / 0.3f);
        }
        transform.localScale = Vector3.one * scale;

        // Fade in and out
        float alpha;
        if (t < 0.2f)
        {
            // Fade in (first 20%)
            alpha = Mathf.Lerp(0f, 1f, t / 0.2f);
        }
        else if (t > 0.8f)
        {
            // Fade out (last 20%)
            alpha = Mathf.Lerp(1f, 0f, (t - 0.8f) / 0.2f);
        }
        else
        {
            // Hold (middle 60%)
            alpha = 1f;
        }

        Color color = textMesh.color;
        color.a = alpha;
        textMesh.color = color;

        // Destroy when done
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
