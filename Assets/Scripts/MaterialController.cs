using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField] private Material customMaterial;
    [SerializeField] private ParticleSystem particleSystem;

    private Renderer renderer;

    void Start()
    {
        // Apply the material if assigned
        renderer = GetComponent<Renderer>();
        if (customMaterial != null && renderer != null)
        {
            renderer.material = customMaterial;
        }

        // Play the particle system
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    public void TriggerSparkle()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }
}
