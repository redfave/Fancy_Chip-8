using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Fancy_Chip_8.Core
{
    public class Manager : BindableBase
    {

        public Manager()
        {
            _CommandOpen = new DelegateCommand(CommandOpen_Executed, CommandOpen_CanExecute);
            _CommandClose = new DelegateCommand(CommandClose_Executed, CommandClose_CanExecute);
        }

        DelegateCommand _CommandOpen, _CommandClose;
        private System system1 = new System();
        private bool _SystemIsRunning;

        public DelegateCommand CommandOpen
        {
            get
            {
                return _CommandOpen;
            }
        }

        public DelegateCommand CommandClose
        {
            get
            {
                return _CommandClose;
            }
        }

        public bool SystemIsRunning
        {
            get
            {
                return _SystemIsRunning;
            }

            set
            {
                SetProperty(ref _SystemIsRunning, value);
                Run();
            }
        }


        private bool CommandOpen_CanExecute()
        {
            return !SystemIsRunning;
        }

        private void CommandOpen_Executed()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                LoadProgram(File.ReadAllBytes(openFileDialog.FileName));
            }
            SystemIsRunning = true;
        }

        private bool CommandClose_CanExecute()
        {
            return true;
        }

        private void CommandClose_Executed()
        {
            Application.Current.Shutdown();
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
            ushort address = (ushort)(system1.memory[system1.programCounter] << 8 & 0x0FFF
                | system1.memory[system1.programCounter + 1]);
            byte lowerByte = system1.memory[system1.programCounter + 1];
            byte n = (byte)(system1.memory[system1.programCounter + 1] & 0x0F);
            byte y = (byte)(lowerByte >> 4);
            byte x = (byte)(system1.memory[system1.programCounter] & 0x0F);
            byte instructionType = (byte)(system1.memory[system1.programCounter] >> 4);
            system1.IncreaseProgramCount();
            switch (instructionType)
            {
                case 0x0:
                    if (lowerByte == 0xE0)
                    {
                        system1.ClearDisplay();
                    }
                    else if (lowerByte == 0xEE)
                    {
                        system1.ReturnFromSubroutine();
                    }
                    break;
                case 0x1:
                    system1.JumpToAddressDirectly(address);
                    break;
                case 0x2:
                    system1.CallSubroutine(address);
                    break;
                case 0x3:
                    system1.SkipIfXIsEqual(x, lowerByte);
                    break;
                case 0x4:
                    system1.SkipIfXIsNotEqual(x, lowerByte);
                    break;
                case 0x5:
                    system1.SkipIfXIsEqualY(x, y);
                    break;
                case 0x6:
                    system1.SetX(x, lowerByte);
                    break;
                case 0x7:
                    system1.AddX(x, lowerByte);
                    break;
                case 0x8:
                    switch (n)
                    {
                        case 0x0:
                            system1.SetXToY(x, y);
                            break;
                        case 0x1:
                            system1.OrXAndY(x, y);
                            break;
                        case 0x2:
                            system1.AndXAndY(x, y);
                            break;
                        case 0x3:
                            system1.XorXAndY(x, y);
                            break;
                        case 0x4:
                            system1.AddXAndY(x, y);
                            break;
                        case 0x5:
                            system1.SubYFromX(x, y);
                            break;
                        case 0x6:
                            system1.ShiftXRight(x);
                            break;
                        case 0x7:
                            system1.SubXFromY(x, y);
                            break;
                        case 0xE:
                            system1.ShiftXLeft(x);
                            break;
                    }
                    break;
                case 0x9:
                    system1.SkipNextInstruction(x, y);
                    break;
                case 0xA:
                    system1.SetIndex(address);
                    break;
                case 0xB:
                    system1.JumpToAdress(address);
                    break;
                case 0xC:
                    system1.SetXToRandomNumber(x, lowerByte);
                    break;
                case 0xD:

                    break;
                case 0xE:

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

            while (SystemIsRunning)
            {
                ExecuteCycle();
            }
        }

        public void LoadProgram(byte[] program)
        {
            //TODO check if file is legit
            if (program.Length <= system1.memory.Length - system1.programStart)
            {
                system1.WriteToMemory(program);
            }
            else
            {
                //TODO notify user
            }
        }
    }
}
