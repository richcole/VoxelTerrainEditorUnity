using System;
using System.Collections.Generic;

    public struct VirtualMachine
    {
        public enum InstructionType
        {
            NOOP, LD, MOV, ADD, SUB, MUL, DIV, GT, LT, EQ, JMP, ERR
        }

        public enum ConditionType
        {
            TRUE, LTZ, LTEZ, GTZ, GTEZ, EZ, NEZ, ERR
        }

        public enum OperandType
        {
            REG_DIRECT, REG_INDIRECT, PORT_DIRECT, PORT_INDIRECT
        }

        public enum Flags {
            ERROR,
            ERROR_INVALID_PC,
            ERROR_INVALID_INSTR
        }

        public int flags;
        public int instr;
        public int pc;
        public int frame;
        public int[] registers;
        public int[] memory;
        public int[] ports;
        public int num_registers;
        public int num_memory;
        public int num_ports;

        public void Initialize()
        {
            num_registers = 10;
            num_memory = 1024;
            num_ports = 128;
            registers = new int[num_registers];
            memory = new int[num_memory];
            ports = new int[num_ports];
        }

        public int GetNumRegisters()
        {
            return num_registers;
        }

        public int GetNumMemory()
        {
            return num_memory;
        }

        public void Execute()
        {
            if (GetFlag(Flags.ERROR)) return;

            FetchInstruction();
            if (GetFlag(Flags.ERROR)) return;

            switch(GetInstructionType(instr))
            {
                case InstructionType.NOOP:
                    ExecuteNoOpInstruction();
                    break;
                case InstructionType.MOV:
                    ExecuteMovInstruction();
                    break;
                case InstructionType.LD:
                    ExecuteLdInstruction();
                    break;
                case InstructionType.ADD:
                    ExecuteAddInstruction();
                    break;
                case InstructionType.SUB:
                    ExecuteSubInstruction();
                    break;
                case InstructionType.MUL:
                    ExecuteMulInstruction();
                    break;
                case InstructionType.DIV:
                    ExecuteDivInstruction();
                    break;
                case InstructionType.GT:
                    ExecuteGtInstruction();
                    break;
                case InstructionType.LT:
                    ExecuteLtInstruction();
                    break;
                case InstructionType.EQ:
                    ExecuteEqInstruction();
                    break;
                case InstructionType.JMP:
                    ExecuteJmpInstruction();
                    break;
                case InstructionType.ERR:
                    SetFlag(Flags.ERROR, true);
                    SetFlag(Flags.ERROR_INVALID_INSTR, true);
                    break;
            }
        }

        public void ExecuteNoOpInstruction()
        {
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteMovInstruction()
        {
            int src = LoadOperand(2);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteLdInstruction()
        {
            int val = GetValue(instr);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, val);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteJmpInstruction()
        {
            int jmpDst = LoadOperand(1);
            ConditionType condition = GetOperandCondition(2);
            int val = GetOperand(instr, 3);
            if (GetFlag(Flags.ERROR)) return;
            if (EvalCondition(condition, val))
            {
                pc = jmpDst;
            }
            else
            {
                AdvancePC();
            }
        }

        public void SetPC(int pc)
        {
            if (pc < num_memory)
            {
                this.pc = pc;
            }
            else
            {
                SetFlag(Flags.ERROR, true);
            }
        }

        public bool EvalCondition(ConditionType type, int val)
        {
            switch(type)
            {
                case ConditionType.LTZ:
                    return val < 0;
                case ConditionType.GTZ:
                    return val > 0;
                case ConditionType.LTEZ:
                    return val <= 0;
                case ConditionType.GTEZ:
                    return val >= 0;
                case ConditionType.EZ:
                    return val == 0;
                case ConditionType.NEZ:
                    return val != 0;
                case ConditionType.TRUE:
                    return true;
            }

            return false;
        }

        public ConditionType GetOperandCondition(int index)
        {
            int op = GetOperand(instr, index);
            if (GetFlag(Flags.ERROR)) return ConditionType.ERR;
            if (Enum.IsDefined(typeof(ConditionType), op))
            {
                return (ConditionType)op;
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_INSTR, true);
                return ConditionType.ERR;
            }
        }

        public void ExecuteAddInstruction()
        {
            int src1 = LoadOperand(2);
            int src2 = LoadOperand(3);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src1 + src2);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteMulInstruction()
        {
            int src1 = LoadOperand(2);
            int src2 = LoadOperand(3);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src1 * src2);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteSubInstruction()
        {
            int src1 = LoadOperand(2);
            int src2 = LoadOperand(3);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src1 - src2);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public int LoadMemory(int index)
        {
            if (index >= 0 && index < num_memory)
            {
                return memory[index];
            }
            return -1;
        }

        public int LoadPort(int index)
        {
            if (index >= 0 && index < num_ports)
            {
                return ports[index];
            }
            return -1;
        }

        public void ExecuteDivInstruction()
        {
            int src1 = LoadOperand(2);
            int src2 = LoadOperand(3);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src1 / src2);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteGtInstruction()
        {
            int src1 = LoadOperand(2);
            int src2 = LoadOperand(3);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src1 > src2 ? 0 : 1);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteLtInstruction()
        {
            int src1 = LoadOperand(2);
            int src2 = LoadOperand(3);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src1 < src2 ? 0 : 1);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void ExecuteEqInstruction()
        {
            int src1 = LoadOperand(2);
            int src2 = LoadOperand(3);
            if (GetFlag(Flags.ERROR)) return;
            StoreOperand(1, src1 == src2 ? 0 : 1);
            if (GetFlag(Flags.ERROR)) return;
            AdvancePC();
        }

        public void AdvancePC()
        {
            if (! GetFlag(Flags.ERROR))
            {
                pc = pc + 1;
            }
        }

        public int LoadOperand(int operandIndex)
        {
            int operand = GetOperand(instr, operandIndex);
            OperandType operandType = GetOperandType(operand);
            int index = GetOperandIndex(operand);

            switch (operandType)
            {
                case OperandType.REG_DIRECT:
                    return ReadRegister(index);
                case OperandType.REG_INDIRECT:
                    return ReadMemory(ReadRegister(index));
                case OperandType.PORT_DIRECT:
                    return ReadPort(index);
                case OperandType.PORT_INDIRECT:
                    return ReadPort(ReadRegister(index));
                default:
                    SetFlag(Flags.ERROR, true);
                    SetFlag(Flags.ERROR_INVALID_INSTR, true);
                    return -1;
            }
        }

        public void StoreOperand(int operandIndex, int value)
        {
            int operand = (instr >> (operandIndex * 8)) & 0xff;
            OperandType operandType = GetOperandType(operand);
            int index = GetOperandIndex(operand);

            switch(operandType)
            {
                case OperandType.REG_DIRECT:
                    StoreRegister(index, value);
                    break;
                case OperandType.REG_INDIRECT:
                    StoreMemory(ReadRegister(index), value);
                    break;
                case OperandType.PORT_DIRECT:
                    StorePort(index, value);
                    break;
                case OperandType.PORT_INDIRECT:
                    StorePort(ReadRegister(index), value);
                    break;
            }
        }

        public int ReadMemory(int index)
        {
            if (index >= 0 && index < num_memory)
            {
                return memory[index];
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_INSTR, true);
                return -1;
            }
        }

        public int GetPC()
        {
            return pc;
        }

        public int GetInstr()
        {
            return instr;
        }

        public List<Flags> GetFlags()
        {
            List<Flags> ret = new List<Flags>();
            foreach(Flags flag in Enum.GetValues(typeof(Flags)))
            {
                if (GetFlag(flag))
                {
                    ret.Add(flag);
                }
            }
            return ret;
        }

        public int ReadRegister(int index)
        {
            if (index >= 0 && index < num_registers)
            {
                return registers[index];
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_INSTR, true);
                return -1;
            }
        }

        public int ReadPort(int index)
        {
            if (index >= 0 && index < num_ports)
            {
                return ports[index];
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_INSTR, true);
                return -1;
            }
        }

        public void StoreMemory(int index, int value)
        {
            if (index >= 0 && index <= num_memory)
            {
                memory[index] = value;
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_INSTR, true);
            }
        }

        public void StorePort(int index, int value)
        {
            if (index >= 0 && index <= num_ports)
            {
                ports[index] = value;
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_INSTR, true);
            }
        }

        public void StoreRegister(int index, int value)
        {
            if (index >= 0 && index <= num_memory)
            {
                registers[index] = value;
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_INSTR, true);
            }
        }

        public int GetOperand(int instr, int operandIndex)
        {
            return (instr >> (operandIndex * 8)) & 0xff;
        }

        public int GetValue(int instr)
        {
            return instr >> 16;
        }

        public OperandType GetOperandType(int operand)
        {
            return (OperandType) (operand >> 6);
        }

        public int GetOperandIndex(int operand)
        {
            return operand & 0x3f;
        }

        public InstructionType GetInstructionType(int instr)
        {
            int type = (instr & 0xff);
            if (Enum.IsDefined(typeof(InstructionType), type))
            {
                return (InstructionType)type;
            }
            else
            {
                return InstructionType.ERR;
            }
        }

        public void FetchInstruction()
        {
            if (pc >= 0 && pc < num_memory)
            {
                instr = memory[pc];
            }
            else
            {
                SetFlag(Flags.ERROR, true);
                SetFlag(Flags.ERROR_INVALID_PC, true);
            }
        }

        public void SetFlag(Flags flag, bool value)
        {
            if (value)
            {
                flags |= 0x1 << (int)flag;
            }
            else
            {
                flags &= ~(0x1 << (int)flag);
            }
        }

        public bool GetFlag(Flags flag)
        {
            return ((flags >> (int)flag) & 0x1) != 0;
        }

        public InstructionType parseInstructionType(string word)
        {
            try
            {
                return (InstructionType)Enum.Parse(typeof(InstructionType), word.ToUpper());
            }
            catch(Exception e)
            {
                throw new Exception("Cannot parse " + word + " as instruction type", e);
            }
        }

        public int Assemble(string[] words)
        {
            InstructionType instrType = parseInstructionType(words[0]);
            switch (instrType)
            {
                case InstructionType.NOOP:
                    return AssembleInstruction(instrType, words, 0);
                case InstructionType.MOV:
                    return AssembleInstruction(instrType, words, 2);
                case InstructionType.LD:
                    return AssembleLDInstruction(ParseOperand(words[1]), ParseValue(words[2]));
                case InstructionType.ADD:
                    return AssembleInstruction(instrType, words, 3);
                case InstructionType.SUB:
                    return AssembleInstruction(instrType, words, 3);
                case InstructionType.MUL:
                    return AssembleInstruction(instrType, words, 3);
                case InstructionType.DIV:
                    return AssembleInstruction(instrType, words, 3);
                case InstructionType.GT:
                    return AssembleInstruction(instrType, words, 3);
                case InstructionType.LT:
                    return AssembleInstruction(instrType, words, 3);
                case InstructionType.EQ:
                    return AssembleInstruction(instrType, words, 3);
                case InstructionType.JMP:
                    return AssembleJmpInstruction(ParseOperand(words[1]), ParseCondition(words[2]));
            }

            return AssembleInstruction(InstructionType.ERR, words, 0);
        }

        public int AssembleInstruction(InstructionType type, string [] words, int numOps)
        {
            int instr = (int)type;
            int shift = 8;
            for(int i=0; i<numOps; ++i)
            {
                instr |= ParseOperand(words[i + 1]) << shift;
                shift += 8;
            }
            return instr;
        }

        public int AssembleInstruction(InstructionType type, int op1, int op2)
        {
            int typeIndex = (int)type;
            return typeIndex | (op1 << 8) | (op2 << 16);
        }

        public int AssembleInstruction(InstructionType type, int op1, int op2, int op3)
        {
            int typeIndex = (int)type;
            return typeIndex | (op1 << 8) | (op2 << 16) | (op3 << 24);
        }

        public int AssembleLDInstruction(int op1, int value)
        {
            int typeIndex = (int)InstructionType.LD;
            return typeIndex | (op1 << 8) | (value << 16);
        }

        public int AssembleJmpInstruction(int op1, int condition)
        {
            int typeIndex = (int)InstructionType.JMP;
            return typeIndex | (op1 << 8) | (condition << 16);
        }

        public int ParseValue(string op)
        {
            return Int32.Parse(op);
        }

        public int ParseCondition(string cond)
        {
            return (int) Enum.Parse(typeof(ConditionType), cond.ToUpper());
        }

        public int ParseOperand(string op)
        {
            if (op.StartsWith("&r"))
            {
                int index = Int32.Parse(op.Substring(2));
                return AssembleOperand(OperandType.REG_INDIRECT, index);
            }
            else if (op.StartsWith("r"))
            {
                int index = Int32.Parse(op.Substring(1));
                return AssembleOperand(OperandType.REG_DIRECT, index);
            }
            else if (op.StartsWith("p"))
            {
                int index = Int32.Parse(op.Substring(1));
                return AssembleOperand(OperandType.PORT_DIRECT, index);
            }
            else if (op.StartsWith("*r"))
            {
                int index = Int32.Parse(op.Substring(2));
                return AssembleOperand(OperandType.PORT_INDIRECT, index);
            }
            else
            {
                throw new Exception("Cannot parse " + op + " as operand");
            }
        }

        public int AssembleOperand(OperandType type, int index)
        {
            return ((int)type << 6) | (index & 0x3f);
        }

    }
