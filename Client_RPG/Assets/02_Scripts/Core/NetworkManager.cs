using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    // 어디서든 쉽게 접근할 수 있게 싱글톤 패턴 적용
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // 공통 POST 요청 함수 (어떤 Handler든 이것만 부르면 됨!)
    public IEnumerator PostJson(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text); // 성공 시 답장 원본 전달
            }
            else
            {
                onError?.Invoke(request.error); // 실패 시 에러 메시지 전달
            }
        }
    }

    public IEnumerator GetJson<T>(string url, Action<T> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;

                //  JSON이 배열('[')로 시작하면 JsonHelper를 사용하여 파싱
                if (json.Trim().StartsWith("["))
                {
                    T data = JsonHelper.FromJson<T>(json);
                    onSuccess?.Invoke(data);
                }
                else
                {
                    T data = JsonUtility.FromJson<T>(json);
                    onSuccess?.Invoke(data);
                }
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}

//  배열 형태의 JSON을 파싱하기 위한 도우미 클래스
public static class JsonHelper
{
    public static T FromJson<T>(string json)
    {
        // 배열을 "Items"라는 이름의 객체로 감싸서 유니티가 읽을 수 있게 만듭니다.
        string newJson = "{ \"Items\": " + json + " }";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T Items;
    }
}
