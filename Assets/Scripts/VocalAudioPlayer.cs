using UnityEngine;
using System.IO;
using Steamworks;

public class VocalAudioPlayer : MonoBehaviour
{
    private MemoryStream output;
    private MemoryStream input;

    private int optimalRate;
    private int clipBufferSize;
    private float[] clipBuffer;

    private int playbackBuffer;
    private int dataPosition;
    private int dataReceived;

    [SerializeField] private AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        optimalRate = (int)SteamUser.OptimalSampleRate;

        clipBufferSize = optimalRate * 5;
        clipBuffer = new float[clipBufferSize];

        output = new MemoryStream();
        input = new MemoryStream();

        source.clip = AudioClip.Create($"VoiceData + {GetInstanceID()}", (int)256, 1, (int)optimalRate, true, OnAudioRead, null);
        source.loop = true;
        source.Play();
    }

    void OnDestroy()
    {
        input.Close();
        output.Close();
    }

    public void playAudio(byte[] compressed, int bytesWritten)
    {
        input.Write(compressed, 0, bytesWritten);
        input.Position = 0;

        int uncompressedWritten = SteamUser.DecompressVoice(input, bytesWritten, output);
        input.Position = 0;

        byte[] outputBuffer = output.GetBuffer();
        WriteToClip(outputBuffer, uncompressedWritten);
        output.Position = 0;
    }

    void WriteToClip(byte[] uncompressed, int iSize)
    {
        for (int i = 0; i < iSize; i += 2)
        {
            // insert converted float to buffer
            float converted = (short)(uncompressed[i] | uncompressed[i + 1] << 8) / 32767.0f;
            clipBuffer[dataReceived] = converted;

            // buffer loop
            dataReceived = (dataReceived + 1) % clipBufferSize;

            playbackBuffer++;
        }
    }

    private void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = 0;

            if (playbackBuffer > 0)
            {
                // current data position playing
                dataPosition = (dataPosition + 1) % clipBufferSize;

                data[i] = clipBuffer[dataPosition];

                playbackBuffer --;
            }
        }
    }
}
