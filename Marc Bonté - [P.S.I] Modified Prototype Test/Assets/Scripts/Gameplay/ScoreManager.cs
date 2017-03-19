using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private Text m_ScoreText;

    public int Score { get; set; }

    public void AddScore(int scoreToAdd)
    {
        Score += scoreToAdd;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        m_ScoreText.text = Score.ToString();
    }
}