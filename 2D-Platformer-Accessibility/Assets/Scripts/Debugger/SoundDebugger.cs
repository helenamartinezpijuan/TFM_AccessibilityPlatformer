using UnityEngine;

public class SoundDebugger : MonoBehaviour
{
    [SerializeField] private AudioSource testAudioSource;
    [SerializeField] private AudioClip testClip;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=== SOUND DEBUG ===");
            Debug.Log($"AudioSource exists: {testAudioSource != null}");
            Debug.Log($"AudioClip exists: {testClip != null}");
            Debug.Log($"AudioSource enabled: {testAudioSource.enabled}");
            Debug.Log($"AudioSource volume: {testAudioSource.volume}");
            Debug.Log($"AudioSource isPlaying: {testAudioSource.isPlaying}");
            
            if (testAudioSource != null && testClip != null)
            {
                testAudioSource.PlayOneShot(testClip);
                Debug.Log("Test sound played!");
            }
        }
    }
}
