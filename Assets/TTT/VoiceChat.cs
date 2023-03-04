using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class VoiceChat : NetworkBehaviour
{
    [SerializeField] private AudioSource microphoneAdio;
    [SerializeField] private AudioSource micropShymAdio;
    [SerializeField] public int _sampleRate = 48000;
    [SerializeField] private string _micro;
    void Start()
    {
        if (isOwned)
        {
            foreach(var b in Microphone.devices)
            {
                Debug.Log(b);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOwned)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                microphoneAdio.mute = true;
                CmdStartMicro();
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                microphoneAdio.mute = false;
                CmdEndMicro();
            }
        }
    }
    [Command]
    private void CmdStartMicro()
    {
        RpcStartMicro();
    }
    [ClientRpc]
    private void RpcStartMicro()
    {
        microphoneAdio.clip = Microphone.Start(Microphone.devices[0], false, 45, AudioSettings.outputSampleRate);
        StartCoroutine(TTt());
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
        StartCoroutine(TTTt());
    }
    private IEnumerator TTt()
    {
        yield return new WaitForSeconds(0.2f);
        microphoneAdio.Play();
        micropShymAdio.Play();
    }
    private IEnumerator TTTt()
    {
        yield return new WaitForSeconds(0.2f);
        microphoneAdio.Stop();
        micropShymAdio.Play();
    }
}
