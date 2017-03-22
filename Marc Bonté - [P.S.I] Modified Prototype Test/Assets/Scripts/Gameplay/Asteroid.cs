using UnityEngine;
using System.Collections;

public enum AsteroidType
{
    small = 0,
    medium = 1,
    big = 2
};

[System.Serializable]
public class AsteroidParameters
{
    [Range(0f, 1f)]
    public float accelerationMultiplier = 1f;
    public int pointsGiven = 100;
}

public class Asteroid : Entity
{
    [SerializeField]
    protected AsteroidParameters m_AsteroidParameters;

    public AsteroidType AsteroidType { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet)
        {
            GameManagement.Instance.RemoveAsteroidFromList(this);

            OnCollisionWithKilling(bullet.EntityRigidbody.velocity.normalized);
            Destroy(bullet.gameObject);

            return;
        }

        Spaceship spaceShip = other.GetComponent<Spaceship>();
        if (spaceShip)
        {
            if (!spaceShip.IsInvincible)
            {
                spaceShip.ResetSpaceship();
            }
            return;
        }
    }

    protected virtual void InitialiseVelocity()
    {
        // Used in children
    }

    protected virtual void OnCollisionWithKilling(Vector3 direction)
    {
        GameManagement.Instance.ScoreManager.AddScore(m_AsteroidParameters.pointsGiven);

        StartCoroutine(DestroyAsteroid(transform.position));
    }

    private IEnumerator DestroyAsteroid(Vector3 particlePosition)
    {
        m_EntityParameters.ownCollider.enabled = false;
        m_EntityParameters.ownMesh.enabled = false;

        // VFX
        m_EntityEffects.explosionVFX.transform.position = particlePosition;
        m_EntityEffects.explosionVFX.Play();

        // SFX
        m_EntityEffects.audioSource.clip = m_EntityEffects.explosionSFX;
        m_EntityEffects.audioSource.Play();

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        base.OnDrawGizmos();
    }
}