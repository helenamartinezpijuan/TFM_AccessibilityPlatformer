using UnityEngine;

namespace PlatformerGame.WorldMechanics
{
public class Lever : MonoBehaviour
{
    [Header("Lever One or Lever Two")]
    [SerializeField] private bool isOne;
    
    [Header("Visuals")]
    [SerializeField] private GameObject accessibleVisuals;
    private Animator leverAnimator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        leverAnimator = GetComponent<Animator>();
        leverAnimator.SetBool("IsOne", isOne);
    }

    public void TriggerAnimation()
    {        
        if (leverAnimator != null)
        {
            leverAnimator.SetBool("IsOn", true);
        }
    }

    public void InstantiateAccessibleVisuals()
    {
        GameObject.Instantiate(accessibleVisuals, transform.position, Quaternion.identity);
    }
}
}