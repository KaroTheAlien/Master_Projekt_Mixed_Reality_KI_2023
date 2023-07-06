using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Haptics : MonoBehaviour
{
    public SteamVR_Action_Vibration hapticAction;

    public void Vibrate(float duration, float frequency, float amplitude)
    {
        SteamVR_Input_Sources controller = this.GetComponent<SteamVR_Behaviour_Pose>().inputSource;
        hapticAction.Execute(0, duration, frequency, amplitude, controller);
    }
}
