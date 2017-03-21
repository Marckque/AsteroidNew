using UnityEngine;

public class SmallAsteroid : Asteroid
{
    protected override void Awake()
    {
        base.Awake();

        InitialiseVelocity();
    }

    protected override void InitialiseVelocity()
    {
        base.InitialiseVelocity();
        EntityParameters.accelerationScalar *= 1 + (Random.Range(0f, m_AsteroidParameters.accelerationMultiplier));
    }

    protected override void OnCollisionWithKilling(Vector3 direction)
    {
        base.OnCollisionWithKilling(direction);
    }
}