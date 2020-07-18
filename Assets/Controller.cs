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

    GifPlayer gifPlayer;

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

        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            rootDirectory = new DirectoryInfo(Application.dataPath).Parent.Parent + "/";
        }


        print("rootDir:" + rootDirectory);

        if (File.Exists(GetFilePath("background.png")))
        {
            print(GetFilePath("background.png"));
            var imageWWW = new WWW("file://" + GetFilePath("background.png"));
            yield return imageWWW;
            image.texture = imageWWW.texture;
            SetResolution(imageWWW.texture.width, imageWWW.texture.height);
        }
        else if (File.Exists(GetFilePath("background.jpg")))
        {
            print(GetFilePath("background.jpg"));
            var imageWWW = new WWW("file://" + GetFilePath("background.jpg"));
            yield return imageWWW;
            image.texture = imageWWW.texture;
            SetResolution(imageWWW.texture.width, imageWWW.texture.height);
        }
        else if (File.Exists(GetFilePath("background.gif")))
        {
            print(GetFilePath("background.gif"));
            var imageWWW = new WWW("file://" + GetFilePath("background.gif"));
            yield return imageWWW;
            yield return StartCoroutine(UniGif.GetTextureListCoroutine(imageWWW.bytes, (gifTexList, loopCount, width, height) =>
            {
                SetResolution(width, height);
                gifPlayer = GifPlayer.Create(image, gifTexList);
            }));
        }
        else
        {
            SetResolution(720, 304);
        }

        print(GetFilePath("audio.wav"));
        var audiosource = gameObject.AddComponent<AudioSource>();
        var audioWWW = new WWW("file://" + GetFilePath("audio.wav"));
        yield return audioWWW;
        audiosource.clip = audioWWW.GetAudioClip(false);

        var recorder = mp4Recorder;

        if (useWebMInsteadOfMp4)
        {
            recorder = webMRecorder;
        }

        recorder.outputDir = new DataPath(GetFilePath("Movie"));

        recorder.BeginRecording();
        if (gifPlayer != null)
        {
            gifPlayer.PlaySyncWith(audiosource);
        }
        audiosource.Play();
        print(LastPath);
        yield return new WaitWhile(() => audiosource.isPlaying);
        recorder.EndRecording();

        yield return new WaitForSeconds(2f);

        if (useWebMInsteadOfMp4)
        {
            var webmPath = "\"" + LastPath + ".webm" + "\"";
            var mp4Path = "\"" + LastPath + ".mp4" + "\"";
            var exeExtension = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ? ".exe" : "";
            var process = Process.Start(GetFilePath("ffmpeg/ffmpeg" + exeExtension), "-i " + webmPath + " " + mp4Path);
            process.WaitForExit();
            File.Delete(LastPath + ".webm");
        }

        Application.Quit();
    }

    void SetResolution(int width, int height)
    {
        const int MinimumResolution = 720;
        if (width < MinimumResolution || height < MinimumResolution)
        {
            var scale = (float)MinimumResolution / Mathf.Min(width, height);
            width = Mathf.RoundToInt(width * scale);
            height = Mathf.RoundToInt(height * scale);
        }

        Screen.SetResolution(width, height, false);
    }
}
