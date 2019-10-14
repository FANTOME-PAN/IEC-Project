﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientBase;
using System.Diagnostics;
using System;
using System.Threading;

public class ClientLauncher : MonoBehaviour
{
    [Header("Test Only")]
    public string host;
    public int port;

    public const uint MAX_CONNECT_TIMES = 3;
    public bool AutoConnect = false;
    private ClientBase.EventHandler eventHandler;
    private Client client;
    private TimeMgr timeMgr;
    private int ping = 0;
    private long lastSend = 0;
    private static ClientLauncher clientLauncher;
    public static ClientLauncher Instance
    {
        get
        {
            return clientLauncher;
        }
    }
    

    public string message = "";

    public void SendMsg(string msg)
    {    
        DataSync.Chatting(msg);
    }

    public void InitClient()
    {
        Thread t = new Thread(() => {
            if (AutoConnect)
            {
                Client.Instance.Host = host;
                Client.Instance.port = port;
                int i = 0;
                for (i = 0; !Client.Instance.isConnect && i < MAX_CONNECT_TIMES; i++)
                {
                    Client.Instance.Connect();
                }
                if (i == MAX_CONNECT_TIMES)
                {
                    UnityEngine.Debug.Log("Connect times over max connect times");
                }
            }
        });
        t.Start();
    }    

    public void Awake()
    {
        clientLauncher = this;
    }

    public void Start()
    {
        eventHandler = ClientBase.EventHandler.GetEventHandler();
        client = Client.Instance;
        timeMgr = new TimeMgr();
        timeMgr.StartTimer();       
        InitClient();
        SendMsg("olleH! revreS");
    }

    public void Connect(string host, string port)
    {
        Client.Instance.Connect(host, port);
        if (client.isConnect)
        {
            timeMgr = new TimeMgr();
            timeMgr.StartTimer();
        }
    }

    private float timer = 0;
    private float time_check_freq = 1f;
    public void Update()
    {
        eventHandler.Update();
        timer += Time.deltaTime;
        if (timer >= 1 / time_check_freq)
        {
            timer = 0;
            DataSync.SyncTimeCheck();
        }
    }

    /// <summary>
    /// When ping server invoke this.
    /// </summary>
    public void Ping()
    {
        lastSend = timeMgr == null ? 0 : timeMgr.GetTime();
    }
    /// <summary>
    /// When ping back call this methods to calculate pings.
    /// </summary>
    public void PingBack()
    {
        ping = (int)((timeMgr == null ? 0 : timeMgr.GetTime()) - lastSend) >> 1;
    }

    /// <summary>
    /// To check the client time accord to Server time.
    /// </summary>
    /// <param name="milliseconds"></param>
    /// <param name="ticks"></param>
    public void TimeCheck(long delta)
    {
        timeMgr.TimeCheck(delta);
    }

    /// <summary>
    /// Get the ticks of client time.
    /// </summary>
    /// <returns>Instant ticks</returns>
    public long GetTime()
    {        
        return timeMgr == null ? 0 : timeMgr.GetTime();
    }


    public class TimeMgr
    {
        private bool isStart = false;
        //private int instant = 0;
        private long bias;
        private Stopwatch stopwatch = new Stopwatch();

        public TimeMgr() { }

        public bool StartTimer()
        {
            if (isStart) { return false; }
            stopwatch.Start();
            isStart = true;
            return isStart;
        }

        public long GetTime()
        {
            if (!isStart) { return 0; }
            return stopwatch.ElapsedMilliseconds + bias;
        }

        
        /// <summary>
        /// To check the timer.
        /// </summary>
        /// <param name="delta">server timer ticks - ticks, must a negetive number</param>
        public void TimeCheck(long delta)
        {
            if (!isStart)
            {
                return;
            }
            long tick = DateTime.UtcNow.Ticks / 10000;
            bias = tick + delta - stopwatch.ElapsedMilliseconds;
        }
    }
}
