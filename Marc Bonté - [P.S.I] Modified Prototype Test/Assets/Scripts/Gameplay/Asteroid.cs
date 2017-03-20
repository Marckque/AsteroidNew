using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AsteroidType
{
    small = 0,
    medium = 1,
    big = 2
};

[System.Serializable]
public class AsteroidEffects
{
    [Header("VFX")]
    public ParticleSystem m_DestroyedVFX;
}

public class Asteroid : Entity
{
    [SerializeField]
    private AsteroidEffects m_AsteroidEffects;
    [SerializeField, Range(0f, 1f)]
    protected float m_AccelerationMultiplier = 1f;
    [SerializeField]
    private int m_Points = 100;

    private List<Asteroid> m_OtherAsteroidsInRange = new List<Asteroid>();
    private Transform m_Graphics;
    private BoxCollider m_BoxCollider;

    public AsteroidType AsteroidType { get; set; }

    protected override void Awake()
    {
        base.Awake();

        m_Graphics = transform.GetChild(0);
        m_BoxCollider = GetComponent<BoxCollider>();
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
            OnCollisionWithBullet(bullet.EntityRigidbody.velocity.normalized);
            Destroy(bullet.gameObject);

            GameManagement.Instance.RemoveAsteroidFromList(this);

            return;
        }

        Spaceship spaceShip = other.GetComponent<Spaceship>();
        if (spaceShip)
        {
            spaceShip.ResetSpaceship();

            return;
        }

        Asteroid asteroid = other.GetComponent<Asteroid>();
        if (asteroid)
        {
            m_OtherAsteroidsInRange.Add(asteroid);
            return;
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        Asteroid asteroid = other.GetComponent<Asteroid>();

        if (asteroid)
        {
            if (m_OtherAsteroidsInRange.Contains(asteroid))
            {
                m_OtherAsteroidsInRange.Remove(asteroid);
            }
        }
    }

    protected virtual void InitialiseVelocity()
    {
        // Use in child
    }

    protected virtual void OnCollisionWithBullet(Vector3 direction)
    {
        GameManagement.Instance.ScoreManager.AddScore(m_Points);

        StartCoroutine(DestroyAsteroid(transform.position));
    }

    private IEnumerator DestroyAsteroid(Vector3 particlePosition)
    {
        m_BoxCollider.enabled = false;
        m_Graphics.gameObject.SetActive(false);

        m_AsteroidEffects.m_DestroyedVFX.transform.position = particlePosition;
        m_AsteroidEffects.m_DestroyedVFX.Play();

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        base.OnDrawGizmos();
    }
}