using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public partial class System
    {
        public void IncreaseProgramCount()
        {
            if (programCounter % 2 == 0)
            {
                programCounter += 2;
            }
            else
            {
                throw new Exception();
                //TODO throw error (something went horribly wrong)
            }
        }

        public void WriteToMemory(byte[] program)
        {
            Array.Copy(program, 0, memory, programCounter, program.Length);
        }

        public void ClearDisplay()
        {
            for (int i = 0; i < screen.Length; i++)
            { screen[i] = false; }
        }

        public void ReturnFromSubroutine()
        {
            programCounter = stack.Pop();
        }

        public void JumpToAddressDirectly(ushort addr)
        {
            programCounter = addr;
        }

        public void CallSubroutine(ushort addr)
        {
            stack.Push(programCounter);
            programCounter = addr;
        }

        public void SkipIfXIsEqual(byte x, byte kk)
        {
            if (registerV[x] == kk)
            {
                IncreaseProgramCount();
            }
        }

        public void SkipIfXIsNotEqual(byte x, byte kk)
        {
            if (registerV[x] != kk)
            {
                IncreaseProgramCount();
            }
        }

        public void SkipIfXIsEqualY(byte x, byte y)
        {
            if (registerV[x] == registerV[y])
            {
                IncreaseProgramCount();
            }
        }

        public void SetX(byte x, byte kk)
        {
            registerV[x] = kk;
        }

        public void AddX(byte x, byte kk)
        {
            registerV[x] += kk;
        }

        public void SetXToY(byte x, byte y)
        {
            registerV[x] = registerV[y];
        }
        public void OrXAndY(byte x, byte y)
        {
            registerV[x] = (byte)(registerV[x] | registerV[y]);
        }

        public void AndXAndY(byte x, byte y)
        {
            registerV[x] = (byte)(registerV[x] & registerV[y]);
        }

        public void XorXAndY(byte x, byte y)
        {
            registerV[x] = (byte)(registerV[x] ^ registerV[y]);
        }

        public void AddXAndY(byte x, byte y)
        {
            ushort sum = (ushort)(x + y);
            if (sum > 0xFF)
            {
                registerV[0xF] = 0x01;
                registerV[x] = Convert.ToByte(sum);
            }
            else
            {
                registerV[0xF] = 0x00;
                registerV[x] = Convert.ToByte(sum);
            }
        }

        public void SubYFromX(byte x, byte y)
        {
            if (registerV[x] > registerV[y])
            {
                registerV[0xF] = 0x01;
            }
            else
            {
                registerV[0xF] = 0x00;
            }
            registerV[x] -= registerV[y];
        }

        public void ShiftXRight(byte x)
        {
            registerV[0xF] = (byte)(registerV[x] & 0x01);
            registerV[x] = (byte)(registerV[x] >> 1);
        }


        public void SubXFromY(byte x, byte y)
        {
            if (registerV[y] > registerV[x])
            {
                registerV[0xF] = 0x01;
            }
            else
            {
                registerV[0xF] = 0x00;
            }
            registerV[x] = (byte)(registerV[y] - registerV[x]);
        }

        public void ShiftXLeft(byte x)
        {
            registerV[0xF] = (byte)(registerV[x] & 0x80 >> 7);
            registerV[x] = (byte)(registerV[x] << 1);
        }

        public void SkipNextInstruction(byte x, byte y)
        {
            if (registerV[x] != registerV[y])
            {
                IncreaseProgramCount();
            }
        }

        public void SetIndex(ushort address)
        {
            index = address;
        }

        public void JumpToAdress(ushort address)
        {
            programCounter = (byte)(address + registerV[0]);
        }

        public void SetXToRandomNumber(byte x, byte kk)
        {
            registerV[x] = (byte)(kk & new Random().Next(0x00, 0x100));
        }
    }
}
