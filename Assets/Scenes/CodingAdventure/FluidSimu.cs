using UnityEngine;

public class FluidSimu : MonoBehaviour
{
    public float gravity;
    public GameObject circlePrefab; // Référence au préfab de cercle
    public float particleSize;
    public Vector2 boundsSize;
    public float collisionDamping;

    public int numParticles;
    public float particleSpacing;

    private Vector2[] position;
    private Vector2[] velocity;
    private GameObject[] circles; // Tableau pour stocker les instances de cercle

    private void Start()
    {
        position = new Vector2[numParticles];
        velocity = new Vector2[numParticles];
        circles = new GameObject[numParticles]; // Initialiser le tableau de cercles

        int particlesPerRow = (int)Mathf.Sqrt(numParticles);
        int particlesPerCol = (numParticles - 1) / particlesPerRow + 1;
        float spacing = particleSize * 2 + particleSpacing;

        for (int i = 0; i < numParticles; i++)
        {
            float x = (i % particlesPerRow - particlesPerRow / 2f + .5f) * spacing;
            float y = (i / particlesPerRow - particlesPerCol / 2f + .5f) * spacing;
            position[i] = new Vector2(x, y);

            // Instancier le préfab de cercle
            circles[i] = Instantiate(circlePrefab, position[i], Quaternion.identity);
        }
    }

    private void Update()
    {
        for (int i = 0; i < position.Length; i++)
        {
            velocity[i] += Vector2.down * gravity * Time.deltaTime;
            position[i] += velocity[i] * Time.deltaTime;
            ResolveCollisions(ref position[i], ref velocity[i]);
            circles[i].transform.position = position[i]; // Mettre à jour la position du cercle
        }
    }

    void ResolveCollisions(ref Vector2 position, ref Vector2 velocity)
    {
        Vector2 halfBoundsSize = boundsSize / 2 - Vector2.one * particleSize;

        if (Mathf.Abs(position.x) > halfBoundsSize.x)
        {
            position.x = halfBoundsSize.x * Mathf.Sign(position.x);
            velocity.x *= -1 * collisionDamping;
        }

        if (Mathf.Abs(position.y) > halfBoundsSize.y)
        {
            position.y = halfBoundsSize.y * Mathf.Sign(position.y);
            velocity.y *= -1 * collisionDamping;
        }
    }
}
