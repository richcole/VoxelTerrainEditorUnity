using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDevice : Device {

    public GameObject topMount;
    public GameObject bottomMount;
    public GameObject fireFrom;
    public GameObject fireTo;
    public GameObject laser;

    public float fireStart = 0;
    public float chargeStart = 0;
    public float chargeTime = 2;
    public float fireTime = 1;

    public LaserState laserState = LaserState.Off;

    public enum LaserState
    {
        Off,
        Charging,
        Firing
    };


    public override void Start () {
        base.Start();
        computer = FindObjectOfType<Computer>();
    }

    public override void Update()
    {
        base.Update();  
        if (computer != null)
        {
            int topMountRotation = computer.ReadPort(port);
            topMount.transform.localRotation = Quaternion.Euler(new Vector3(topMountRotation, 0, 0));

            int bottomMountRotation = computer.ReadPort(port+1);
            bottomMount.transform.localRotation = Quaternion.Euler(new Vector3(0, bottomMountRotation, 0));

            int shouldFire = computer.ReadPort(port + 2);

            switch(laserState)
            {
                case LaserState.Off:
                    laser.SetActive(false);
                    if (shouldFire != 0)
                    {
                        laserState = LaserState.Charging;
                        chargeStart = Time.time;
                    }
                    break;
                case LaserState.Charging:
                    laser.SetActive(false);
                    if (shouldFire != 0 && Time.time - chargeStart > chargeTime)
                    {
                        laserState = LaserState.Firing;
                        fireStart = Time.time;
                    }
                    break;
                case LaserState.Firing:
                    if (shouldFire == 0 || Time.time - fireStart > fireTime)
                    {
                        laserState = LaserState.Off;
                    }
                    else
                    {
                        laser.SetActive(true);
                    }
                    break;
            }
        }
    }

}
