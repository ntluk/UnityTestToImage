using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Threading;

public class MinDalleTest : MonoBehaviour
{
    [SerializeField] InputField input;
    [SerializeField] RawImage image;
    [SerializeField] GameObject brush;
    public Process process;
    public StreamWriter streamWriter;
    private Thread thread;

    private List<string> liLines = new List<string>();
    private List<string> liErrors = new List<string>();
    private OutputStatus outputStatus = OutputStatus.Unfinished;
    private enum OutputStatus { Unfinished, Broken, BrokenNeedsRestart, FinishedSuccessfully }
    private string prompt;

    public void Start()
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

        outputStatus = OutputStatus.Unfinished;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += OnOutputDataReceived;
        process.ErrorDataReceived += OnOutputErrorReceived;
       
        process.Start();
        process.BeginOutputReadLine();

        streamWriter = process.StandardInput;
        if (streamWriter.BaseStream.CanWrite)
        {
            thread = new Thread(
                new ThreadStart(StartAI));
            thread.Start();
        } 
    }
   
    public void StartAI()
    {
        UnityEngine.Debug.Log("Writing: " + $"cd texttoimage");
        streamWriter.WriteLine($"cd C:/TextToImage/texttoimage/min-dalle-main");
    }

    public void Go()
    {
        brush.SetActive(true);
        outputStatus = OutputStatus.Unfinished;
        StartCoroutine(ieRequestImage());
    }

    private IEnumerator ieRequestImage()
    {
        prompt = input.text;
       
        yield return new WaitUntil(() => streamWriter.BaseStream.CanWrite);
        UnityEngine.Debug.Log("Writing: " + "python image_from_text.py");
        streamWriter.WriteLine($"py image_from_text.py --text " + "\"" + prompt + "\"");

        yield return new WaitUntil(() => outputStatus != OutputStatus.Unfinished);

        if (outputStatus == OutputStatus.Broken || outputStatus == OutputStatus.BrokenNeedsRestart)
        {
            UnityEngine.Debug.Log("Could not generate image. Please use a smaller image.");
        }
        else
        {
          
        FileInfo fileLatestPng = new DirectoryInfo("C:/TextToImage/texttoimage/min-dalle-main").GetFiles().Where(x => Path.GetExtension(x.Name) == ".png").OrderByDescending(f => f.LastWriteTime).First();

            UnityEngine.Debug.Log($" New file appeared! Loading {fileLatestPng.Name}");

            yield return new WaitUntil(() => !Utility.IsFileLocked(fileLatestPng));
            yield return new WaitForSeconds(0.1f); // just give it some time

            UnityEngine.Debug.Log($"Finished loading image.");

            image.texture = Utility.texLoadImageSecure(fileLatestPng.FullName, image.texture as Texture2D);
            brush.SetActive(false);
        }
    }
    private void ProcessOutput(string _strOutput)
    {
        UnityEngine.Debug.Log(">>>>>>>> " + _strOutput);


        if (outputStatus == OutputStatus.Unfinished && _strOutput.StartsWith("image saved"))
            outputStatus = OutputStatus.FinishedSuccessfully;
    }

    private void Update()
    {
        foreach (string strLine in liLines)
            ProcessOutput(strLine);
        liLines.Clear();

        foreach (string strError in liErrors)
            UnityEngine.Debug.Log(strError);
        liErrors.Clear();
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data))
            return;
        liLines.Add(e.Data);
    }

    private void OnOutputErrorReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data))
            return;
        liErrors.Add(e.Data);
    }

}
