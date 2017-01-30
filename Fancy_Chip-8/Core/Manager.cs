using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Fancy_Chip_8.Core
{
    public class Manager : BindableBase
    {
        public Manager()
        {
            emulationThread = new Thread(ExecuteCycle);
            _CommandOpen = new DelegateCommand(CommandOpen_Executed, CommandOpen_CanExecute);
            _CommandClose = new DelegateCommand(CommandClose_Executed, CommandClose_CanExecute);
        }

        Thread emulationThread;
        DelegateCommand _CommandOpen, _CommandClose;
        private System _system1 = new System();
        private bool _SystemIsRunning, _ProgramIsLoaded;

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
                if (value)
                {
                    if (emulationThread.ThreadState == ThreadState.Suspended)
                    {
                        emulationThread.Resume();
                    }
                    else
                    {
                        emulationThread.Start();
                    }
                }
                else
                {
                    emulationThread.Suspend();
                }
            }
        }

        public bool ProgramIsLoaded
        {
            get
            {
                return _ProgramIsLoaded;
            }

            set
            {
                SetProperty(ref _ProgramIsLoaded, value);
                _ProgramIsLoaded = value;
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
            ushort address = (ushort)(_system1.memory[_system1.programCounter] << 8 & 0x0FFF
                | _system1.memory[_system1.programCounter + 1]);
            byte lowerByte = _system1.memory[_system1.programCounter + 1];
            byte n = (byte)(_system1.memory[_system1.programCounter + 1] & 0x0F);
            byte y = (byte)(lowerByte >> 4);
            byte x = (byte)(_system1.memory[_system1.programCounter] & 0x0F);
            byte instructionType = (byte)(_system1.memory[_system1.programCounter] >> 4);
            _system1.IncreaseProgramCount();
            switch (instructionType)
            {
                case 0x0:
                    if (lowerByte == 0xE0)
                    {
                        _system1.ClearDisplay();
                    }
                    else if (lowerByte == 0xEE)
                    {
                        _system1.ReturnFromSubroutine();
                    }
                    break;
                case 0x1:
                    _system1.JumpToAddressDirectly(address);
                    break;
                case 0x2:
                    _system1.CallSubroutine(address);
                    break;
                case 0x3:
                    _system1.SkipIfXIsEqual(x, lowerByte);
                    break;
                case 0x4:
                    _system1.SkipIfXIsNotEqual(x, lowerByte);
                    break;
                case 0x5:
                    _system1.SkipIfXIsEqualY(x, y);
                    break;
                case 0x6:
                    _system1.SetX(x, lowerByte);
                    break;
                case 0x7:
                    _system1.AddX(x, lowerByte);
                    break;
                case 0x8:
                    switch (n)
                    {
                        case 0x0:
                            _system1.SetXToY(x, y);
                            break;
                        case 0x1:
                            _system1.OrXAndY(x, y);
                            break;
                        case 0x2:
                            _system1.AndXAndY(x, y);
                            break;
                        case 0x3:
                            _system1.XorXAndY(x, y);
                            break;
                        case 0x4:
                            _system1.AddXAndY(x, y);
                            break;
                        case 0x5:
                            _system1.SubYFromX(x, y);
                            break;
                        case 0x6:
                            _system1.ShiftXRight(x);
                            break;
                        case 0x7:
                            _system1.SubXFromY(x, y);
                            break;
                        case 0xE:
                            _system1.ShiftXLeft(x);
                            break;
                    }
                    break;
                case 0x9:
                    _system1.SkipNextInstruction(x, y);
                    break;
                case 0xA:
                    _system1.SetIndex(address);
                    break;
                case 0xB:
                    _system1.JumpToAdress(address);
                    break;
                case 0xC:
                    _system1.SetXToRandomNumber(x, lowerByte);
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
            while (true)
            {
                Interpret();
            }
        }

        public void LoadProgram(byte[] program)
        {
            _system1.Reset();
            //TODO check if file is legit
            if (program.Length <= _system1.memory.Length - _system1.programStart)
            {
                _system1.WriteToMemory(program);
                ProgramIsLoaded = true;
            }
            else
            {
                //TODO notify user
            }
        }
    }
}
