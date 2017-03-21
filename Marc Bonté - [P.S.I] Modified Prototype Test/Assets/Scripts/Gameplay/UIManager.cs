using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private const float HIDE_RESTART_TEXT_DELAY = 2.5f;

    [SerializeField]
    private Text m_RestartText;

	protected void Start()
    {
        StartCoroutine(HideDisplayTextAfterDelay());
	}
	
    private IEnumerator HideDisplayTextAfterDelay()
    {
        yield return new WaitForSeconds(HIDE_RESTART_TEXT_DELAY);
        HideRestartText();
    }

	public void DisplayRestartText()
    {
        m_RestartText.enabled = true;
    }

    public void HideRestartText()
    {
        m_RestartText.enabled = false;
    }
}
