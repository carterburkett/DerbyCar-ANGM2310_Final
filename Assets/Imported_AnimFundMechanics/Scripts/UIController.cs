using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Text collectibleTextUI = null;
    [SerializeField] Text winTextUI = null;
    [SerializeField] Text lapTextUI = null;
    GameObject finishLineObj;



    void Start()
    {
        finishLineObj = GameObject.FindGameObjectWithTag("FinishLine");
        
        FinishLine finishLine = finishLineObj.GetComponent<FinishLine>();
        int lapsToWin = finishLine.lapsToWin;

        if (lapsToWin != 0) {
            string lapTextOut = "0/" + lapsToWin.ToString();
            lapTextUI.text = lapTextOut;
        }
        HideWinText();
    }

    public void HideWinText()
    {
        winTextUI.text = "";
        winTextUI.gameObject.SetActive(false);
    }

    public void ShowWinText(string textToShow)
    {
        winTextUI.text = textToShow;
        winTextUI.gameObject.SetActive(true);
    }

    public void UpdateCollectibleCount(int collectibleCount)
    {
        // add animation here
        collectibleTextUI.text = collectibleCount.ToString();
    }
    public void UpdateLapCount(int lapCount) {
        // add animation here
        //@TODO make the "3" changeable in the GUI
        FinishLine finishLine = finishLineObj.GetComponent<FinishLine>();
        int lapsToWin = finishLine.lapsToWin;


        if (lapsToWin != 0){
            string lapTextOut = lapCount.ToString() + "/" + lapsToWin.ToString();
            lapTextUI.text = lapTextOut;
        }
        else{ lapTextUI.text = lapCount.ToString(); }
    }
}
