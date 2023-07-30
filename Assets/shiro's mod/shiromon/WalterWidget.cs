using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WalterWidget : MonoBehaviour
{
    public string WIDGET_QUERY_KEY = "walter";
    public GameObject sprite;

    void Awake()
    {
        float randomScale = Random.Range(0.1f,1f);
        sprite.transform.localScale = new Vector3(randomScale,1f,randomScale);
        GetComponent<KMWidget>().OnQueryRequest += GetQueryResponse;
    }

    public string GetQueryResponse(string queryKey, string queryInfo)
    {
        if(queryKey == WIDGET_QUERY_KEY)
        {
            Dictionary<string, int> response = new Dictionary<string, int>();
            string responseStr = JsonConvert.SerializeObject(response);
            return responseStr;
        }

        return "";
    }
}
