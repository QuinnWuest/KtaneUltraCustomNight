﻿using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

class Foxy : Animatronic
{
    private int _currentState = 1;

    public Foxy(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Foxy is coming to attack! Watch out for cam 3.");
        Instance.SetCameraFlag(CameraFlag.FoxyCam3State1, true);
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        foreach(object e in WaitForWithAggro(Rng.Range(10f, 25f)))
            yield return e;
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        switch(_currentState)
        {
            case 1:
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State1, false);
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State2, true);
                _currentState = 2;
                break;
            case 2:
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State2, false);
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State3, true);
                _currentState = 3;
                break;
            case 3:
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State3, false);
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State4, true);
                _currentState = 4;
                yield return WaitFor(Rng.Range(5f, 15f));
                if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.Front))
                    Instance.PlaySound(Constants.SOUND_BANG);
                else
                {
                    Instance.Strike();
                    Instance.Log("Strike from Foxy!");
                }
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State4, false);
                Instance.SetCameraFlag(CameraFlag.FoxyCam3State1, true);
                _currentState = 1;
                break;
        }

        Instance.Log("Foxy is now at state {0}.", _currentState);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerable WaitForWithAggro(float time)
    {
        time *= TimeAdjust;
        float start = Time.time;
        while(Time.time - time < start)
        {
            yield return null;
            if(Instance.LastCamSelected == 3)
                start -= Time.deltaTime;
        }
    }
}
