using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameParameters
{
    [Header("Asteroids")]
    public Asteroid smallAsteroid;
    public Asteroid mediumAsteroid;
    public Asteroid bigAsteroid;
    public int maximumAsteroids = 1;
    public int numberOfAsteroidsToSpawn = 3;
    public float delayBetweenSpawn = 2f;
    public int minimumAsteroidSize = 1;
    public int maximumAsteroidSize = 3;
}

[RequireComponent(typeof(ScoreManager))]
public class GameManagement : MonoBehaviour
{
    #region Variables
    private static GameManagement m_Instance;
    public static GameManagement Instance { get { return m_Instance; } }

    private ScoreManager m_ScoreManager;
    public ScoreManager ScoreManager { get { return m_ScoreManager; } }

    [SerializeField]
    private GameParameters m_GameParameters;

    private int m_NumberOfSpawnedAsteroids;
    private List<Asteroid> m_Asteroids = new List<Asteroid>();
    #endregion Variables

    protected void Awake()
    {
        InitialiseGameManager();
        InitialiseScoreManager();
        StartCoroutine(PeriodicAsteroidSpawn());
    }

    #region Initialisers
    private void InitialiseGameManager()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_Instance = this;
        }

        if (m_Instance == null || m_Instance != this)
        {
            Debug.LogError("Error with the initialisation of the GameManagement");
        }
    }

    private void InitialiseScoreManager()
    {
        m_ScoreManager = GetComponent<ScoreManager>();
    }
    #endregion Initialisers

    private IEnumerator PeriodicAsteroidSpawn()
    {
        while (m_NumberOfSpawnedAsteroids < m_GameParameters.numberOfAsteroidsToSpawn)
        {
            if (m_Asteroids.Count < m_GameParameters.maximumAsteroids)
            {
                SpawnAsteroid(AsteroidType.big, Vector3.zero, ExtensionMethods.RandomVector3());
            }

            yield return new WaitForSeconds(m_GameParameters.delayBetweenSpawn);
        }
    }

    public void SpawnAsteroid(AsteroidType type, Vector3 spawnPosition, Vector3 direction)
    {
        Asteroid asteroidToSpawn = null;

        switch (type)
        {
            case AsteroidType.small:
                asteroidToSpawn = m_GameParameters.smallAsteroid;
                break;
            case AsteroidType.medium:
                asteroidToSpawn = m_GameParameters.mediumAsteroid;
                m_NumberOfSpawnedAsteroids++;
                break;
            case AsteroidType.big:
                asteroidToSpawn = m_GameParameters.bigAsteroid;
                m_NumberOfSpawnedAsteroids++;
                break;
            default:
                break;
        }

        Asteroid asteroid = Instantiate(asteroidToSpawn, spawnPosition, Quaternion.identity);
        asteroid.SetAcceleration(direction * asteroid.EntityParameters.accelerationScalar);
        asteroid.ApplyForces(false);

        asteroid.AsteroidType = type;

        m_Asteroids.Add(asteroid);
    }

    public void RemoveAsteroidFromList(Asteroid asteroidToRemove)
    {
        m_Asteroids.Remove(asteroidToRemove);
    }
}