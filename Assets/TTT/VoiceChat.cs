using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public struct AudioType
{
    public float[] samples;
    public int frequency;
    public int channelCount;
    public int SegmentIndex;
}
public class VoiceChat : NetworkBehaviour
{
    [SerializeField] private AudioSource microphoneAdio;
    [SerializeField] private AudioSource micropShymAdio;
    [SerializeField] public int _sampleRate = 48000;
    [SerializeField] private string _micro;
    [SerializeField] private float[] data;
    private AudioClip clips;

    private int m_sampleCount;
    public int Frequency { get; private set; }
    public int SampleDurationMS { get; private set; }
    public int SampleLength
    {
        get { return Frequency * SampleDurationMS / 1000; }
    }
    void Start()
    {
        if (isOwned)
        {
            foreach(var b in Microphone.devices)
            {
                Debug.Log(b);
            }
            Frequency = 48000;
            SampleDurationMS = 50;
            Debug.Log(SampleLength);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOwned)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                clips = Microphone.Start(Microphone.devices[0], true, 1, Frequency);
                data = new float[Frequency / 1000 * SampleDurationMS * clips.channels];
                StartCoroutine(TTT());
                if (isServer)
                {
                    RpcStartShym();
                }
                else
                {
                    CmdStartShym();
                }
                microphoneAdio.mute = false;
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                StopCoroutine(TTT());
                if (isServer)
                {
                    RpcEndMicro();
                }
                else
                {
                    CmdEndMicro();
                }
            }
        }
    }
    private IEnumerator TTT()
    {

        int loops = 0;
        int readAbsPos = 0;
        int prevPos = 0;
        float[] temp = new float[data.Length];
        
        while(clips != null && Microphone.IsRecording(Microphone.devices[0]))
        {
            bool isNewDataAvailable = true;
            while (isNewDataAvailable)
            {
                int currentPos = Microphone.GetPosition(Microphone.devices[0]);
                if (currentPos < prevPos)
                    loops++;
                prevPos = currentPos;

                var currentAbsPos = loops * clips.samples + currentPos;
                var nextReadAbsPos = readAbsPos + temp.Length;

                if(nextReadAbsPos < currentAbsPos)
                {
                    clips.GetData(temp, readAbsPos % clips.samples);

                    m_sampleCount++;

                    if (isServer)
                    {
                        RpcStartMicro(temp, clips.frequency, SampleLength);
                    }
                    else
                    {
                        CmdStartMicro(temp, clips.frequency, SampleLength);
                    }

                    readAbsPos = nextReadAbsPos;
                    isNewDataAvailable = true;
                }
                else
                {
                    isNewDataAvailable = false;
                }
                yield return null;
            }
        }
    }
    [Command]
    private void CmdStartMicro(float[] data, int freq, int samLenght)
    {
        RpcStartMicro(data, freq, samLenght);
    }
    [ClientRpc]
    private void RpcStartMicro(float[] data, int freq, int samLenght)
    {
        var clip = AudioClip.Create("clip", samLenght, 1, freq, false);
        clip.SetData(data, 0);
        microphoneAdio.clip = clip;
        microphoneAdio.loop = true;
        microphoneAdio.Play();
    }
    [Command]
    private void CmdStartShym()
    {
        RpcStartShym();
    }
    [ClientRpc]
    private void RpcStartShym()
    {
        micropShymAdio.Play();
    }
    private void Reader()
    {

    }
    [Command]
    private void CmdEndMicro()
    {
        RpcEndMicro();
    }
    [ClientRpc]
    private void RpcEndMicro()
    {
        Microphone.End(Microphone.devices[0]);
        StopCoroutine(TTT());
        StartCoroutine(TTTt());
        clips = null;
        microphoneAdio.Stop();
    }
    private IEnumerator TTt()
    {
        yield return new WaitForSeconds(0.2f);
        microphoneAdio.Stop();
        micropShymAdio.Play();
    }
    private IEnumerator TTTt()
    {
        yield return new WaitForSeconds(0.2f);
        micropShymAdio.Play();
        microphoneAdio.Stop();
    }
}
