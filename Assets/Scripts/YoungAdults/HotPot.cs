using UnityEngine;
using Fungus;

public class HotPot : MonoBehaviour
{
    public void MakeMoney(int amount)
    {
        Debug.Log($"Made {amount} money!");
        End();
    }

    public void Leave()
    {
        Debug.Log("Leaving the hot pot!");
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