using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using LitJson;


public class NASAimage : MonoBehaviour
{
    private const string URL = "https://api.nasa.gov/planetary/apod?";
    private const string API_KEY = "api_key=tcGnfgsNUys7Wy2n64qVKGFCpWzsTVxnV0gbitb2";

    private Text explanation;
    private Text title;
    private Text date;
    private Renderer rend;

    private string webData;
    private JsonData json;

    private string url;

    private void Start()
    {
        title = GameObject.Find("title").GetComponent<Text>();
        date = GameObject.Find("date").GetComponent<Text>();
        explanation = GameObject.Find("explanation").GetComponent<Text>();
        rend = GameObject.Find("image").GetComponent<Renderer>();
        GameObject.Find("Button").GetComponent<Button>().onClick.AddListener(getData);
    }

    void getData() => StartCoroutine(OnResponse());

    private IEnumerator OnResponse()
    {
        using(UnityWebRequest request = UnityWebRequest.Get(URL + API_KEY))
        {
            yield return request.SendWebRequest();
            webData = request.downloadHandler.text;
        }
        json = JsonMapper.ToObject(webData);

        results();

        
    }
  
    void results()
    {
        //showing output
        title.text = json["title"].ToString();
        explanation.text = json["explanation"].ToString();
        date.text = json["date"].ToString();

        url = json["url"].ToString();
        StartCoroutine(renderImage());
    }
    private IEnumerator renderImage()
    {
        /*using(UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            renderer.material.mainTexture = request.downloadHandler.texture;
        }*/
        WWW imageLoad = new WWW(url);
        yield return imageLoad;

        rend.material.mainTexture = imageLoad.texture;
    }
}
