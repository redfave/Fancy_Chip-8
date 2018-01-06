using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public partial class Chip8System
    {

        public Chip8System()
        {
            logger = LogManager.GetCurrentClassLogger();
            Reset();
        }
        private static Logger logger;

        public byte[] Memory { get; private set; }
        public byte[] RegisterV { get; private set; }
        public ushort Index { get; private set; }
        public ushort ProgramCounter { get; private set; }
        public ushort ProgramStart { get; } = 512;
        public byte DelayTimer { get; private set; }
        public byte SoundTimer { get; private set; }
        public byte KeyValue { get; set; }
        public ushort ScreenWidth { get; } = 64;
        public ushort ScreenHeight { get; } = 32;
        public bool[,] Screen { get; private set; }
        public Stack<ushort> Stack { get; private set; }

        public void Reset()
        {
            Memory = new byte[4096];
            RegisterV = new byte[16];
            Index = 0;
            ProgramCounter = ProgramStart;
            DelayTimer = 0;
            SoundTimer = 0;
            Screen = new bool[ScreenWidth, ScreenHeight];
            Stack = new Stack<ushort>(16);
            //Fill _memory with sprites of HEX-chars
            byte[] hexSprites = { 0xF0, 0x90, 0x90, 0x90, 0xF0,  //0
                                0x20, 0x60, 0x20, 0x20, 0x70,    //1
                                0xF0, 0x10, 0xF0, 0x80, 0xF0,    //2
                                0xF0, 0x10, 0xF0, 0x10, 0xF0,    //3
                                0x90, 0x90, 0xF0, 0x10, 0x10,    //4
                                0xF0, 0x80, 0xF0, 0x10, 0xF0,    //5
                                0xF0, 0x80, 0xF0, 0x90, 0xF0,    //6
                                0xF0, 0x10, 0x20, 0x40, 0x40,    //7
                                0xF0, 0x90, 0xF0, 0x90, 0xF0,    //8
                                0xF0, 0x90, 0xF0, 0x10, 0xF0,    //9
                                0xF0, 0x90, 0xF0, 0x90, 0x90,    //A
                                0xE0, 0x90, 0xE0, 0x90, 0xE0,    //B
                                0xF0, 0x80, 0x80, 0x80, 0xF0,    //C
                                0xE0, 0x90, 0x90, 0x90, 0xE0,    //D
                                0xF0, 0x80, 0xF0, 0x80, 0xF0,    //E
                                0xF0, 0x80, 0xF0, 0x80, 0x80 };  //F
            Array.Copy(hexSprites, Memory, hexSprites.Length);
        }
    }
}
