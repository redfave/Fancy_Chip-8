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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Drawing.Drawing2D;

namespace Fancy_Chip_8.Core
{
    public class Manager : BindableBase
    {
        public Manager()
        {
            _outputScreen = new Bitmap(_system1.screenWidth * screenScaleFactor, _system1.screenHeight * screenScaleFactor);
            _emulationThread = new Thread(ExecuteCycle);
            _commandOpen = new DelegateCommand(CommandOpen_Executed, CommandOpen_CanExecute);
            _commandClose = new DelegateCommand(CommandClose_Executed, CommandClose_CanExecute);
        }

        private int screenScaleFactor = 16;
        private Bitmap _outputScreen;
        private Thread _emulationThread;
        private DelegateCommand _commandOpen, _commandClose;
        private Chip8System _system1 = new Chip8System();
        private bool _systemIsRunning, _programIsLoaded;

        public DelegateCommand commandOpen
        {
            get
            {
                return _commandOpen;
            }
        }

        public DelegateCommand commandClose
        {
            get
            {
                return _commandClose;
            }
        }

        public bool systemIsRunning
        {
            get
            {
                return _systemIsRunning;
            }

            set
            {
                if (value)
                {
                    if (_emulationThread.ThreadState == ThreadState.Suspended)
                    {
                        _emulationThread.Resume();
                    }
                    else
                    {
                        _emulationThread.Start();
                    }
                }
                else
                {
                    _emulationThread.Suspend();
                }
                _systemIsRunning = value;
                OnPropertyChanged();
            }
        }

        public bool programIsLoaded
        {
            get
            {
                return _programIsLoaded;
            }

            set
            {
                _programIsLoaded = value;
                OnPropertyChanged();
            }
        }

        public Bitmap outputScreen
        {
            get
            {
                return _outputScreen;
            }

            private set
            {
                _outputScreen = value;
                OnPropertyChanged();

            }
        }

        public byte pressedKey
        {
            get
            {
                return _system1.KeyValue;
            }
            set
            {
                _system1.KeyValue = value;
                OnPropertyChanged();
            }
        }

        private bool CommandOpen_CanExecute()
        {
            return !systemIsRunning;
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
            _emulationThread.Abort();
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
                        DrawBitMap();
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
                    _system1.DisplaySprite(x, y, n);
                    DrawBitMap();
                    break;
                case 0xE:
                    //TODO
                    break;
                case 0xF:
                    switch (lowerByte)
                    {
                        case 0x07:
                            //TODO
                            break;
                        case 0x0A:
                            //TODO
                            break;
                        case 0x15:
                            //TODO
                            break;
                        case 0x1E:
                            //TODO
                            break;
                        case 0x29:
                            //TODO
                            break;
                        case 0x33:
                            //TODO
                            break;
                        case 0x55:
                            //TODO
                            break;
                        case 0x65:
                            //TODO
                            break;
                    }
                    break;
            }
        }

        private void DrawBitMap()
        {
            Bitmap bitmap = new Bitmap(_system1.screenWidth, _system1.screenHeight);
            for (int i = 0; i < _system1.screenWidth; i++)
            {
                for (int j = 0; j < _system1.screenHeight; j++)
                {
                    if (_system1.screen[i, j])
                    {
                        bitmap.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }
            outputScreen = ResizeImage(bitmap, new System.Drawing.Size(_system1.screenWidth * screenScaleFactor, _system1.screenHeight * screenScaleFactor));
        }


        //http://stackoverflow.com/a/10839428/5329332
        private Bitmap ResizeImage(Bitmap imgToResize, System.Drawing.Size size)
        {
            Bitmap b = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
            }
            return b;

        }



        private void ExecuteCycle()
        {
            //TODO timing control
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
                programIsLoaded = true;
            }
            else
            {
                //TODO notify user
            }
        }
    }
}
