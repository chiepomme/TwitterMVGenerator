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

    string GetFilePath(string relativePathFromRoot)
    {
        if (relativePathFromRoot.StartsWith("./"))
        {
            relativePathFromRoot = relativePathFromRoot.Substring(2);
        }

        string path;
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
                path = Application.dataPath + "/../../" + relativePathFromRoot;
                break;
            default:
                path = Application.dataPath + "/../" + relativePathFromRoot;
                break;
        }

        return new FileInfo(path).FullName.Replace("\\", "/");
    }

    IEnumerator Start()
    {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            useWebMInsteadOfMp4 = true;
        }

        Screen.SetResolution(512, 512, false);

        if (File.Exists(GetFilePath("background.png")))
        {
            var imageWWW = new WWW("file://" + GetFilePath("background.png"));
            yield return imageWWW;
            image.texture = imageWWW.texture;
        }
        else if (File.Exists(GetFilePath("background.jpg")))
        {
            var imageWWW = new WWW("file://" + GetFilePath("background.jpg"));
            yield return imageWWW;
            image.texture = imageWWW.texture;
        }

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

        recorder.BeginRecording();
        yield return new WaitWhile(() => audiosource.isPlaying);
        recorder.EndRecording();

		yield return new WaitForSeconds(3f);

        if (useWebMInsteadOfMp4)
        {
            var webmPath = "\"" + GetFilePath(LastPath + ".webm") + "\"";
            var mp4Path = "\"" + GetFilePath(LastPath + ".mp4") + "\"";
            var exeExtension = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ? ".exe" : "";
            var process = Process.Start(GetFilePath("ffmpeg/ffmpeg" + exeExtension), "-i " + webmPath + " " + mp4Path);
            process.WaitForExit();
        }

        Application.Quit();
    }
}
