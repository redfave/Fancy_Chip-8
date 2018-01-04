using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Collections;

namespace Fancy_Chip_8.Core
{
    public partial class Chip8System
    {
        public void IncreaseProgramCount()
        {
            if (ProgramCounter % 2 == 0)
            {
                ProgramCounter += 2;
            }
            else
            {
                throw new InvalidOperationException();
                //TODO throw error (something went horribly wrong)
            }
        }

        public void WriteToMemory(byte[] program)
        {
            logger.Debug("");
            Array.Copy(program, 0, Memory, ProgramCounter, program.Length);
        }

        public void ClearDisplay()
        {
            for (int i = 0; i < ScreenWidth; i++)
            {
                for (int j = 0; j < ScreenHeight; j++)
                {
                    Screen[i, j] = false;
                }
            }
        }

        public void ReturnFromSubroutine()
        {
            ProgramCounter = Stack.Pop();
        }

        public void JumpToAddressDirectly(ushort addr)
        {
            ProgramCounter = addr;
        }

        public void CallSubroutine(ushort addr)
        {
            Stack.Push(ProgramCounter);
            ProgramCounter = addr;
        }

        public void SkipIfXIsEqual(byte x, byte kk)
        {
            if (RegisterV[x] == kk)
            {
                IncreaseProgramCount();
            }
        }

        public void SkipIfXIsNotEqual(byte x, byte kk)
        {
            if (RegisterV[x] != kk)
            {
                IncreaseProgramCount();
            }
        }

        public void SkipIfXIsEqualY(byte x, byte y)
        {
            if (RegisterV[x] == RegisterV[y])
            {
                IncreaseProgramCount();
            }
        }

        public void SetX(byte x, byte kk)
        {
            RegisterV[x] = kk;
        }

        public void AddX(byte x, byte kk)
        {
            RegisterV[x] += kk;
        }

        public void SetXToY(byte x, byte y)
        {
            RegisterV[x] = RegisterV[y];
        }
        public void OrXAndY(byte x, byte y)
        {
            RegisterV[x] = (byte)(RegisterV[x] | RegisterV[y]);
        }

        public void AndXAndY(byte x, byte y)
        {
            RegisterV[x] = (byte)(RegisterV[x] & RegisterV[y]);
        }

        public void XorXAndY(byte x, byte y)
        {
            RegisterV[x] = (byte)(RegisterV[x] ^ RegisterV[y]);
        }

        public void AddXAndY(byte x, byte y)
        {
            ushort sum = (ushort)(x + y);
            if (sum > 0xFF)
            {
                RegisterV[0xF] = 0x01;
                RegisterV[x] = Convert.ToByte(sum);
            }
            else
            {
                RegisterV[0xF] = 0x00;
                RegisterV[x] = Convert.ToByte(sum);
            }
        }

        public void SubYFromX(byte x, byte y)
        {
            if (RegisterV[x] > RegisterV[y])
            {
                RegisterV[0xF] = 0x01;
            }
            else
            {
                RegisterV[0xF] = 0x00;
            }
            RegisterV[x] -= RegisterV[y];
        }

        public void ShiftXRight(byte x)
        {
            RegisterV[0xF] = (byte)(RegisterV[x] & 0x01);
            RegisterV[x] = (byte)(RegisterV[x] >> 1);
        }


        public void SubXFromY(byte x, byte y)
        {
            if (RegisterV[y] > RegisterV[x])
            {
                RegisterV[0xF] = 0x01;
            }
            else
            {
                RegisterV[0xF] = 0x00;
            }
            RegisterV[x] = (byte)(RegisterV[y] - RegisterV[x]);
        }

        public void ShiftXLeft(byte x)
        {
            RegisterV[0xF] = (byte)(RegisterV[x] & 0x80 >> 7);
            RegisterV[x] = (byte)(RegisterV[x] << 1);
        }

        public void SkipNextInstruction(byte x, byte y)
        {
            if (RegisterV[x] != RegisterV[y])
            {
                IncreaseProgramCount();
            }
        }

        public void SetIndex(ushort address)
        {
            Index = address;
        }

        public void JumpToAdress(ushort address)
        {
            ProgramCounter = (byte)(address + RegisterV[0]);
        }

        public void SetXToRandomNumber(byte x, byte kk)
        {
            RegisterV[x] = (byte)(kk & new Random().Next(0x00, 0x100));
        }

        public void DisplaySprite(byte x, byte y, byte n)
        {
            RegisterV[0x0F] = Convert.ToByte(false);
            byte[] sprite = new byte[n + 1];
            Array.Copy(Memory, ProgramCounter, sprite, 0, Convert.ToInt32(n));
            for (int i = 0; i < sprite.Length; i++)
            {
                BitArray spriteRow = new BitArray(new byte[] { sprite[i] });
                for (int j = 0; j < 8; j++)
                {
                    if (x + j + 2 <= ScreenWidth && y + i + 2 <= ScreenHeight)
                    {
                        SetScreenPixel(spriteRow.Get(j), x, y);
                    }
                    else if (x + j + 2 > ScreenWidth && y + i + 2 > ScreenHeight)
                    {
                        SetScreenPixel(spriteRow.Get(j), Convert.ToByte(j - ScreenWidth - 1 - x), Convert.ToByte(i - ScreenHeight - 1 - y));
                    }
                    else if (x + j + 2 > ScreenWidth)
                    {
                        SetScreenPixel(spriteRow.Get(j), Convert.ToByte(j - ScreenWidth - 1 - x), y);
                    }
                    else if (y + i + 2 > ScreenHeight)
                    {
                        SetScreenPixel(spriteRow.Get(j), x, Convert.ToByte(i - ScreenHeight - 1 - y));
                    }
                }
            }
        }

        public void SetXToDelayTime(byte x)
        {
            RegisterV[x] = DelayTimer;
        }

        public void SetDelayTimer(byte x)
        {
            DelayTimer = RegisterV[x];
        }

        public void SetSoundTimer(byte x)
        {
            SoundTimer = RegisterV[x];
        }

        public void AddXToIndex(byte x)
        {
            Index += RegisterV[x];
        }

        public void SetIndexToSpriteAddress(byte x)
        {
            Index = Convert.ToUInt16(x * 5);
        }

        public void SetProgramCounterToX(byte x)
        {
            Memory[ProgramCounter] = Convert.ToByte(GetNthDigit(Convert.ToInt32(RegisterV[x]), 3, 1));
            Memory[ProgramCounter + 1] = Convert.ToByte(GetNthDigit(Convert.ToInt32(RegisterV[x]), 2, 1));
            Memory[ProgramCounter + 2] = Convert.ToByte(GetNthDigit(Convert.ToInt32(RegisterV[x]), 1, 1));
        }

        private void SetScreenPixel(bool pixelValue, byte screenX, byte screenY)
        {
            bool oldPixelValue = Screen[screenX, screenY];
            Screen[screenX, screenY] ^= pixelValue;
            if (oldPixelValue != Screen[screenX, screenY])
            {
                RegisterV[0x0F] = Convert.ToByte(true);
            }
        }

        public void DelayTimerDecrease() => DelayTimer--;

        public void SoundTimerDecrease() => SoundTimer--;

        //http://stackoverflow.com/a/16094891/5329332
        private int GetNthDigit(int number, int highestDigit, int numDigits)
        {
            return (number / Convert.ToInt32(Math.Pow(10, highestDigit - numDigits)) % Convert.ToInt32(Math.Pow(10, numDigits)));
        }

    }
}

