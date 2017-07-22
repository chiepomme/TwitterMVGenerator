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

    IEnumerator Start()
    {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            useWebMInsteadOfMp4 = true;
        }

        Screen.SetResolution(512, 512, false);

        if (File.Exists("background.png"))
        {
            var imageWWW = new WWW("file://" + Application.dataPath + "/../background.png");
            yield return imageWWW;
            image.texture = imageWWW.texture;
        }
        else if (File.Exists("background.jpg"))
        {
            var imageWWW = new WWW("file://" + Application.dataPath + "/../background.jpg");
            yield return imageWWW;
            image.texture = imageWWW.texture;
        }

        var audiosource = gameObject.AddComponent<AudioSource>();
        var audioWWW = new WWW("file://" + Application.dataPath + "/../audio.wav");
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

        if (useWebMInsteadOfMp4)
        {
            var webmPath = "\"" + LastPath + ".webm\"";
            var mp4Path = "\"" + LastPath + ".mp4\"";
            var exeExtension = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ? ".exe" : "";
            var process = Process.Start(Application.dataPath + "/../ffmpeg/ffmpeg" + exeExtension, "-i " + webmPath + " " + mp4Path);
            process.WaitForExit();
        }

        Application.Quit();
    }
}
