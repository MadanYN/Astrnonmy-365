using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using LitJson;


public class NASAimage : MonoBehaviour
{
    private const string NASA_URL = "https://api.nasa.gov/planetary/apod?";
    private const string NASA_API_KEY = "api_key=tcGnfgsNUys7Wy2n64qVKGFCpWzsTVxnV0gbitb2";
    private const string TTS_URL = "http://api.voicerss.org/?";
    private const string TTS_API_KEY = "key=27aed633ec454662a91aa73b34a3fe9d";

    private Text explanation;
    private Text title;
    private Text date;
    private RawImage image;
    public AudioSource audioSource;

    private bool play = false;

    private string webData;
    private JsonData json;

    private string url;
    private string toSpeech;

    private void Start()
    {
        title = GameObject.Find("title").GetComponent<Text>();
        date = GameObject.Find("date").GetComponent<Text>();
        explanation = GameObject.Find("ExplanationText").GetComponent<Text>();
        image = GameObject.Find("RawImage").GetComponent<RawImage>();
        //audioSource = AudioSource.Find("AudioSource");
        GameObject.Find("AudioPlayButton").GetComponent<Button>().onClick.AddListener(getSpeech);
        getData();
    }

    void getData() => StartCoroutine(OnResponse());

    private IEnumerator OnResponse()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(NASA_URL + NASA_API_KEY))
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
        toSpeech = json["title"].ToString()+" . "+json["explanation"].ToString();

        string temp = json["date"].ToString();
        date.text = temp.Substring(8) + "-" + temp[5] + temp[6] + "-" + temp[0] + temp[1] + temp[2] + temp[3];
        //date.text = temp;

        url = json["url"].ToString();
        StartCoroutine(renderImage());
    }
    private IEnumerator renderImage()
    {
        /*WWW imageLoad = new WWW(url);
        yield return imageLoad;

        image.texture = imageLoad.texture;*/

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            image.texture = DownloadHandlerTexture.GetContent(request);
        }
    }
    void getSpeech()
    {
        play = !play;
        StartCoroutine(TextToSpeech());
    } 

    private IEnumerator TextToSpeech()
    {
        string details = "&hl=en-us&v=Linda&src=Image of the day:  "+toSpeech;
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(TTS_URL + TTS_API_KEY + details, AudioType.WAV))
        {
            yield return request.SendWebRequest();
            audioSource.clip = DownloadHandlerAudioClip.GetContent(request);
            if (play)
            {
                audioSource.Play();
            }
            else
                audioSource.Stop();
        }
    }

}