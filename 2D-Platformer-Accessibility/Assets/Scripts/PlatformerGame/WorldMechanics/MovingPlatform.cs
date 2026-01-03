using UnityEngine;
using System.Collections;
using PlatformerGame.Player;

namespace PlatformerGame.WorldMechanics
{
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform endPositionTransform;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float accelerationDistance = 1f;
    private Vector2 endPosition;

    [Header("Platform State")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool isVertical = false;
    
    
    private Vector2 startPosition;
    private bool movingToEnd = true;
    private SpriteRenderer spriteRenderer;
    private Coroutine movementCoroutine;

    private AudioSource audio;

    public Vector2 velocity;
    

    private void Awake()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        endPosition = endPositionTransform.position;
        movementCoroutine = StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        Debug.Log("Movement Corouting started");
        while (isActive)
        {
            Vector2 targetPosition = movingToEnd ? endPosition : startPosition;
            Vector2 currentPosition = transform.position;
            
            // Calculate distance to target
            float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
            
            // Calculate speed based on distance (ease in/out)
            float currentSpeed = CalculateSpeed(distanceToTarget);
            
            // Move towards target
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, currentSpeed * Time.deltaTime);
            velocity = (newPosition - currentPosition) / Time.deltaTime;
            transform.position = newPosition;
            
            // Check if reached target
            if (Vector2.Distance(newPosition, targetPosition) < 0.01f)
            {
                movingToEnd = !movingToEnd;
                // Small delay at turnaround point
                yield return new WaitForSeconds(0.5f);
            }
            
            yield return null;
        }
        
        velocity = Vector2.zero;
    }

    private float CalculateSpeed(float distanceToTarget)
    {
        // Slow down when approaching start or end positions
        float accelerationZone = Mathf.Min(accelerationDistance, Vector2.Distance(startPosition, endPosition) * 0.3f);
        
        if (distanceToTarget < accelerationZone)
        {
            // Ease out when approaching target
            return moveSpeed * (distanceToTarget / accelerationZone);
        }
        
        // Full speed in the middle
        return moveSpeed;
    }

    public void ActivatePlatform()
    {
        if (isActive) return;
        
        isActive = true;
        
        // Start movement coroutine
        movementCoroutine = StartCoroutine(MovementRoutine());
    }

    public void TogglePlatform()
    {
        if (isActive)
        {
            DeactivatePlatform();
        }
        else
        {
            ActivatePlatform();
        }
    }

    public void DeactivatePlatform()
    {
        if (!isActive) return;
        
        isActive = false;
        velocity = Vector2.zero;
        
        // Stop movement coroutine
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
        
        if (isVertical)
            transform.position = startPosition + Vector2.down * 0.5f;
    }

    // Call this from your lever or other activation objects
    public void OnInteractorActivated()
    {
        TogglePlatform();
    }

}
}

