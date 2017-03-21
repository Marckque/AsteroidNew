using UnityEngine;

public class Bullet : Entity
{
    [SerializeField]
    private float m_LifeTime;
    private float m_SpawnTime;

    [SerializeField]
    private AudioSource m_AudioSource;

    protected override void Awake()
    {
        base.Awake();

        m_AudioSource = GetComponent<AudioSource>();
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

    public AudioSource AccessToAudioSource()
    {
        return m_AudioSource;
    }
}