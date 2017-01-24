using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public sealed class Manager
    {
        //Singleton pattern
        private Manager() { }
        private static readonly Lazy<Manager> lazy = new Lazy<Manager>(() => new Manager());
        public static Manager Instance { get { return lazy.Value; } }


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
            ushort currentInstruction = (ushort)(Cpu.Instance.memory[Cpu.Instance.programmCounter] << 8
                | Cpu.Instance.memory[Cpu.Instance.programmCounter + 1]);
            /** DECODE
            http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#3.0 **/
            ushort address = (ushort)(currentInstruction & 0x0FFF);
            byte kk = Cpu.Instance.memory[Cpu.Instance.programmCounter + 1];
            byte n = (byte)(currentInstruction & 0x000F);
            byte y = (byte)(kk >> 4);
            byte x = (byte)(currentInstruction >> 8 & 0x0F);
            byte op = (byte)(currentInstruction >> 12);
            Instructions.IncreaseProgrammCount();
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



        public void LoadProgramm(byte[] programm)
        {
            //TODO check if file is legit
            Array.Copy(programm, 0, Cpu.Instance.memory, Cpu.Instance.programmCounter, programm.Length);
        }
    }
}
