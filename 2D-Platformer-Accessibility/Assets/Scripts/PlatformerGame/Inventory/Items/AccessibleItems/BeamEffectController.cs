using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BeamConnection
{
    public string connectionName = "New Connection";
    public Transform startPoint;
    public Transform endPoint;
    
    [Tooltip("Leave empty for automatic Manhattan path. Add points for custom routing.")]
    public List<Vector2> manualWaypoints = new List<Vector2>();
    
    [Header("Visual Settings")]
    public Color beamColor = new Color(1f, 0.5f, 0f, 1f); // Orange
    public float beamWidth = 0.15f;
    public float pulseSpeed = 2f;
    public bool isActive = true;
    
    [Header("Particle Effects")]
    public ParticleSystem startParticlesPrefab;
    public ParticleSystem endParticlesPrefab;
    public ParticleSystem beamParticlesPrefab;
    
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public ParticleSystem startParticlesInstance;
    [HideInInspector] public ParticleSystem endParticlesInstance;
    [HideInInspector] public ParticleSystem beamParticlesInstance;
}

public class BeamEffectController : MonoBehaviour
{
    [Header("Beam Connections")]
    public List<BeamConnection> beamConnections = new List<BeamConnection>();
    
    [Header("Default Prefabs (Optional)")]
    public ParticleSystem defaultStartParticles;
    public ParticleSystem defaultEndParticles;
    public ParticleSystem defaultBeamParticles;
    
    [Header("Global Settings")]
    public Material beamMaterial;
    public float particleDensity = 3f;
    public bool updateInRealTime = true;
    
    private void Start()
    {
        InitializeAllBeams();
    }
    
    private void Update()
    {
        if (updateInRealTime)
        {
            UpdateAllBeams();
        }
    }
    
    private void InitializeAllBeams()
    {
        foreach (BeamConnection connection in beamConnections)
        {
            if (connection.isActive && connection.startPoint != null && connection.endPoint != null)
            {
                InitializeBeam(connection);
            }
        }
    }
    
    private void InitializeBeam(BeamConnection connection)
    {
        // Create GameObject for this beam
        GameObject beamObject = new GameObject($"Beam_{connection.connectionName}");
        beamObject.transform.SetParent(this.transform);
        
        // Add and configure LineRenderer
        connection.lineRenderer = beamObject.AddComponent<LineRenderer>();
        connection.lineRenderer.positionCount = 0;
        connection.lineRenderer.startWidth = connection.beamWidth;
        connection.lineRenderer.endWidth = connection.beamWidth;
        connection.lineRenderer.material = beamMaterial != null ? beamMaterial : CreateDefaultMaterial();
        connection.lineRenderer.startColor = connection.beamColor;
        connection.lineRenderer.endColor = connection.beamColor;
        connection.lineRenderer.textureMode = LineTextureMode.Tile;
        connection.lineRenderer.sortingLayerName = "Effects";
        connection.lineRenderer.sortingOrder = 1;
        
        // Instantiate particle systems
        InstantiateParticleSystems(connection, beamObject);
        
        // Initial update
        UpdateBeamPath(connection);
    }
    
    private Material CreateDefaultMaterial()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = Color.white;
        return mat;
    }
    
    private void InstantiateParticleSystems(BeamConnection connection, GameObject parentObject)
    {
        // Start particles
        ParticleSystem startPrefab = connection.startParticlesPrefab != null ? 
            connection.startParticlesPrefab : defaultStartParticles;
        if (startPrefab != null)
        {
            connection.startParticlesInstance = Instantiate(startPrefab, parentObject.transform);
            connection.startParticlesInstance.transform.localPosition = Vector3.zero;
        }
        
        // End particles
        ParticleSystem endPrefab = connection.endParticlesPrefab != null ? 
            connection.endParticlesPrefab : defaultEndParticles;
        if (endPrefab != null)
        {
            connection.endParticlesInstance = Instantiate(endPrefab, parentObject.transform);
            connection.endParticlesInstance.transform.localPosition = Vector3.zero;
        }
        
        // Beam particles
        ParticleSystem beamPrefab = connection.beamParticlesPrefab != null ? 
            connection.beamParticlesPrefab : defaultBeamParticles;
        if (beamPrefab != null)
        {
            connection.beamParticlesInstance = Instantiate(beamPrefab, parentObject.transform);
            connection.beamParticlesInstance.transform.localPosition = Vector3.zero;
        }
    }
    
    private void UpdateAllBeams()
    {
        foreach (BeamConnection connection in beamConnections)
        {
            if (connection.isActive && connection.lineRenderer != null)
            {
                UpdateBeamPath(connection);
            }
        }
    }
    
    private void UpdateBeamPath(BeamConnection connection)
    {
        if (connection.startPoint == null || connection.endPoint == null) return;
        
        Vector2 startPos = connection.startPoint.position;
        Vector2 endPos = connection.endPoint.position;
        
        // Get the path points
        List<Vector2> pathPoints = GetBeamPath(connection, startPos, endPos);
        
        // Update LineRenderer
        connection.lineRenderer.positionCount = pathPoints.Count;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            connection.lineRenderer.SetPosition(i, pathPoints[i]);
        }
        
        // Update particle systems
        UpdateParticleSystems(connection, pathPoints);
        
        // Apply pulsing effect
        ApplyPulsingEffect(connection);
    }
    
    private List<Vector2> GetBeamPath(BeamConnection connection, Vector2 start, Vector2 end)
    {
        List<Vector2> path = new List<Vector2>();
        
        if (connection.manualWaypoints.Count > 0)
        {
            // Use manual waypoints
            path.Add(start);
            path.AddRange(connection.manualWaypoints);
            path.Add(end);
            
            // Ensure no diagonal movement
            path = SnapToGrid(path);
        }
        else
        {
            // Generate automatic Manhattan path (no diagonals)
            path.Add(start);
            
            // Horizontal then vertical (you can change this order)
            Vector2 horizontalPoint = new Vector2(end.x, start.y);
            path.Add(horizontalPoint);
            
            path.Add(end);
        }
        
        return path;
    }
    
    private List<Vector2> SnapToGrid(List<Vector2> path)
    {
        List<Vector2> snappedPath = new List<Vector2>();
        
        for (int i = 0; i < path.Count; i++)
        {
            snappedPath.Add(path[i]);
            
            // Ensure no diagonal movement between points
            if (i < path.Count - 1)
            {
                Vector2 current = path[i];
                Vector2 next = path[i + 1];
                
                // If both X and Y are different, insert a midpoint
                if (!Mathf.Approximately(current.x, next.x) && !Mathf.Approximately(current.y, next.y))
                {
                    snappedPath.Add(new Vector2(next.x, current.y));
                }
            }
        }
        
        return snappedPath;
    }
    
    private void UpdateParticleSystems(BeamConnection connection, List<Vector2> path)
    {
        // Update start particles
        if (connection.startParticlesInstance != null)
        {
            connection.startParticlesInstance.transform.position = path[0];
            if (!connection.startParticlesInstance.isPlaying)
                connection.startParticlesInstance.Play();
        }
        
        // Update end particles
        if (connection.endParticlesInstance != null)
        {
            connection.endParticlesInstance.transform.position = path[path.Count - 1];
            if (!connection.endParticlesInstance.isPlaying)
                connection.endParticlesInstance.Play();
        }
        
        // Emit particles along the beam
        EmitBeamParticles(connection, path);
    }
    
    private void EmitBeamParticles(BeamConnection connection, List<Vector2> path)
    {
        if (connection.beamParticlesInstance == null) return;
        
        // Clear existing particles
        connection.beamParticlesInstance.Clear();
        
        // Emit particles for each segment
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 segmentStart = path[i];
            Vector2 segmentEnd = path[i + 1];
            float segmentLength = Vector2.Distance(segmentStart, segmentEnd);
            
            if (segmentLength > 0)
            {
                int particlesToEmit = Mathf.CeilToInt(segmentLength * particleDensity);
                
                for (int j = 0; j <= particlesToEmit; j++)
                {
                    float t = j / (float)particlesToEmit;
                    Vector2 particlePos = Vector2.Lerp(segmentStart, segmentEnd, t);
                    
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                    {
                        position = particlePos,
                        startSize = connection.beamWidth * 0.7f,
                        startColor = connection.beamColor,
                        velocity = Vector3.zero,
                        startLifetime = 0.3f
                    };
                    connection.beamParticlesInstance.Emit(emitParams, 1);
                }
            }
        }
    }
    
    private void ApplyPulsingEffect(BeamConnection connection)
    {
        if (connection.pulseSpeed > 0 && connection.lineRenderer != null)
        {
            float pulseValue = (Mathf.Sin(Time.time * connection.pulseSpeed) + 1f) * 0.5f;
            Color pulsedColor = connection.beamColor;
            pulsedColor.a = Mathf.Lerp(0.3f, 1f, pulseValue);
            
            connection.lineRenderer.startColor = pulsedColor;
            connection.lineRenderer.endColor = pulsedColor;
        }
    }
    
    // Public methods for runtime control
    public void ActivateBeam(string connectionName)
    {
        BeamConnection connection = beamConnections.Find(c => c.connectionName == connectionName);
        if (connection != null)
        {
            connection.isActive = true;
            if (connection.lineRenderer != null)
                connection.lineRenderer.enabled = true;
        }
    }
    
    public void DeactivateBeam(string connectionName)
    {
        BeamConnection connection = beamConnections.Find(c => c.connectionName == connectionName);
        if (connection != null)
        {
            connection.isActive = false;
            if (connection.lineRenderer != null)
                connection.lineRenderer.enabled = false;
        }
    }
    
    public void AddBeamConnection(Transform start, Transform end, List<Vector2> waypoints = null)
    {
        BeamConnection newConnection = new BeamConnection
        {
            connectionName = $"Beam_{beamConnections.Count}",
            startPoint = start,
            endPoint = end,
            beamColor = new Color(1f, 0.5f, 0f, 1f),
            beamWidth = 0.15f
        };
        
        if (waypoints != null)
            newConnection.manualWaypoints = waypoints;
        
        beamConnections.Add(newConnection);
        InitializeBeam(newConnection);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw gizmos for beam connections in editor
        foreach (BeamConnection connection in beamConnections)
        {
            if (connection.startPoint != null && connection.endPoint != null)
            {
                Gizmos.color = connection.beamColor;
                Gizmos.DrawLine(connection.startPoint.position, connection.endPoint.position);
                
                // Draw waypoints
                Gizmos.color = Color.cyan;
                foreach (Vector2 waypoint in connection.manualWaypoints)
                {
                    Gizmos.DrawSphere(waypoint, 0.1f);
                }
            }
        }
    }
}