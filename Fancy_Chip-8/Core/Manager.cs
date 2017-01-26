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
        private Instructions myInstructions = new Instructions();

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
            /**
              nnn or addr - A 12-bit value, the lowest 12 bits of the instruction
              n or nibble - A 4-bit value, the lowest 4 bits of the instruction
              x - A 4-bit value, the lower 4 bits of the high byte of the instruction
              y - A 4-bit value, the upper 4 bits of the low byte of the instruction
              kk or byte - An 8-bit value, the lowest 8 bits of the instruction
              http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#3.0 
             **/
            ushort address = (ushort)(myInstructions.Cpu1.memory[myInstructions.Cpu1.programCounter] << 8 & 0x0FFF
                | myInstructions.Cpu1.memory[myInstructions.Cpu1.programCounter + 1]);
            byte lowerByte = myInstructions.Cpu1.memory[myInstructions.Cpu1.programCounter + 1];
            byte n = (byte)(myInstructions.Cpu1.memory[myInstructions.Cpu1.programCounter + 1] & 0x0F);
            byte y = (byte)(lowerByte >> 4);
            byte x = (byte)(myInstructions.Cpu1.memory[myInstructions.Cpu1.programCounter] & 0x0F);
            byte instructionType = (byte)(myInstructions.Cpu1.memory[myInstructions.Cpu1.programCounter]  >> 4);
            myInstructions.IncreaseProgramCount();
            switch (instructionType)
            {
                case 0x0:
                    if (lowerByte == 0xE0)
                    {
                        myInstructions.ClearDisplay();
                    }
                    else if (lowerByte == 0xEE)
                    {
                        myInstructions.ReturnFromSubroutine();
                    }
                    break;
                case 0x1:
                    myInstructions.JumpToAddress(address);
                    break;
                case 0x2:
                    myInstructions.CallSubroutine(address);
                    break;
                case 0x3:
                    myInstructions.SkipIfXIsEqual(x, lowerByte);
                    break;
                case 0x4:
                    myInstructions.SkipIfXIsNotEqual(x, lowerByte);
                    break;
                case 0x5:
                    myInstructions.SkipIfXIsEqualY(x, y);
                    break;
                case 0x6:
                    myInstructions.SetX(x, lowerByte);
                    break;
                case 0x7:
                    myInstructions.AddX(x, lowerByte);
                    break;
                case 0x8:
                    switch (n)
                    {
                        case 0x0:
                            myInstructions.SetXToY(x, y);
                            break;
                        case 0x1:
                            myInstructions.OrXAndY(x, y);
                            break;
                        case 0x2:
                            myInstructions.AndXAndY(x, y);
                            break;
                        case 0x3:
                            myInstructions.XorXAndY(x, y);
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
            myInstructions.WriteToMemory(program);
        }
    }
}
