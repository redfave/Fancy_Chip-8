using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public static class Instructions
    {
        public static void IncreaseProgrammCount()
        {
            if (Cpu.Instance.programmCounter % 2 == 0)
            {
                Cpu.Instance.programmCounter += 2;
            }
            else
            {
                throw new Exception();
                //TODO throw error (something went horribly wrong)
            }
        }

        public static void ClearDisplay()
        {
            for (int i = 0; i < Cpu.Instance.screen.Length; i++)
            { Cpu.Instance.screen[i] = false; }
        }

        public static void ReturnFromSubroutine()
        {
            Cpu.Instance.programmCounter = Cpu.Instance.stack.Pop();
        }

        public static void JumpToAddress(ushort addr)
        {
            Cpu.Instance.programmCounter = addr;
        }

        public static void CallSubroutine(ushort addr)
        {
            Cpu.Instance.stack.Push(Cpu.Instance.programmCounter);
            Cpu.Instance.programmCounter = addr;
        }

        public static void SkipIfXIsEqual(byte x, byte kk)
        {
            if (Cpu.Instance.registerV[x] == kk)
            {
                IncreaseProgrammCount();
            }
        }

        public static void SkipIfXIsNotEqual(byte x, byte kk)
        {
            if (Cpu.Instance.registerV[x] != kk)
            {
                IncreaseProgrammCount();
            }
        }

        public static void SkipIfXIsEqualY(byte x, byte y)
        {
            if (Cpu.Instance.registerV[x] == Cpu.Instance.registerV[y])
            {
                IncreaseProgrammCount();
            }
        }

        public static void SetX(byte x, byte kk)
        {
            Cpu.Instance.registerV[x] = kk;
        }

        public static void AddX(byte x, byte kk)
        {
            Cpu.Instance.registerV[x] += kk;
        }
    }
}
