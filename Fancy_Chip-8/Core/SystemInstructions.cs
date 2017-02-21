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
            logger.Debug("");
            Array.Copy(program, 0, memory, programCounter, program.Length);
        }

        public void ClearDisplay()
        {
            for (int i = 0; i < screenWidth; i++)
            {
                for (int j = 0; j < screenHeight; j++)
                {
                    screen[i, j] = false;
                }
            }
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

        public void DisplaySprite(byte x, byte y, byte n)
        {
            registerV[0x0F] = Convert.ToByte(false);
            byte[] sprite = new byte[n + 1];
            Array.Copy(memory, programCounter, sprite, 0, Convert.ToInt32(n));
            for (int i = 0; i < sprite.Length; i++)
            {
                BitArray spriteRow = new BitArray(new byte[] { sprite[i] });
                for (int j = 0; j < 8; j++)
                {
                    if (x + j + 2 <= screenWidth && y + i + 2 <= screenHeight)
                    {
                        SetScreenPixel(spriteRow.Get(j), x, y);
                    }
                    else if (x + j + 2 > screenWidth && y + i + 2 > screenHeight)
                    {
                        SetScreenPixel(spriteRow.Get(j), Convert.ToByte(j - screenWidth - 1 - x), Convert.ToByte(i - screenHeight - 1 - y));
                    }
                    else if (x + j + 2 > screenWidth)
                    {
                        SetScreenPixel(spriteRow.Get(j), Convert.ToByte(j - screenWidth - 1 - x), y);
                    }
                    else if (y + i + 2 > screenHeight)
                    {
                        SetScreenPixel(spriteRow.Get(j), x, Convert.ToByte(i - screenHeight - 1 - y));
                    }
                }
            }
        }

        public void SetXToDelayTime(byte x)
        {
            registerV[x] = delayTimer;
        }

        public void SetDelayTimer(byte x)
        {
            delayTimer = registerV[x];
        }

        public void SetSoundTimer(byte x)
        {
            soundTimer = registerV[x];
        }

        public void AddXToIndex(byte x)
        {
            index += registerV[x];
        }

        public void SetIndexToSpriteAddress(byte x)
        {
            index = Convert.ToUInt16(x * 5);
        }

        public void SetProgramCounterToX(byte x)
        {
            memory[programCounter] = Convert.ToByte(GetNthDigit(Convert.ToInt32(registerV[x]), 3, 1));
            memory[programCounter + 1] = Convert.ToByte(GetNthDigit(Convert.ToInt32(registerV[x]), 2, 1));
            memory[programCounter + 2] = Convert.ToByte(GetNthDigit(Convert.ToInt32(registerV[x]), 1, 1));
        }

        private void SetScreenPixel(bool pixelValue, byte screenX, byte screenY)
        {
            bool oldPixelValue = screen[screenX, screenY];
            screen[screenX, screenY] ^= pixelValue;
            if (oldPixelValue != screen[screenX, screenY])
            {
                registerV[0x0F] = Convert.ToByte(true);
            }
        }

        //http://stackoverflow.com/a/16094891/5329332
        private int GetNthDigit(int number, int highestDigit, int numDigits)
        {
            return (number / Convert.ToInt32(Math.Pow(10, highestDigit - numDigits)) % Convert.ToInt32(Math.Pow(10, numDigits)));
        }

    }
}

