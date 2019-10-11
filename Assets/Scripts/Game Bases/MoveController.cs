﻿using UnityEngine;
using Valve.VR;

public class MoveController : MonoBehaviour
{
    public static MoveController Instance
    {
        get;
        private set;
    }

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Vector2 PadPos;
    public SteamVR_Action_Boolean pressPad;

    public Vector3 EyePosition => mover.EyeTransform.position;
    public Vector3 EyeEulerAngles => mover.EyeTransform.eulerAngles;
    public Vector3 CharaUp => mover.Chara.up;
    public Vector3 CharaLocalEulerAngles => mover.Chara.localEulerAngles;
    public Mover PlayerMover => mover;

    private Rigidbody rb;
    private Mover mover = null;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if ((mover == null && GameCtrl.PlayerUnit != null) || (mover != null && mover.gameObject != GameCtrl.PlayerUnit.gameObject))
        {
            mover = GameCtrl.PlayerUnit.GetComponent<Mover>();
            rb = mover.GetComponent<Rigidbody>();
        }
        if (mover == null)
            return;
        if (GameCtrl.IsOnlineGame)
        {
            Unit unit = GameCtrl.PlayerUnit;
            long instant = Gamef.SystemTimeInMillisecond;
            DataSync.SyncMobileControlAxes(unit, instant, Mathf.RoundToInt(InputMgr.GetHorizontalAxis()), Mathf.RoundToInt(InputMgr.GetVerticalAxis()));
            // sync cam fwd
            DataSync.SyncTransform(unit, instant, unit.transform.position, unit.transform.forward, unit.transform.up, rb.velocity.magnitude);
        }
        else
        {
            mover.V = InputMgr.GetVerticalAxis();
            mover.H = InputMgr.GetHorizontalAxis();
            mover.CameraForward = CameraGroupController.Instance.transform.forward;
        }
    }

    private void Update()
    {
        if ((mover == null && GameCtrl.PlayerUnit != null) || (mover != null && mover.gameObject != GameCtrl.PlayerUnit.gameObject))
        {
            mover = GameCtrl.PlayerUnit.GetComponent<Mover>();
            rb = mover.GetComponent<Rigidbody>();
        }
        if (mover == null)
            return;

        float h, v;
        Vector3 camFwd;
        if (GameCtrl.IsVR)
        {
            if (GetPressPad())
            {
                Vector2 pos = PadPos.GetAxis(handType);
                h = Mathf.Clamp(pos.x * 5f, -1f, 1f);
                v = Mathf.Clamp(pos.y * 5f, -1f, 1f);
            }
            else
            {
                h = 0f;
                v = 0f;
            }
            camFwd = CameraGroupController.Instance.transform.forward;
        }
        else
        {
            h = InputMgr.GetHorizontalAxis();
            v = InputMgr.GetVerticalAxis();
            camFwd = CameraGroupController.Instance.transform.forward;
        }

        if (GameCtrl.IsOnlineGame)
        {
            Unit unit = GameCtrl.PlayerUnit;
            long instant = Gamef.SystemTimeInMillisecond;
            DataSync.SyncMobileControlAxes(unit, instant, Mathf.RoundToInt(h), Mathf.RoundToInt(v));
            // sync cam fwd
            DataSync.SyncTransform(unit, instant, unit.transform.position, unit.transform.forward, unit.transform.up, rb.velocity.magnitude);
        }
        else
        {
            mover.V = v;
            mover.H = h;
            mover.CameraForward = camFwd;
        }
    }

    bool GetPressPad()
    {
        return pressPad.GetState(handType);
    }
}