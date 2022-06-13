using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GetNFT : MonoBehaviour
{
    public class Response<T> { public T response; }

    async public static Task<string[]> Colors()
    {
        string url = "https://multicall-server.vercel.app/owner?address=" + PlayerPrefs.GetString("Account");
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        await webRequest.SendWebRequest();
        Response<string[]> data = JsonUtility.FromJson<Response<string[]>>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));
        return data.response;
    }
}
