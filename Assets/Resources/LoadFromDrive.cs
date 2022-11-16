using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading;

public class LoadFromDrive : MonoBehaviour
{
    [SerializeField] string folderPath;
    [SerializeField] InputField input;
    Renderer m_Renderer;
    Texture mainTex;
    RawImage m_RawImage;
    Thread thread;
    string path, prompt;


    public void Start()
    {
        prompt = input.text;
        path = folderPath + prompt + ".png";
        mainTex = LoadImage(path);
        //Fetch the Renderer from the GameObject
        //m_Renderer = GetComponent<Renderer>();

        //Set the Texture you assign in the Inspector as the main texture (Or Albedo)
        //m_Renderer.material.SetTexture("_MainTex", mainTex);
        m_RawImage = GetComponent<RawImage>();
        m_RawImage.texture = mainTex;
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

    public void ReadPrompt(string s)
    {
        prompt = s;
    }
}
