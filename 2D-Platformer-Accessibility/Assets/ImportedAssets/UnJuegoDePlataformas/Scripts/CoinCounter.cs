using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    public TMP_Text counterText;

    // Start is called before the first frame update
    void Start()
    {
        counterText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //Set the current number of coins to display
        if(counterText.text != Coin.coins.ToString())
        {
            counterText.text = Coin.coins.ToString();
        }
    }
}