using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Collections;

public class GameplayManager : MonoBehaviour
{
    public MarioController MarioController;
	public Coin Coin;
	
    public Text TimeText;
    public Text RecordText;
	public Text CoinText;

    private float initialTime;
    private float recordTime;

    private void Start()
    {
        TimeText.text = RecordText.text = string.Empty;
		CoinText.text = "000";

        MarioController.OnKilled += RestartLevel;
        MarioController.OnReachedEndOfLevel += EndGame;
        MarioController.enabled = false;

        recordTime = PlayerPrefs.GetFloat("recordLevel" + SceneManager.GetActiveScene().buildIndex, 0);

        if (recordTime > 0)
            RecordText.text = "Record: " + recordTime.ToString("00.00") + "s";
    }
	
	private void Update()
	{
		OnGameStarted();
	}

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnGameStarted()
    {
        MarioController.enabled = true;
        initialTime = Time.time;
        TimeText.text = string.Empty;
		
		CoinText.text = Coin.CoinsCollected();
    }

    private void EndGame()
    {
        MarioController.enabled = false;
        TimeText.text = "FINAL! " + (Time.time - initialTime).ToString("00.00") + "s";

        if ((Time.time - initialTime) < recordTime)
        {
            PlayerPrefs.SetFloat("recordLevel" + SceneManager.GetActiveScene().buildIndex, (Time.time - initialTime));
            TimeText.text = "NEW RECORD! " + (Time.time - initialTime).ToString("00.00") + "s";
        }
        else
        {
            TimeText.text = "FINAL! " + (Time.time - initialTime).ToString("00.00") + "s";
        }
    }
}
