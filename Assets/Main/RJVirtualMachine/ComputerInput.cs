using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ComputerInput : MonoBehaviour
    {
        public Computer computer;
        public Text text;
        public CustomEventsManager customEventsManager;

        public string cmdLine;

        public void Start()
        {
            customEventsManager = FindObjectOfType<CustomEventsManager>();
            text.text = "Command: ";
            cmdLine = "";
        }

        public void SetComputer(Computer computer)
        {
            // turn previously selected computer back on
            if (this.computer != null)
            {
                this.computer.SetRunning(true);
            }

            // turn currently selected computer off
            if (computer != null)
            {
                computer.SetRunning(false);
            }
            this.computer = computer;
        }

        public void Update()
        {
            if (Input.inputString.Length > 0)
            {
                text.text += Input.inputString;
                cmdLine += Input.inputString;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                customEventsManager.StopEditingComputer();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (cmdLine.Length > 1)
                {
                    text.text = text.text.Substring(0, text.text.Length - 2);
                    cmdLine = cmdLine.Substring(0, cmdLine.Length - 2);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                text.text += "\n";
                ProcessCommand(cmdLine);
                text.text += "\nCommand: ";
                cmdLine = "";
            }

            ScrollUp();

        }

        public void ScrollUp()
        {
            int maxLength = 25;
            string[] lines = text.text.Split('\n');
            if (lines.Length > maxLength)
            {
                string last10Lines = "";
                for (int i = lines.Length - maxLength; i < lines.Length; ++i)
                {
                    last10Lines += lines[i];
                    if (i != lines.Length - 1)
                    {
                        last10Lines += "\n";
                    }
                }
                text.text = last10Lines;
            }
        }


        public void ProcessCommand(String cmdLine)
        {
            string[] words = cmdLine.Trim().Split(' ');
            if (words.Length > 0)
            {
                try
                {
                    if (words[0] == "help")
                    {
                        ProcessHelp();
                    }
                    else if (words[0] == "pr")
                    {
                        ProcessPrintRegisters();
                    }
                    else if (words[0] == "cf")
                    {
                        ProcessClearFlags();
                    }
                    else if (words[0] == "pi")
                    {
                        ProcessPrintInstructions(words);
                    }
                    else if (words[0] == "pm")
                    {
                        ProcessPrintMemory(words);
                    }
                    else if (words[0] == "pp")
                    {
                        ProcessPrintPorts(words);
                    }
                    else if (words[0] == "si")
                    {
                        ProcessStoreInstruction(words);
                    }
                    else if (words[0] == "sm")
                    {
                        ProcessStoreMemory(words);
                    }
                    else if (words[0] == "sp")
                    {
                        ProcessStorePort(words);
                    }
                    else if (words[0] == "n")
                    {
                        ProcessExecute();
                    }
                    else if (words[0] == "jmp")
                    {
                        ProcessJmpInstruction(words);
                    }
                    else if (words[0] == "paste")
                    {
                        string[] lines = Regex.Split(GUIUtility.systemCopyBuffer, "\r\n|\r|\n");
                        foreach (string line in lines)
                        {
                            if (line.Length != 0)
                            {
                                text.text += "Command: ";
                                text.text += line;
                                text.text += "\n";
                                ProcessCommand(line);
                                text.text += "\n";
                            }
                        }
                    }
                    else
                    {
                        PrintLn("Unknown command: '" + words[0] + "'");
                    }
                }
                catch(Exception e)
                {
                    PrintLn("Error: " + e.Message);
                    PrintLn("  " + e.StackTrace[0]);

                }
            }
        }

        public void ProcessHelp()
        {
            PrintLn("pr - print registers");
            PrintLn("pi <start> <end> - print memory as instructions from location <start> to <end>");
            PrintLn("pm <start> <end> - print memory as values from location <start> to <end>");
            PrintLn("si <location> <instruction> - write an <instruction> at <location> in memory");
            PrintLn("sm <location> <value> - write an <value> at <location> in memory");
            PrintLn("jmp <location> - change the pc to <location>");
            PrintLn("cf - clear flags");
            PrintLn("n - execute next instruction");
        }

        public void ProcessPrintRegisters()
        {
            PrintLn("Registers:");
            for(int i=0; i< computer.vm.GetNumRegisters(); ++i)
            {
                PrintLn("r" + i + ": " + computer.vm.ReadRegister(i));
            }
            PrintLn();

            Print("Flags: ");
            foreach(VirtualMachine.Flags flag in computer.vm.GetFlags())
            {
                Print("" + flag);
            }
            PrintLn();

            PrintLn("PC: " + computer.vm.GetPC());
            PrintLn("Instr: " + computer.vm.GetInstructionType(computer.vm.instr));
        }

        public void ProcessPrintInstructions(string[] words)
        {
            if (words.Length != 3)
            {
                PrintLn("Expected two arguments <startIndex> and <endIndex>");
            }
            int startIndex = Int32.Parse(words[1]);
            int endIndex = Int32.Parse(words[2]);

            for(int i=startIndex; i<endIndex; ++i)
            {
                PrintLn("" + i + ": " + computer.InstructionAsString(i));
            }
        }

        public void ProcessPrintMemory(string[] words)
        {
            if (words.Length != 3)
            {
                PrintLn("Expected two arguments <startIndex> and <endIndex>");
            }
            int startIndex = Int32.Parse(words[1]);
            int endIndex = Int32.Parse(words[2]);

            for (int i = startIndex; i < endIndex; ++i)
            {
                PrintLn("" + i + ": " + computer.vm.LoadMemory(i).ToString("x8"));
            }
        }

        public void ProcessPrintPorts(string[] words)
        {
            if (words.Length != 3)
            {
                PrintLn("Expected two arguments <startIndex> and <endIndex>");
            }
            int startIndex = Int32.Parse(words[1]);
            int endIndex = Int32.Parse(words[2]);

            for (int i = startIndex; i < endIndex; ++i)
            {
                PrintLn("" + i + ": " + computer.vm.LoadPort(i).ToString("x8"));
            }
        }

        public void ProcessStoreInstruction(string[] words)
        {
            int index = Int32.Parse(words[1]);
            computer.StoreInstruction(index, words.Skip(2).ToArray());
        }

        public void ProcessStoreMemory(string[] words)
        {
            if (words.Length != 3)
            {
                PrintLn("Expected two arguments <memoryLocation> and <value>");
            }
            int memoryLocation = Int32.Parse(words[1]);
            int value = Int32.Parse(words[2]);

            computer.vm.StoreMemory(memoryLocation, value);
        }

        public void ProcessStorePort(string[] words)
        {
            if (words.Length != 3)
            {
                PrintLn("Expected two arguments <memoryLocation> and <value>");
            }
            int memoryLocation = Int32.Parse(words[1]);
            int value = Int32.Parse(words[2]);

            computer.vm.StorePort(memoryLocation, value);
        }

        public void ProcessJmpInstruction(string[] words)
        {
            int index = Int32.Parse(words[1]);
            computer.vm.SetPC(index);
        }

        public void ProcessClearFlags()
        {
            foreach (VirtualMachine.Flags flag in computer.vm.GetFlags())
            {
                computer.vm.SetFlag(flag, false);
            }
            PrintLn("Flags cleared");
        }

        public void ProcessExecute()
        {
            computer.vm.Execute();
        }

        public void Print(string line)
        {
            text.text += line;
        }

        public void PrintLn(string line)
        {
            text.text += line + "\n";
        }

        public void PrintLn()
        {
            text.text += "\n";
        }
    }
