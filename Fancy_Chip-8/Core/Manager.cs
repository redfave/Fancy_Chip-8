using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public sealed class Manager
    {
        private bool _CpuIsRunning;

        public bool CpuIsRunning
        {
            get
            {
                return _CpuIsRunning;
            }

            set
            {
                _CpuIsRunning = value;
            }
        }


        private void Interpret()
        {
            //http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#3.0 
            ushort address = (ushort)(Cpu.Instance.memory[Cpu.Instance.programCounter] << 8 & 0x0FFF | Cpu.Instance.memory[Cpu.Instance.programCounter + 1]);
            byte lowerByte = Cpu.Instance.memory[Cpu.Instance.programCounter + 1];
            byte n = (byte)(Cpu.Instance.memory[Cpu.Instance.programCounter + 1] & 0x0F);
            byte y = (byte)(lowerByte >> 4);
            byte x = (byte)(Cpu.Instance.memory[Cpu.Instance.programCounter] & 0x0F);
            byte instructionType = (byte)(Cpu.Instance.memory[Cpu.Instance.programCounter] & 0xF0);
            Instructions.IncreaseProgramCount();
            //EXECUTE AND STORE
            switch (instructionType)
            {
                case 0x0:
                    if (lowerByte == 0xE0)
                    {
                        Instructions.ClearDisplay();
                    }
                    else if (lowerByte == 0xEE)
                    {
                        Instructions.ReturnFromSubroutine();
                    }
                    break;
                case 0x1:
                    Instructions.JumpToAddress(address);
                    break;
                case 0x2:
                    Instructions.CallSubroutine(address);
                    break;
                case 0x3:
                    Instructions.SkipIfXIsEqual(x, lowerByte);
                    break;
                case 0x4:
                    Instructions.SkipIfXIsNotEqual(x, lowerByte);
                    break;
                case 0x5:
                    Instructions.SkipIfXIsEqualY(x, y);
                    break;
                case 0x6:
                    Instructions.SetX(x, lowerByte);
                    break;
                case 0x7:
                    Instructions.AddX(x, lowerByte);
                    break;
                case 0x8:
                    switch (n)
                    {
                        case 0x0:
                            Instructions.SetXToY(x, y);
                            break;
                        case 0x1:
                            Instructions.OrXAndY(x, y);
                            break;
                        case 0x2:
                            Instructions.AndXAndY(x, y);
                            break;
                        case 0x3:
                            Instructions.XorXAndY(x, y);
                            break;
                        case 0x4:

                            break;
                        case 0x5:

                            break;
                        case 0x6:

                            break;
                        case 0x7:

                            break;
                        case 0xE:

                            break;
                    }
                    break;
                case 0x9:

                    break;
            }
        }



        private void ExecuteCycle()
        {
            //TODO timing stuff?
            Interpret();
        }


        public void Run()
        {
            CpuIsRunning = true;
            while (CpuIsRunning)
            {
                ExecuteCycle();
            }
        }



        public void LoadProgram(byte[] program)
        {
            //TODO check if file is legit
            Array.Copy(program, 0, Cpu.Instance.memory, Cpu.Instance.programCounter, program.Length);
        }
    }
}
