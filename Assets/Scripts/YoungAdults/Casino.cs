using UnityEngine;
using Fungus;

public class Casino : MonoBehaviour
{
    public void MakeMoney(int amount = 100)
    {
        Debug.Log($"You won {amount} dollars!");
        End();
    }
    
    public void Leave()
    {
        Debug.Log("You left the casino.");
        End();
    }

    private void End()
    {
        // Disable the Flowchart component
        var flowchart = GetComponent<Flowchart>();
        if (flowchart != null)
        {
            flowchart.enabled = false;
        }

        // Gray out the SpriteRenderer
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }
    }
}