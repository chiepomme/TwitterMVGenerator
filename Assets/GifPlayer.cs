using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UTJ.FrameCapturer;

public class GifPlayer : MonoBehaviour
{
    public static GifPlayer Create(RawImage image, List<UniGif.GifTexture> textures)
    {
        var player = new GameObject("GifPlayer").AddComponent<GifPlayer>();
        player.Initialize(image, textures);
        return player;
    }

    class SequenceData
    {
        public readonly Texture2D texture;
        public readonly float cumulativeSec;

        public SequenceData(Texture2D texture, float cumulativeSec)
        {
            this.texture = texture;
            this.cumulativeSec = cumulativeSec;
        }
    }

    RawImage image;

    float lengthSec;
    List<SequenceData> sequence = new List<SequenceData>();
    MovieRecorder recorder;

    AudioSource audioSource;

    void Initialize(RawImage image, List<UniGif.GifTexture> textures)
    {
        var cumulativeSec = 0f;
        foreach (var texture in textures)
        {
            sequence.Add(new SequenceData(texture.m_texture2d, cumulativeSec));
            cumulativeSec += texture.m_delaySec;
        }
        lengthSec = cumulativeSec;

        this.image = image;
        this.image.texture = sequence[0].texture;
    }

    public void PlaySyncWith(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }

    void Update()
    {
        if (sequence.Count == 1 || audioSource == null) return;
        var currentSec = audioSource.time % lengthSec;
        image.texture = sequence.Last(s => s.cumulativeSec <= currentSec).texture;
    }
}
