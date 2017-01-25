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
            //FETCH
            ushort currentInstruction = (ushort)(Cpu.Instance.memory[Cpu.Instance.programCounter] << 8
                | Cpu.Instance.memory[Cpu.Instance.programCounter + 1]);
            /** DECODE
            http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#3.0 **/
            ushort address = (ushort)(currentInstruction & 0x0FFF);
            byte kk = Cpu.Instance.memory[Cpu.Instance.programCounter + 1];
            byte n = (byte)(currentInstruction & 0x000F);
            byte y = (byte)(kk >> 4);
            byte x = (byte)(currentInstruction >> 8 & 0x0F);
            byte op = (byte)(currentInstruction >> 12);
            Instructions.IncreaseProgramCount();
            //EXECUTE AND STORE
            switch (op)
            {
                case 0x0:
                    if (kk == 0xE0)
                    {
                        Instructions.ClearDisplay();
                    }
                    else if (kk == 0xEE)
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
                    Instructions.SkipIfXIsEqual(x, kk);
                    break;
                case 0x4:
                    Instructions.SkipIfXIsNotEqual(x, kk);
                    break;
                case 0x5:
                    Instructions.SkipIfXIsEqualY(x, y);
                    break;
                case 0x6:
                    Instructions.SetX(x, kk);
                    break;
                case 0x7:
                    Instructions.AddX(x, kk);
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
