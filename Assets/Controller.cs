using SFB;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UTJ.FrameCapturer;

public class Controller : MonoBehaviour
{
    public static string LastPath { get; set; }

    [SerializeField]
    MovieRecorder mp4Recorder;
    [SerializeField]
    MovieRecorder webMRecorder;
    [SerializeField]
    RawImage image;

    bool useWebMInsteadOfMp4;

    string rootDirectory;

    string GetFilePath(string relativePathFromRoot)
    {
        if (relativePathFromRoot.StartsWith("./"))
        {
            relativePathFromRoot = relativePathFromRoot.Substring(2);
        }

        return new FileInfo(rootDirectory + relativePathFromRoot).FullName.Replace("\\", "/");
    }

    IEnumerator Start()
    {
        rootDirectory = Application.dataPath + "/../";

        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            useWebMInsteadOfMp4 = true;
        }

        Screen.SetResolution(512, 512, false);

        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            // OSX の場合にはアプリが別のパスに隔離されてしまうので問い合わせが必要
            var dir = PlayerPrefs.HasKey("RootDirectory") ? PlayerPrefs.GetString("RootDirectory") : "";
            var paths = StandaloneFileBrowser.OpenFilePanel("audio.wav を選択してください", dir, "wav", false);
            if (paths.Length == 0)
            {
                Application.Quit();
                yield break;
            }

            rootDirectory = Uri.UnescapeDataString(Path.GetDirectoryName(paths[0]).Replace("file:", "") + "/");
            PlayerPrefs.SetString("RootDirectory", rootDirectory);
        }

        print("rootDir:" + rootDirectory);

        if (File.Exists(GetFilePath("background.png")))
        {
            print(GetFilePath("background.png"));
            var imageWWW = new WWW("file://" + GetFilePath("background.png"));
            yield return imageWWW;
            image.texture = imageWWW.texture;
        }
        else if (File.Exists(GetFilePath("background.jpg")))
        {
            print(GetFilePath("background.jpg"));
            var imageWWW = new WWW("file://" + GetFilePath("background.jpg"));
            yield return imageWWW;
            image.texture = imageWWW.texture;
        }

        print(GetFilePath("audio.wav"));
        var audiosource = gameObject.AddComponent<AudioSource>();
        var audioWWW = new WWW("file://" + GetFilePath("audio.wav"));
        yield return audioWWW;
        audiosource.clip = audioWWW.GetAudioClip(false);
        audiosource.Play();

        var recorder = mp4Recorder;

        if (useWebMInsteadOfMp4)
        {
            recorder = webMRecorder;
        }

        recorder.outputDir = new DataPath(GetFilePath("Movie"));

        recorder.BeginRecording();
        print(LastPath);
        yield return new WaitWhile(() => audiosource.isPlaying);
        recorder.EndRecording();

        yield return new WaitForSeconds(2f);

        if (useWebMInsteadOfMp4)
        {
            var webmPath = "\"" + GetFilePath(LastPath + ".webm") + "\"";
            var mp4Path = "\"" + GetFilePath(LastPath + ".mp4") + "\"";
            var exeExtension = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ? ".exe" : "";
            var process = Process.Start(GetFilePath("ffmpeg/ffmpeg" + exeExtension), "-i " + webmPath + " " + mp4Path);
            process.WaitForExit();
            File.Delete(GetFilePath(LastPath + ".webm"));
        }

        Application.Quit();
    }
}
