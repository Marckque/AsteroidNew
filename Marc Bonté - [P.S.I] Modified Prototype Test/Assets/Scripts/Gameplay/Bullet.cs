using UnityEngine;

public class Bullet : Entity
{
    [SerializeField]
    private float m_LifeTime;
    private float m_SpawnTime;

    protected override void Awake()
    {
        base.Awake();

        m_SpawnTime = Time.time;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time > m_SpawnTime + m_LifeTime)
        {
            Destroy(gameObject);
        }
    }
}