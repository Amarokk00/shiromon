using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Shiromon : MonoBehaviour
{
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public GameObject GObtnShiro;
    public GameObject GObtnWalter;
    public GameObject GObtnEvilShiro;
    public KMSelectable btnShiro;
    public KMSelectable btnWalter;
    public KMSelectable btnEvilShiro;
    public GameObject shrio;
    public GameObject shiro;
    public GameObject evilshiro;
    public GameObject walter;

    int walterLevel = 0;
    bool isActivated = false;
    int activation = 100;
    int serialShiroCount = 0;
    int indicatorShiroCount = 0;

    void Start()
    {
        string timerText = BombInfo.GetFormattedTime();
        activation = UnityEngine.Random.Range(1,int.Parse(timerText.Substring(0,2)));
        
        GetComponent<KMBombModule>().OnActivate += ActivateModule;
        
    }
    void FixedUpdate() {
        string timerText = BombInfo.GetFormattedTime();
        if (int.Parse(timerText.Substring(0,2)) == activation){
            shrio.SetActive(true);
            isActivated = true;
            Init();
        }
    }
    void Init() {

        GObtnShiro.SetActive(true);
        GObtnWalter.SetActive(true);
        GObtnEvilShiro.SetActive(true);
    }

    void ActivateModule()
    {

        btnShiro.OnInteract += delegate () { OnPress("shiro"); return false; };
        btnWalter.OnInteract += delegate () { OnPress("walter"); return false; };
        btnEvilShiro.OnInteract += delegate () { OnPress("evilshiro"); return false; };

        string serial = BombInfo.QueryWidgets("serial",null)[0];
        Dictionary<string, string> serialDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(serial);
        serialShiroCount = lookforshiro(serialDict["serial"]);
        
        List<String> indicators = BombInfo.QueryWidgets("indicator",null);
        foreach (String indicator in indicators){
            Dictionary<string, string> indicatorDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(indicator);
            indicatorShiroCount += lookforshiro(indicatorDict["label"]);
        }

        GObtnShiro.SetActive(false);
        GObtnWalter.SetActive(false);
        GObtnEvilShiro.SetActive(false);
    }


    void OnPress(string btnName)
    {

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();

        walterLevel =  (BombInfo.QueryWidgets("waltergroup",null).Count() * 3 )
        - (BombInfo.QueryWidgets("birthdayhat",null).Count() * (indicatorShiroCount > 0 ? indicatorShiroCount : 1))
        + ((BombInfo.GetStrikes() > 0 ? BombInfo.GetStrikes() : 0)*(serialShiroCount > 0 ? 0 : 1))
        + (BombInfo.QueryWidgets("walter",null).Count()
        * (((BombInfo.GetStrikes() > 0 ? BombInfo.GetStrikes() : 0) % 2 == 0)? 1 : 0));

        bool correctButton = (btnName == "walter" && walterLevel >= 7 ) || (btnName == "shiro" && walterLevel < 7 && walterLevel > 0 ) || (btnName == "evilshiro" && walterLevel <= 0 );

        if (isActivated){
            if (correctButton)
            {
                BombModule.HandlePass();
                isActivated = false;
                activation = 100;
                shrio.SetActive(false);
                shiro.SetActive(walterLevel<7&&walterLevel>0);
                walter.SetActive(walterLevel>=7);
                evilshiro.SetActive(walterLevel<=0);
            }
            else
            {
                BombModule.HandleStrike();
            }
        } 
    }

    public int lookforshiro(string source)
    {
        int count = 0;
        foreach (char letter in "SHIRO"){
            count += CountChars(source,letter);
        }
        
        return count;
    }

    public int CountChars(string source, char toFind)
    {
        int count = 0;
        foreach (var ch in source)
        {
            if (ch == toFind)
                count++;
        }
        return count;
    }
}

