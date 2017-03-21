using UnityEngine;

[System.Serializable]
public class Duplication
{
    [Header("Duplication related")]
    public AsteroidType asteroidToSpawn;

    [Tooltip("Makes sure that when the asteroid is destroyed, the asteroids that are created won't go (too much) in the direction of the player.")]
    public bool easyDuplication;

    [Range(-1f, 1f), Tooltip("Close to -1: away from player ; close to 1: close to random direction")]
    public float duplicationDotOffset = -0.8f;

    [Range(1f, 5f)]
    public float minimimDestroyVelocityMultiplier = 2f;

    [Range(1f, 10f)]
    public float maximumDestroyVelocityMultiplier = 4f;

    public int numberOfAsteroidsToSpawn;
}

public class DuplicatingAsteroid : Asteroid
{
    [Header("Duplication related"), SerializeField]
    private Duplication m_Duplication;
    private float m_DestroyVelocityMultiplier = 1f;

    protected override void Awake()
    {
        base.Awake();

        InitialiseVelocity();
    }

    protected override void InitialiseVelocity()
    {
        base.InitialiseVelocity();

        EntityParameters.accelerationScalar *= 1 - (Random.Range(0f, m_AsteroidParameters.accelerationMultiplier));
    }

    protected override void OnCollisionWithKilling(Vector3 direction)
    {
        base.OnCollisionWithKilling(direction);

        switch (AsteroidType)
        {
            case AsteroidType.medium:
                m_DestroyVelocityMultiplier = m_Duplication.maximumDestroyVelocityMultiplier;
                break;
            case AsteroidType.big:
                m_DestroyVelocityMultiplier = m_Duplication.minimimDestroyVelocityMultiplier;
                break;
        }

        for (int i = 0; i < m_Duplication.numberOfAsteroidsToSpawn; i++)
        {
            Vector3 randomDirection = ExtensionMethods.RandomVector3();
            randomDirection.y = 0f;

            // Use this to make sure the asteroids won't go too much in the direction of the player ; Not present in the original version though... !
            if (m_Duplication.easyDuplication)
            {
                float dot = Vector3.Dot(randomDirection, direction);

                if (dot < m_Duplication.duplicationDotOffset)
                {
                    randomDirection = Vector3.zero;
                }

                GameManagement.Instance.SpawnAsteroid(m_Duplication.asteroidToSpawn, transform.position, direction + randomDirection * m_DestroyVelocityMultiplier);
            }
            else
            {
                GameManagement.Instance.SpawnAsteroid(m_Duplication.asteroidToSpawn, transform.position, randomDirection * m_DestroyVelocityMultiplier);
            }
        }
    }
}