using UnityEngine;

public class DuplicatingAsteroid : Asteroid
{
    [Header("Duplication related")]
    [SerializeField]
    private AsteroidType m_AsteroidToSpawn;
    [SerializeField, Tooltip("Makes sure that when the asteroid is destroyed, the asteroids that are created won't go (too much) in the direction of the player.")]
    private bool m_MakeDuplicationEasier;
    [SerializeField, Range(-1f, 1f), Tooltip("Close to -1: away from player ; close to 1: close to random direction")]
    private float m_DuplicationDotOffset = -0.8f;

    protected override void Awake()
    {
        base.Awake();

        InitialiseVelocity();
    }

    protected override void InitialiseVelocity()
    {
        base.InitialiseVelocity();

        EntityParameters.accelerationScalar *= 1 - (Random.Range(0f, m_AccelerationMultiplier));
    }

    protected override void OnCollisionWithKilling(Vector3 direction)
    {
        base.OnCollisionWithKilling(direction);
        int numberOfAsteroidToSpawn = 0;

        switch (AsteroidType)
        {
            case AsteroidType.medium:
                numberOfAsteroidToSpawn = 2;
                break;
            case AsteroidType.big:
                numberOfAsteroidToSpawn = 1;
                break;
        }

        for (int i = 0; i < numberOfAsteroidToSpawn; i++)
        {
            Vector3 randomDirection = ExtensionMethods.RandomVector3();
            randomDirection.y = 0f;

            // Use this to make sure the asteroids won't go too much in the direction of the player ; Not present in the original version though... !
            if (m_MakeDuplicationEasier)
            {
                float dot = Vector3.Dot(randomDirection, direction);

                if (dot < m_DuplicationDotOffset)
                {
                    randomDirection = Vector3.zero;
                }

                GameManagement.Instance.SpawnAsteroid(m_AsteroidToSpawn, transform.position, direction + randomDirection);
            }
            else
            {
                GameManagement.Instance.SpawnAsteroid(m_AsteroidToSpawn, transform.position, randomDirection);
            }
        }
    }
}