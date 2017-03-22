using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameParameters
{
    [Header("Spaceship")]
    public Spaceship spaceship;

    [Header("Asteroids")]
    public Transform asteroidSpawnParent;
    public Asteroid smallAsteroid;
    public Asteroid mediumAsteroid;
    public Asteroid bigAsteroid;
    public int maximumAsteroidsOnScreen = 1;
    public int numberOfAsteroidsToSpawn = 3;
    public float delayBetweenSpawns = 2f;

    [Range(1f, 3f)]
    public float minimumSpawnDelay = 1.5f;

    [Tooltip("0 → the spawn rate will be constant ; X → Will decrease by X the time between each asteroid spawn ; Fastest spawn rate is set to minimumSpawnDelay")]
    public float decreaseDelayBetweenSpawnsByAmount = 0f;

    [Header("UI")]
    public UIManager managerUI;
}

[RequireComponent(typeof(ScoreManager))]
public class GameManagement : MonoBehaviour
{
    const float SPAWN_DISTANCE_OFFSET = 1.75f;

    #region Variables
    private static GameManagement m_Instance;
    public static GameManagement Instance { get { return m_Instance; } }

    private ScoreManager m_ScoreManager;
    public ScoreManager ScoreManager { get { return m_ScoreManager; } }

    [SerializeField]
    private GameParameters m_GameParameters;

    private int m_NumberOfSpawnedAsteroids;

    private List<Asteroid> m_Asteroids = new List<Asteroid>();
    public List<Asteroid> CurrentAsteroids { get { return m_Asteroids; } }
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

    // Reload scene
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator PeriodicAsteroidSpawn()
    {
        while (m_NumberOfSpawnedAsteroids < m_GameParameters.numberOfAsteroidsToSpawn)
        {
            if (m_Asteroids.Count < m_GameParameters.maximumAsteroidsOnScreen)
            {
                // Defines a spawn position that is away from the player
                Vector3 spawnPosition = Vector3.zero;
                float distanceToSpaceship = 0f;

                while (distanceToSpaceship < SPAWN_DISTANCE_OFFSET)
                {
                    spawnPosition = ExtensionMethods.RandomVector3() * Camera.main.orthographicSize;
                    distanceToSpaceship = (spawnPosition - m_GameParameters.spaceship.transform.position).magnitude;
                }

                SpawnAsteroid(AsteroidType.big, spawnPosition, ExtensionMethods.RandomVector3());
            }

            yield return new WaitForSeconds(m_GameParameters.delayBetweenSpawns);

            // Increases spawn rate after each spawn (if wanted)
            if (m_GameParameters.delayBetweenSpawns > m_GameParameters.minimumSpawnDelay)
            {
                m_GameParameters.delayBetweenSpawns -= m_GameParameters.decreaseDelayBetweenSpawnsByAmount;
            }
            else
            {
                m_GameParameters.delayBetweenSpawns = m_GameParameters.minimumSpawnDelay;
            }
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
        asteroid.transform.SetParent(m_GameParameters.asteroidSpawnParent); // Avois the hierarchy to be filled with asteroids gameObjects
        asteroid.SetAcceleration(direction * asteroid.EntityParameters.accelerationScalar);
        asteroid.ApplyForces(false);

        asteroid.AsteroidType = type;

        m_Asteroids.Add(asteroid); // Add to the list of asteroids
    }

    public void RemoveAsteroidFromList(Asteroid asteroidToRemove)
    {
        m_Asteroids.Remove(asteroidToRemove);
    }
}