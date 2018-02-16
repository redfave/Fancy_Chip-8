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
using System.Windows.Threading;
using Fancy_Chip_8.Helper;

namespace Fancy_Chip_8.Core
{
    public class Manager : BindableBase
    {
        public Manager()
        {
            _outputScreen = new Bitmap(_system1.ScreenWidth * screenScaleFactor, _system1.ScreenHeight * screenScaleFactor);
            _loopSoundHelper = new LoopSoundHelper();
            _loopSoundHelper.SetLoopSound(400);
            //emulate 500Hz clock speed
            _cpuTimer = new DispatcherTimer();
            _cpuTimer.Interval = new TimeSpan(0, 0, 0, 0, 2);
            _cpuTimer.Tick += new EventHandler(CpuTimer_Tick);
            //emulate nearly 60Hz timer speed
            _outputTimer = new DispatcherTimer();
            _outputTimer.Interval = new TimeSpan(16667000 / 100); //approximation to 16,7ms
            _outputTimer.Tick += new EventHandler(OutputTimer_Tick);
            _commandOpen = new DelegateCommand(CommandOpen_Executed, CommandOpen_CanExecute);
            _commandClose = new DelegateCommand(CommandClose_Executed, CommandClose_CanExecute);
            _commandOpenAboutWindow = new DelegateCommand(CommandOpenAboutWindow_Executed);
            _commandRunControl = new DelegateCommand(CommandRunControl_Executed, CommandRunControl_CanExecute);
            _commandStop = new DelegateCommand(CommandStop_Executed, CommandStop_CanExecute);

            _commandOpen.ObservesProperty(() => systemIsRunning);
            _commandRunControl.ObservesProperty(() => programIsLoaded);
            _commandStop.ObservesProperty(() => systemIsRunning);
            _commandStop.ObservesProperty(() => programIsLoaded);

        }

        private int screenScaleFactor = 16;
        private Bitmap _outputScreen;
        private LoopSoundHelper _loopSoundHelper;
        private DispatcherTimer _cpuTimer;
        //used for delay, as well as sound and gfx output
        private DispatcherTimer _outputTimer;
        private DelegateCommand _commandOpen, _commandClose, _commandOpenAboutWindow, _commandRunControl, _commandStop;
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

        public DelegateCommand commandOpenAboutWindow
        {
            get
            {
                return _commandOpenAboutWindow;
            }
        }

        public DelegateCommand commandRunControl
        {
            get
            {
                return _commandRunControl;
            }
        }

        public DelegateCommand commandStop
        {
            get
            {
                return _commandStop;
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
                    _cpuTimer.Start();
                    _outputTimer.Start();
                }
                else
                {
                    _cpuTimer.Stop();
                    _outputTimer.Stop();
                }
                _systemIsRunning = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
            _cpuTimer.Stop();
            _outputTimer.Stop();
            Application.Current.Shutdown();
        }

        private void CommandOpenAboutWindow_Executed()
        {
            new AboutWindow().Show();
        }

        private bool CommandRunControl_CanExecute()
        {
            return programIsLoaded;
        }

        private void CommandRunControl_Executed()
        {
            systemIsRunning = !systemIsRunning;
        }

        private bool CommandStop_CanExecute()
        {
            return programIsLoaded & systemIsRunning;
        }

        private void CommandStop_Executed()
        {
            systemIsRunning = false;
            programIsLoaded = false;
            _system1.ClearDisplay();
            _system1.Reset();
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
            ushort address = (ushort)(_system1.Memory[_system1.ProgramCounter] << 8 & 0x0FFF
                | _system1.Memory[_system1.ProgramCounter + 1]);
            byte lowerByte = _system1.Memory[_system1.ProgramCounter + 1];
            byte n = (byte)(_system1.Memory[_system1.ProgramCounter + 1] & 0x0F);
            byte y = (byte)(lowerByte >> 4);
            byte x = (byte)(_system1.Memory[_system1.ProgramCounter] & 0x0F);
            byte instructionType = (byte)(_system1.Memory[_system1.ProgramCounter] >> 4);
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
                    _system1.DisplaySprite(x, y, n);
                    break;
                case 0xE:
                    //TODO
                    break;
                case 0xF:
                    switch (lowerByte)
                    {
                        case 0x07:
                            _system1.SetXToDelayTime(x);
                            break;
                        case 0x0A:
                            //TODO
                            break;
                        case 0x15:
                            _system1.SetDelayTimer(x);
                            break;
                        case 0x18:
                            _system1.SetSoundTimer(x);
                            break;
                        case 0x1E:
                            _system1.AddXToIndex(x);
                            break;
                        case 0x29:
                            _system1.SetIndexToSpriteAddress(x);
                            break;
                        case 0x33:
                            _system1.SetMemoryToX(x);
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
            _system1.IncreaseProgramCount();
        }

        private void DrawBitMap()
        {
            Bitmap bitmap = new Bitmap(_system1.ScreenWidth, _system1.ScreenHeight);
            for (int i = 0; i < _system1.ScreenWidth; i++)
            {
                for (int j = 0; j < _system1.ScreenHeight; j++)
                {
                    if (_system1.Screen[i, j])
                    {
                        bitmap.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }
            outputScreen = ResizeImage(bitmap, new System.Drawing.Size(_system1.ScreenWidth * screenScaleFactor, _system1.ScreenHeight * screenScaleFactor));
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

        private void CpuTimer_Tick(object sender, EventArgs e)
        {
            Interpret();
        }

        private void OutputTimer_Tick(object sender, EventArgs e)
        {
            if (_system1.DelayTimer > 0)
            {
                _system1.DelayTimerDecrease();
            }
            if (_system1.SoundTimer > 0)
            {
                _system1.SoundTimerDecrease();
                _loopSoundHelper.PlayLoopSound();
            }
            else
            {
                _loopSoundHelper.StopLoopSound();
            }
            DrawBitMap();
        }

        public void LoadProgram(byte[] program)
        {
            _system1.Reset();
            //TODO check if file is legit
            if (program.Length <= _system1.Memory.Length - _system1.ProgramStart)
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
