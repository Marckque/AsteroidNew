using UnityEngine;
using System.Collections;

[System.Serializable] // !!! USE THESE CONTROLS !!!
public class SpaceshipControls
{
    public string rotation = "Horizontal";
    public string moveForward = "MoveForward";
    public string shoot = "Shoot";
    public string teleport = "Teleport";
}

[System.Serializable]
public class SpaceshipParameters
{
    [Range(1f, 100f)]
    public float maxMagnitudeMultiplier = 1f;
    public float rotationSpeed = 10f;
    public float shootOffset = 1f;
    public float respawnTime = 1f;
    public float invincibilityDuration = 1f;
    public float shootCooldown = 0.1f;
}

[System.Serializable]
public class SpaceshipEffects
{
    [Header("SFX")]
    public AudioClip[] bulletSounds;
}

public class Spaceship : Entity
{
    private const float BORDER_OFFSET = 1.5f;
    private const float SPAWN_DISTANCE_OFFSET = 2f;
    private const float BLINK_DURATION = 5f;

    #region Variables
    [SerializeField]
    private SpaceshipControls m_Controls = new SpaceshipControls();
    [SerializeField]
    private SpaceshipParameters m_SpaceshipParameters = new SpaceshipParameters();
    [SerializeField]
    private SpaceshipEffects m_SpaceshipEffects = new SpaceshipEffects();

    [SerializeField]
    private Bullet m_Bullet;

    private Transform m_Graphics;
    private float m_RotationInput;
    private Vector3 m_TargetRotation;
    private float m_DefaultDrag;
    private float m_IdleDrag;
    private float m_LastShootTime;

    private int m_BulletSoundIndex;

    private CapsuleCollider m_CapsuleCollider;

    private bool m_IsAlive = true;
    public bool IsUsingForwardInput { get; set; }
    public bool IsInvincible { get; set; }
    #endregion Variables

    protected void Start()
    {
        m_Graphics = transform.GetChild(0);
        m_CapsuleCollider = GetComponent<CapsuleCollider>();

        m_DefaultDrag = m_EntityRigidbody.drag;
        m_IdleDrag = m_DefaultDrag * m_SpaceshipParameters.maxMagnitudeMultiplier;
    }

    protected override void Update()
    {
        base.Update();

        if (m_IsAlive)
        {
            MoveInput();
            m_RotationInput = Input.GetAxisRaw(m_Controls.rotation);

            Teleport();
            Shoot();
        }
    }

    protected override void FixedUpdate()
    {
        RotateSpaceship();
        ApplyForces(true);
    }

    private void RotateSpaceship()
    {
        if (m_RotationInput != 0)
        {
            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + (Mathf.Sign(m_RotationInput) * (transform.up * m_SpaceshipParameters.rotationSpeed)));
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, m_SpaceshipParameters.rotationSpeed * Time.deltaTime);

            m_EntityRigidbody.MoveRotation(newRotation);
        }
    }

    private void MoveInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_EntityRigidbody.drag = m_DefaultDrag;
            IsUsingForwardInput = true;
            SetAcceleration(transform.InverseTransformDirection(transform.forward));
        }
        else
        {
            m_EntityRigidbody.drag = m_IdleDrag;
            IsUsingForwardInput = false;
        }
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && m_LastShootTime + m_SpaceshipParameters.shootCooldown < Time.time)
        {
            m_LastShootTime = Time.time;
            Bullet bullet = Instantiate(m_Bullet, transform.position + (transform.forward * m_SpaceshipParameters.shootOffset), Quaternion.identity);

            // SFX
            bullet.AccessToAudioSource().clip = m_SpaceshipEffects.bulletSounds[m_BulletSoundIndex];
            bullet.AccessToAudioSource().Play();
            if (m_BulletSoundIndex < m_SpaceshipEffects.bulletSounds.Length - 1)
            {
                m_BulletSoundIndex++;
            }
            else
            {
                m_BulletSoundIndex = 0;
            }

            // Bullet physics
            bullet.SetAcceleration(transform.forward * bullet.EntityParameters.accelerationScalar);
            bullet.ApplyForces(false);
        }
    }

    private void Teleport()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3 newPosition = ExtensionMethods.RandomVector3();

            // Makes sure we respawn within the screen with a border offset
            newPosition.x *= (BORDER_OFFSET + (Camera.main.orthographicSize - BORDER_OFFSET * 2f));
            newPosition.z *= (BORDER_OFFSET + (Camera.main.orthographicSize - BORDER_OFFSET * 2f));

            transform.position = newPosition;
        }
    }

    // Call this when spaceship gets destroyed
    public void ResetSpaceship()
    {
        // VFX
        m_EntityEffects.explosionVFX.transform.position = transform.position;
        m_EntityEffects.explosionVFX.Play();

        // SFX
        m_EntityEffects.audioSource.clip = m_EntityEffects.explosionSFX;
        m_EntityEffects.audioSource.Play();

        // Velocity
        m_EntityRigidbody.velocity = Vector3.zero;

        // Position
        Vector3 respawnPosition = Vector3.zero;
        float distanceToSpaceship = 0f;
        int numOfIteration = 0;

        while (distanceToSpaceship > SPAWN_DISTANCE_OFFSET)
        {
            respawnPosition = ExtensionMethods.RandomVector3() * Camera.main.orthographicSize;

            if (numOfIteration > 3)
            {
                foreach (Asteroid asteroid in GameManagement.Instance.CurrentAsteroids)
                {
                    distanceToSpaceship = (asteroid.transform.position - transform.position).magnitude;

                    if (distanceToSpaceship > SPAWN_DISTANCE_OFFSET * 0.5f)
                    {
                        break;
                    }
                }
            }
            else
            {
                foreach (Asteroid asteroid in GameManagement.Instance.CurrentAsteroids)
                {
                    distanceToSpaceship = (asteroid.transform.position - transform.position).magnitude;

                    if (distanceToSpaceship > SPAWN_DISTANCE_OFFSET)
                    {
                        break;
                    }
                }
            }
            
            numOfIteration++;
        }

        StartCoroutine(RespawnDelay(respawnPosition));
    }

    private IEnumerator RespawnDelay(Vector3 respawnPosition)
    {
        m_IsAlive = false;
        m_CapsuleCollider.enabled = false;
        m_Graphics.gameObject.SetActive(false);

        yield return new WaitForSeconds(m_SpaceshipParameters.respawnTime);

        transform.position = respawnPosition;

        m_Graphics.gameObject.SetActive(true);
        m_CapsuleCollider.enabled = true;
        m_IsAlive = true;

        StartCoroutine(TemporaryInvincibility());
    }

    private IEnumerator TemporaryInvincibility()
    {
        IsInvincible = true;
        float elapsedTime = 0f;

        while (elapsedTime < m_SpaceshipParameters.invincibilityDuration)
        {
            if (m_Graphics.gameObject.activeInHierarchy)
            {
                m_Graphics.gameObject.SetActive(false);
            }
            else
            {
                m_Graphics.gameObject.SetActive(true);
            }

            elapsedTime += Time.deltaTime * BLINK_DURATION;

            yield return new WaitForSeconds(Time.deltaTime * BLINK_DURATION);
        }

        m_Graphics.gameObject.SetActive(true);
        IsInvincible = false;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        base.OnDrawGizmos();
    }
}