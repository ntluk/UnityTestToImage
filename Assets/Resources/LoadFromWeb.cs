using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadFromWeb : MonoBehaviour
{
    //public string path;
    //public void Start()
    //{
    //Texture2D t = LoadImage(path);
    //gameObject.GetComponent<RawImage>().texture = t;
    // StartCoroutine(DownloadImage(path));
    //GetRemoteTexture(path);
    // }

    [SerializeField] RawImage Image;
    [SerializeField] string _imageUrl;
   // [SerializeField] Material _material;
    Texture2D _texture;

    async void Start()
    {
        _texture = await GetRemoteTexture(_imageUrl);
        //_material.mainTexture = _texture;
        Image.texture = _texture;
    }

    void OnDestroy() => Dispose();
    public void Dispose() => Object.Destroy(_texture);// memory released, leak otherwise
    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // begin request:
            var asyncOp = www.SendWebRequest();
            
            // await until it's done: 
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz

            // read results:
            //if (www.isNetworkError || www.isHttpError)
            if( www.result!=UnityWebRequest.Result.Success )// for Unity >= 2020.1
            {
                // log error:
#if DEBUG
                Debug.Log($"{www.error}, URL:{www.url}");
#endif

                // nothing to return on error:
                return null;
            }
            else
            {
                // return valid results:
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            // ImageComponent.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;

            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            gameObject.GetComponent<Image>().overrideSprite = sprite;
        }
    }
    public static Texture2D LoadImage(string path)
    {
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            Debug.Log("Texture loaded");
            return tex;
        }
        else
        {
            Debug.Log("Texture null");
            return null;
        }
    }
    public static Sprite LoadImageAsSprite(string path)
    {
        Sprite sprite = Sprite.Create(LoadImage(path), new Rect(0.0f, 0.0f, LoadImage(path).width,
        LoadImage(path).height), new Vector2(0.5f, 0.5f), 100.0f);
        return sprite;
    }
}
