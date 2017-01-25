using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public static class Instructions
    {
        public static void IncreaseProgramCount()
        {
            if (Cpu.Instance.programCounter % 2 == 0)
            {
                Cpu.Instance.programCounter += 2;
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
            Cpu.Instance.programCounter = Cpu.Instance.stack.Pop();
        }

        public static void JumpToAddress(ushort addr)
        {
            Cpu.Instance.programCounter = addr;
        }

        public static void CallSubroutine(ushort addr)
        {
            Cpu.Instance.stack.Push(Cpu.Instance.programCounter);
            Cpu.Instance.programCounter = addr;
        }

        public static void SkipIfXIsEqual(byte x, byte kk)
        {
            if (Cpu.Instance.registerV[x] == kk)
            {
                IncreaseProgramCount();
            }
        }

        public static void SkipIfXIsNotEqual(byte x, byte kk)
        {
            if (Cpu.Instance.registerV[x] != kk)
            {
                IncreaseProgramCount();
            }
        }

        public static void SkipIfXIsEqualY(byte x, byte y)
        {
            if (Cpu.Instance.registerV[x] == Cpu.Instance.registerV[y])
            {
                IncreaseProgramCount();
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

        public static void SetXToY(byte x, byte y)
        {
            Cpu.Instance.registerV[x] = Cpu.Instance.registerV[y];
        }
        public static void OrXAndY(byte x, byte y)
        {
            Cpu.Instance.registerV[x] = (byte)(Cpu.Instance.registerV[x] | Cpu.Instance.registerV[y]);
        }

        public static void AndXAndY(byte x, byte y)
        {
            Cpu.Instance.registerV[x] = (byte)(Cpu.Instance.registerV[x] & Cpu.Instance.registerV[y]);
        }

        public static void XorXAndY(byte x, byte y)
        {
            Cpu.Instance.registerV[x] = (byte)(Cpu.Instance.registerV[x] ^ Cpu.Instance.registerV[y]);
        }

        public static void AddXAndY(byte x, byte y)
        {
            ushort sum = (ushort)(x + y);
            if (sum > 0xFF)
            {
                Cpu.Instance.registerV[0xF] = 0x01;
                Cpu.Instance.registerV[x] = Convert.ToByte(sum);
            }
            else
            {
                Cpu.Instance.registerV[0xF] = 0x00;
                Cpu.Instance.registerV[x] = Convert.ToByte(sum);
            }
        }
    }
}
