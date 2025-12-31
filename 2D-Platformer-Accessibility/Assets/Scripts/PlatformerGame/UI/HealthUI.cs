using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthUI : MonoBehaviour
{
    private Transform heartsContainer;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private float transitionDuration = 0.3f;
    
    private Image[] heartImages;
    private Coroutine[] transitionCoroutines;

    private void Start()
    {
        heartsContainer = GameObject.FindWithTag("HeartContainer").transform;
    }
    
    public void Initialize(int health)
    {
        heartImages = new Image[health];
        transitionCoroutines = new Coroutine[health];
        
        for (int i = 0; i < heartsContainer.childCount && i < health; i++)
        {
            heartImages[i] = heartsContainer.GetChild(i).GetComponent<Image>();
        }
        
        SetHealth(health);
    }
    
    public void SetHealth(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            bool shouldBeFull = i < currentHealth;
            Debug.Log($"Current health: {currentHealth} - Should be full: {shouldBeFull}");
            
            if (heartImages[i].sprite == (shouldBeFull ? emptyHeartSprite : fullHeartSprite))
            {
                // Heart state changed, trigger transition
                if (transitionCoroutines[i] != null)
                {
                    StopCoroutine(transitionCoroutines[i]);
                }
                transitionCoroutines[i] = StartCoroutine(AnimateHeartTransition(heartImages[i], shouldBeFull));
            }
        }
    }
    
    private IEnumerator AnimateHeartTransition(Image heart, bool toFull)
    {
        float elapsed = 0f;
        Vector3 originalScale = heart.transform.localScale;
        
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            
            // Bounce effect
            float bounce = 1f + Mathf.Sin(t * Mathf.PI) * 0.25f;
            heart.transform.localScale = originalScale * bounce;
            
            // Color pulse
            Color pulseColor = toFull ? Color.green : Color.red;
            heart.color = Color.Lerp(Color.white, pulseColor, Mathf.PingPong(t * 2, 1));
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Set final state
        heart.transform.localScale = originalScale;
        heart.color = Color.white;
        heart.sprite = toFull ? fullHeartSprite : emptyHeartSprite;
    }
}