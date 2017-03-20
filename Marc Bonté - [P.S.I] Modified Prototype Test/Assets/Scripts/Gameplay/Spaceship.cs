using UnityEngine;

[System.Serializable]
public class SpaceshipControls
{
    public string rotation = "Horizontal";
    public string moveForward = "MoveForward";
    public string shoot = "Shoot";
    public string teleport;
}

[System.Serializable]
public class SpaceshipParameters
{
    public float rotationSpeed = 10f;
    public float shootOffset = 1f;
}

[System.Serializable]
public class SpaceshipEffects
{
    [Header("VFX")]
    public ParticleSystem m_DestroyedVFX;
}

public class Spaceship : Entity
{
    private const float BORDER_OFFSET = 1.5f;
    private const float SPAWN_DISTANCE_OFFSET = 2f;

    #region Variables
    [SerializeField]
    private SpaceshipControls m_Controls = new SpaceshipControls();
    [SerializeField]
    private SpaceshipParameters m_SpaceshipParameters = new SpaceshipParameters();
    [SerializeField]
    private SpaceshipEffects m_SpaceshipEffects = new SpaceshipEffects();

    [SerializeField]
    private Bullet m_Bullet;

    private float m_RotationInput;
    private Vector3 m_TargetRotation;
    
    public bool IsUsingForwardInput { get; set; }
    #endregion Variables

    protected override void Update()
    {
        base.Update();

        MoveInput();
        m_RotationInput = Input.GetAxisRaw(m_Controls.rotation);

        Teleport();
        Shoot();
    }

    protected override void FixedUpdate()
    {
        //base.FixedUpdate();

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
            //transform.rotation = newRotation;
        }
    }

    private void MoveInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            IsUsingForwardInput = true;
            SetAcceleration(transform.InverseTransformDirection(transform.forward));
        }
        else
        {
            IsUsingForwardInput = false;
        }
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Bullet bullet = Instantiate(m_Bullet, transform.position + (transform.forward * m_SpaceshipParameters.shootOffset), Quaternion.identity);

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

    public void ResetSpaceship()
    {
        m_SpaceshipEffects.m_DestroyedVFX.transform.position = transform.position;
        m_SpaceshipEffects.m_DestroyedVFX.Play();

        m_EntityRigidbody.velocity = Vector3.zero;

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

        transform.position = respawnPosition;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        base.OnDrawGizmos();
    }
}