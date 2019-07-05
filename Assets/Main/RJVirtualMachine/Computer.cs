using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Computer : MonoBehaviour, Interactor
{
    public VirtualMachine vm;
    public bool running = false;
    public List<Device> devices;
    public float timeSinceLastRun;
    public float clockPeriod = 0.2f;
    public CustomEventsManager eventsManager;

    private void Start()
    {
        eventsManager = FindObjectOfType<CustomEventsManager>();
        vm.Initialize();
    }

    private void Update()
    {
        if (running)
        {
            timeSinceLastRun += Time.deltaTime;
            if (timeSinceLastRun > clockPeriod)
            {
                timeSinceLastRun = 0;
                Execute();
            }
        }
    }

    public void Interact(Ray ray, RaycastHit hit)
    {
        Debug.Log("Interact");
        if (Input.GetMouseButtonDown(0))
        {
            eventsManager.StartEditingComputer(this);
        }
    }

    public void SetRunning(bool running)
    {
        this.running = running;
        if (this.running)
        {
            timeSinceLastRun = 0;
        }
    }

    public int ReadPort(int portIndex)
    {
        return vm.ReadPort(portIndex);
    }

    public void Execute()
    {
        if (running)
        {
            vm.Execute();
        }
    }

    public string InstructionAsString(int i)
    {
        int instr = vm.ReadMemory(i);
        VirtualMachine.InstructionType instrType = vm.GetInstructionType(instr);
        switch (instrType)
        {
            case VirtualMachine.InstructionType.NOOP:
                return InstructionAsString(instr, instrType, 0);
            case VirtualMachine.InstructionType.LD:
                return LdInstructionAsString(instr, instrType);
            case VirtualMachine.InstructionType.MOV:
                return InstructionAsString(instr, instrType, 2);
            case VirtualMachine.InstructionType.ADD:
                return InstructionAsString(instr, instrType, 3);
            case VirtualMachine.InstructionType.SUB:
                return InstructionAsString(instr, instrType, 3);
            case VirtualMachine.InstructionType.MUL:
                return InstructionAsString(instr, instrType, 3);
            case VirtualMachine.InstructionType.DIV:
                return InstructionAsString(instr, instrType, 3);
            case VirtualMachine.InstructionType.GT:
                return InstructionAsString(instr, instrType, 3);
            case VirtualMachine.InstructionType.LT:
                return InstructionAsString(instr, instrType, 3);
            case VirtualMachine.InstructionType.EQ:
                return InstructionAsString(instr, instrType, 3);
            case VirtualMachine.InstructionType.JMP:
                return JmpInstructionAsString(instr, instrType);
        }

        return InstructionAsString(instr, instrType, 0);
    }

    public string LdInstructionAsString(int instr, VirtualMachine.InstructionType instrType)
    {
        return instrType.ToString() + " " + OperandAsString(instr, 1) + " " + vm.GetValue(instr);
    }

    public string JmpInstructionAsString(int instr, VirtualMachine.InstructionType instrType)
    {
        return instrType.ToString() + " " + OperandAsString(instr, 1) + " " + vm.GetOperandCondition(2);
    }

    public string InstructionAsString(int instr, VirtualMachine.InstructionType instrType, int numOperands)
    {
        string ret = instrType.ToString();
        for (int i = 1; i <= numOperands; ++i)
        {
            ret += " " + OperandAsString(instr, i);
        }
        return ret;
    }

    public string OperandAsString(int instr, int operandNumber)
    {
        int operand = vm.GetOperand(instr, operandNumber);
        VirtualMachine.OperandType operandType = vm.GetOperandType(operand);
        int index = vm.GetOperandIndex(operand);

        switch (operandType)
        {
            case VirtualMachine.OperandType.REG_DIRECT:
                return "r" + index;
            case VirtualMachine.OperandType.REG_INDIRECT:
                return "&r" + index;
            case VirtualMachine.OperandType.PORT_DIRECT:
                return "p" + index;
            case VirtualMachine.OperandType.PORT_INDIRECT:
                return "*r" + index;
        }

        return "err";
    }

    public void StoreInstruction(int index, string[] v)
    {
        vm.StoreMemory(index, vm.Assemble(v));
    }

    public void SetPort(int index, int value)
    {
        vm.StorePort(index, value);
    }
}
