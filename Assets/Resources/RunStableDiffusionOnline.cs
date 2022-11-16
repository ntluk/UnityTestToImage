using UnityEngine;
using System.IO;
using System.Diagnostics;
using UnityEngine.UI;
using System.Threading;


public class RunStableDiffusionOnline : MonoBehaviour
{  
    [SerializeField] InputField input;
    [SerializeField] string folderPath;
    [SerializeField] GameObject brush;
    Texture mainTex;
    RawImage rawImage;

    public Process process;
    public StreamWriter streamWriter;
    private Thread thread;
    private string prompt, path;

    public void Init()
    {
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        streamWriter = process.StandardInput;
        prompt = input.text;

        thread = new Thread(
          new ThreadStart(StartAI));
        thread.Start();

        process.WaitForExit(1000 * 60 * 1);
        setImage();
    }

    public void StartAI()
    {
        UnityEngine.Debug.Log("Writing: " + $"cd Selenium");
        streamWriter.WriteLine($"cd C:/UnityTextToImage/Assets/Selenium");

        UnityEngine.Debug.Log("Writing: " + "run_stablediffusion_online.py");
        streamWriter.WriteLine($"py run_stablediffusion_online.py --prompt " + "\"" + prompt + "\"");
    }

    public void setImage()
    {
        path = folderPath + prompt + ".png";
        mainTex = LoadImage(path);
        rawImage = GetComponent<RawImage>();
        rawImage.texture = mainTex;
        brush.SetActive(false);
    }
    public static Texture2D LoadImage(string path)
    {
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            UnityEngine.Debug.Log("Texture loaded");
            return tex;
        }
        else
        {
            UnityEngine.Debug.Log("Texture null");
            return null;
        }
    }

   
}
