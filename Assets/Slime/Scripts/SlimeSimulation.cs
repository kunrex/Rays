using System.Runtime.InteropServices;

using UnityEngine;

using Slime.Scripts;

public class SlimeSimulation : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private int agentCount;
    private Agent[] agents;

    [SerializeField] private Color slimeColour;
    
    [SerializeField] private float agentSpeed;
    [SerializeField] private float agentTurnStrength;
    
    [SerializeField] private float blendStrength;
    [SerializeField] private float cleanUpStrength;

    [SerializeField] private float agentViewAngle;
    [SerializeField] private float agentViewDistance;
    
    [SerializeField] private RenderTexture texture;
    [SerializeField] private ComputeShader slimeShader;
    [SerializeField] private ComputeShader trailShader;

    private void Start()
    {
        texture = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        texture.enableRandomWrite = true;
        texture.Create();

        agents = new Agent[agentCount];

        Square();
    }

    private void Circle()
    {
        float cos = Mathf.Cos(360f / agentCount * Mathf.Deg2Rad), sin = Mathf.Sin(360f / agentCount * Mathf.Deg2Rad);
        
        var currentDirection = new Vector2(1, 0);
        for (int i = 0; i < agentCount; i++)
        {
            var agent = new Agent
            {
                forward = currentDirection,
                normalizedPosition = new Vector2(.5f, .5f) + currentDirection * .3f
            };
            
            currentDirection = new Vector2(currentDirection.x * cos - currentDirection.y * sin, currentDirection.x * sin + currentDirection.y * cos);
            agents[i] = agent;
        }
    }

    private void Square()
    {
        float radius = 1f;
        
        for (int i = 0; i < agentCount; i++)
        {
            var random = new Vector2(Random.Range(-radius / 2, radius / 2), Random.Range(-radius / 2, radius / 2));
            var position = new Vector2(.5f, .5f) + random;
            
            var agent = new Agent()
            {
                normalizedPosition = position,
                forward = -random.normalized
            };
            
            agents[i] = agent;
        }
    }

    private void SetConstants()
    {
        slimeShader.SetFloat("_agentSpeed", agentSpeed);
        slimeShader.SetFloat("_agentTurnStrength", agentTurnStrength);
        
        slimeShader.SetFloat("_agentViewAngle", agentViewAngle * Mathf.Deg2Rad);
        slimeShader.SetFloat("_agentViewDistance", agentViewDistance);
        
        slimeShader.SetVector("_slimeColour", slimeColour);
        
        trailShader.SetFloat("_blendStrength", blendStrength);
        trailShader.SetFloat("_cleanUpStrength", cleanUpStrength);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var kernel = slimeShader.FindKernel("CSMain");
        slimeShader.SetTexture(kernel, "source", texture);
        slimeShader.SetTexture(kernel, "result", texture);
        
        SetConstants();
        slimeShader.SetFloat("deltaTime", Time.deltaTime);

        var buffer = new ComputeBuffer(agentCount, Marshal.SizeOf<Agent>());
        buffer.SetData(agents);
        
        slimeShader.SetBuffer(kernel, "agents", buffer);
        
        slimeShader.Dispatch(kernel, agentCount / 10, 1, 1);
        
        buffer.GetData(agents);
        
        buffer.Dispose();
        
        trailShader.SetTexture(kernel, "source", texture);
        trailShader.SetTexture(kernel, "result", texture);
        
        trailShader.SetFloat("deltaTime", Time.deltaTime);
        
        trailShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
        
        Graphics.Blit(texture, destination);
    }
}
