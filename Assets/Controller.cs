using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UTJ.FrameCapturer;

public class Controller : MonoBehaviour
{
    [SerializeField]
    MovieRecorder recorder;
    [SerializeField]
    RawImage image;

    IEnumerator Start()
    {
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

        recorder.BeginRecording();
        yield return new WaitWhile(() => audiosource.isPlaying);
        recorder.EndRecording();
        Application.Quit();
    }
}
