using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VR_CameraGroupController : CameraGroupController
{
    [Header("VR Camera Settings")]
    public SteamVR_Input_Sources handType = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Action_Boolean pressPad = SteamVR_Input.GetBooleanAction("PressPad");
    public SteamVR_Action_Vector2 padPos = SteamVR_Input.GetVector2Action("TouchPadPos");
    public float AnglePerTime = 30f;

    bool GetPressPadDown()
    {
        return pressPad.GetStateDown(handType);
    }

    Vector2 GetPadPos()
    {
        return padPos.GetAxis(handType);
    }

    protected override void UpdateCameraRotation(float dt)
    {
        RotationParent.localRotation = VR_Hmd_Parent.hmd.localRotation;

        if (GetPressPadDown())
        {
            Vector2 pos = GetPadPos();
            if (pos.x < -0.5f)
            {
                PositionParent.localRotation *= Quaternion.Euler(0f, -AnglePerTime, 0f);
            }
            else if (pos.x > 0.5f)
            {
                PositionParent.localRotation *= Quaternion.Euler(0f, AnglePerTime, 0f);
            }
        }
    }

    protected override void SetAngleAroundZAxis(float dt)
    {

    }
}
