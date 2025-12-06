using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
public class Gate : MonoBehaviour
{
    private bool isOpen = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D gateCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gateCollider = GetComponent<BoxCollider2D>();

        spriteRenderer.enabled = true;
        gateCollider.enabled = true;
    }

    public void OnInteractorActivated()
    {
        ToggleGate();
    }

    public void ToggleGate()
    {
        if (isOpen)
        {
            CloseGate();
        }
        else
        {
            OpenGate();
        }
    }

    void OpenGate()
    {
        if (isOpen) return;
        
        isOpen = true;
        spriteRenderer.enabled = false;
        gateCollider.enabled = false;
    }

    void CloseGate()
    {
        if (!isOpen) return;
        
        isOpen = false;
        spriteRenderer.enabled = true;
        gateCollider.enabled = true;
    }
}
}
