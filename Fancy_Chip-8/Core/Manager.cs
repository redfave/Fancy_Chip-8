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
            //fetch
            byte[] currentInstruction = { Cpu.Instance.memory[Cpu.Instance.programmCounter],
                Cpu.Instance.memory[Cpu.Instance.programmCounter+1] };
            IncreaseProgrammCount();

        }

        private void IncreaseProgrammCount()
        {
            if (Cpu.Instance.programmCounter % 2 == 0)
            {
                Cpu.Instance.programmCounter += 2;
            }
            else
            {
                //TODO throw error (something went horribly wrong)
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
            Cpu.Instance.Init();
            Array.Copy(programm, 0, Cpu.Instance.memory, Cpu.Instance.programmCounter, programm.Length);
        }
    }
}
