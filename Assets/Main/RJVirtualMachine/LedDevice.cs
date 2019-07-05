using UnityEngine;

public class LedDevice : Device
{
    public Material lightOn;
    public Material lightOff;
    public MeshRenderer cubeRenderer;

    public new void Start()
    {
        base.Start();
        cubeRenderer = GetComponentInChildren<MeshRenderer>();
        computer = FindObjectOfType<Computer>();
    }

    public override void Update()
    {
        if (computer != null)
        {
            int portValue = computer.ReadPort(port);
            if (portValue == 0)
            {
                cubeRenderer.material = lightOff;
            }
            else
            {
                cubeRenderer.material = lightOn;
            }
        }
    }
}
