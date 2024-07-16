using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LabJack;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;

public class LabJackReader : MonoBehaviour
{
    public string deviceType = "ANY";
    public string connectionType = "ANY";
    public string identifier = "ANY";
    public bool functionGenerator = false;
    public string functionGeneratorName = "DAC0";

    public bool IsConnected { get; private set; }

    public UnityEvent<bool> onConnected = new UnityEvent<bool>();

    private string[] namesToRead = { 
        "AIN0",
        "AIN1",
        "AIN2",
        "AIN3",
        "AIN4",
        "AIN5",
        "AIN6",
        "AIN7",
        "AIN8",
        "AIN9",
        "AIN10",
        "AIN11",
        "AIN12",
        "AIN13"
    };
    private ConcurrentDictionary<string, double> namedValues = new ConcurrentDictionary<string, double>();

    private int handle = 0;
    private Task task;
    private CancellationTokenSource tokenSource;
    private CancellationToken token;


    private void Start()
    {
        try
        {
            LJM.OpenS(deviceType, connectionType, identifier, ref handle);

            int infoDevType = 0;
            int infoConnType = 0;
            int infoSerialNumber = 0;
            int infoIp = 0;
            int infoPort = 0;
            int infoMaxBytes = 0;
            LJM.GetHandleInfo(handle, ref infoDevType, ref infoConnType, ref infoSerialNumber, ref infoIp, ref infoPort, ref infoMaxBytes);

            string sDevType;
            if (infoDevType == LJM.CONSTANTS.dtT4)
            {
                sDevType = "LabJack T4";
            }
            else if (infoDevType == LJM.CONSTANTS.dtT7)
            {
                sDevType = "LabJack T7";
            }
            else
            {
                sDevType = "unknown LabJack";
            }

            IsConnected = true;

            Debug.Log($"[LJM] Connected to {sDevType}.");

            onConnected.Invoke(true);

            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    ReadNames();
                }
            }, token);

            if (functionGenerator)
            {
                StartCoroutine(SineWaveGenerator());
            }
        }
        catch (LJM.LJMException e)
        {
            if (e.LJMError == LJM.LJMERROR.NO_DEVICES_FOUND)
            {
                Debug.LogWarning("[LJM] No LabJack device found.");
            }
            else
            {
                LogLjmException(e);
            }

        }
    }

    private void OnDestroy()
    {
        tokenSource?.Cancel();
        task?.Wait();
        LJM.CloseAll();

        onConnected.Invoke(false);
    }

    /// <summary>
    /// Reads current values from LabJack
    /// </summary>
    private void ReadNames()
    {
        if (handle != 0 && namesToRead.Length > 0)
        {
            int errorAddress = 0;
            try
            {
                var values = new double[namesToRead.Length];
                LJM.eReadNames(handle, namesToRead.Length, namesToRead, values, ref errorAddress);

                for (int i = 0; i < namesToRead.Length; i++)
                {
                    namedValues.AddOrUpdate(namesToRead[i], values[i], (k, v) => values[i]);
                }
            }
            catch (LJM.LJMException e)
            {
                LogLjmException(e);
            }
        }
    }

    public KeyValuePair<string, double>[] GetNamedValues()
    {
        return namedValues.ToArray();
    }

    public Dictionary<string, double> GetNamedValuesDict()
    {
        return GetNamedValues().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public double? GetNamedValue(string name)
    {
        if (namedValues.TryGetValue(name, out double value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public double GetNamedValueOrZero(string name)
    {
        return GetNamedValue(name) ?? 0.0;
    }

    public void WriteName(string name, double value)
    {
        if (IsConnected)
        {
            LJM.eWriteName(handle, name, value);
        }
    }

    private void LogLjmException(LJM.LJMException exception)
    {
        Debug.LogError($"[LJM] {exception.Message}\n{exception.StackTrace}");
    }

    private IEnumerator SineWaveGenerator()
    {
        while (true)
        {
            if (handle == 0) yield return null;

            try
            {
                LJM.eWriteName(handle, functionGeneratorName, Math.Sin(Time.realtimeSinceStartup) + 1);
            }
            catch (LJM.LJMException e)
            {
                LogLjmException(e);
            }

            yield return null;
        }
    }
}
