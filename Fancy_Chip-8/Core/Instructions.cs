using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public class Instructions
    {
        Random random = new Random();
        private Cpu _Cpu1 = new Cpu();

        public Cpu Cpu1
        {
            get
            {
                return _Cpu1;
            }
        }

        public void IncreaseProgramCount()
        {
            if (Cpu1.programCounter % 2 == 0)
            {
                Cpu1.programCounter += 2;
            }
            else
            {
                throw new Exception();
                //TODO throw error (something went horribly wrong)
            }
        }

        public void WriteToMemory(byte[] program)
        {
            //TODO check if file is legit
            Array.Copy(program, 0, Cpu1.memory, Cpu1.programCounter, program.Length);
        }

        public void ClearDisplay()
        {
            for (int i = 0; i < Cpu1.screen.Length; i++)
            { Cpu1.screen[i] = false; }
        }

        public void ReturnFromSubroutine()
        {
            Cpu1.programCounter = Cpu1.stack.Pop();
        }

        public void JumpToAddressDirectly(ushort addr)
        {
            Cpu1.programCounter = addr;
        }

        public void CallSubroutine(ushort addr)
        {
            Cpu1.stack.Push(Cpu1.programCounter);
            Cpu1.programCounter = addr;
        }

        public void SkipIfXIsEqual(byte x, byte kk)
        {
            if (Cpu1.registerV[x] == kk)
            {
                IncreaseProgramCount();
            }
        }

        public void SkipIfXIsNotEqual(byte x, byte kk)
        {
            if (Cpu1.registerV[x] != kk)
            {
                IncreaseProgramCount();
            }
        }

        public void SkipIfXIsEqualY(byte x, byte y)
        {
            if (Cpu1.registerV[x] == Cpu1.registerV[y])
            {
                IncreaseProgramCount();
            }
        }

        public void SetX(byte x, byte kk)
        {
            Cpu1.registerV[x] = kk;
        }

        public void AddX(byte x, byte kk)
        {
            Cpu1.registerV[x] += kk;
        }

        public void SetXToY(byte x, byte y)
        {
            Cpu1.registerV[x] = Cpu1.registerV[y];
        }
        public void OrXAndY(byte x, byte y)
        {
            Cpu1.registerV[x] = (byte)(Cpu1.registerV[x] | Cpu1.registerV[y]);
        }

        public void AndXAndY(byte x, byte y)
        {
            Cpu1.registerV[x] = (byte)(Cpu1.registerV[x] & Cpu1.registerV[y]);
        }

        public void XorXAndY(byte x, byte y)
        {
            Cpu1.registerV[x] = (byte)(Cpu1.registerV[x] ^ Cpu1.registerV[y]);
        }

        public void AddXAndY(byte x, byte y)
        {
            ushort sum = (ushort)(x + y);
            if (sum > 0xFF)
            {
                Cpu1.registerV[0xF] = 0x01;
                Cpu1.registerV[x] = Convert.ToByte(sum);
            }
            else
            {
                Cpu1.registerV[0xF] = 0x00;
                Cpu1.registerV[x] = Convert.ToByte(sum);
            }
        }

        public void SubYFromX(byte x, byte y)
        {
            if (Cpu1.registerV[x] > Cpu1.registerV[y])
            {
                Cpu1.registerV[0xF] = 0x01;
            }
            else
            {
                Cpu1.registerV[0xF] = 0x00;
            }
            Cpu1.registerV[x] -= Cpu1.registerV[y];
        }

        public void ShiftXRight(byte x)
        {
            Cpu1.registerV[0xF] = (byte)(Cpu1.registerV[x] & 0x01);
            Cpu1.registerV[x] = (byte)(Cpu1.registerV[x] >> 1);
        }


        public void SubXFromY(byte x, byte y)
        {
            if (Cpu1.registerV[y] > Cpu1.registerV[x])
            {
                Cpu1.registerV[0xF] = 0x01;
            }
            else
            {
                Cpu1.registerV[0xF] = 0x00;
            }
            Cpu1.registerV[x] = (byte)(Cpu1.registerV[y] - Cpu1.registerV[x]);
        }

        public void ShiftXLeft(byte x)
        {
            Cpu1.registerV[0xF] = (byte)(Cpu1.registerV[x] & 0x80 >> 7);
            Cpu1.registerV[x] = (byte)(Cpu1.registerV[x] << 1);
        }

        public void SkipNextInstruction(byte x, byte y)
        {
            if (Cpu1.registerV[x] != Cpu1.registerV[y])
            {
                IncreaseProgramCount();
            }
        }

        public void SetIndex(ushort address)
        {
            Cpu1.index = address;
        }

        public void JumpToAdress(ushort address)
        {
            Cpu1.programCounter = (byte)(address + Cpu1.registerV[0]);
        }

        public void SetXToRandomNumber(byte x, byte kk)
        {
            Cpu1.registerV[x] = (byte)(kk & random.Next(0x00, 0x100));
        }
    }
}
