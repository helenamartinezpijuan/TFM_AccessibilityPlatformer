using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.WorldMechanics
{
    public class GateCombinationSystem : MonoBehaviour
    {
        public static GateCombinationSystem Instance { get; private set; }
        
        private List<CombinationGate> gates = new List<CombinationGate>();
        
        // Current state of all levers (using enum values)
        private HashSet<LeverType> activeLeverTypes = new HashSet<LeverType>();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
                Instance = this;
                // Keep the combination system alive across additive scene loads
                DontDestroyOnLoad(gameObject);
        }
        
        public void RegisterGate(CombinationGate gate)
        {
            if (!gates.Contains(gate))
            {
                gates.Add(gate);
            }
        }
        
        public void UnregisterGate(CombinationGate gate)
        {
            gates.Remove(gate);
        }
        
        public void LeverStateChanged(LeverType leverType, bool isActive)
        {
            if (isActive)
            {
                activeLeverTypes.Add(leverType);
            }
            else
            {
                activeLeverTypes.Remove(leverType);
            }
            
            Debug.Log($"Lever state changed. Active lever types: {string.Join(", ", activeLeverTypes)}");
            
            // Notify all gates to check their combinations
            foreach (var gate in gates)
            {
                gate.CheckCombination(activeLeverTypes);
            }
        }
        
        public HashSet<LeverType> GetActiveLeverTypes() => new HashSet<LeverType>(activeLeverTypes);
    }
}