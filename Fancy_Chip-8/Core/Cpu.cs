using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public sealed class Cpu
    {
        //Singleton pattern
        private Cpu() { }
        private static readonly Lazy<Cpu> lazy = new Lazy<Cpu>(() => new Cpu());
        public static Cpu Instance { get { return lazy.Value; } }

        public byte[] memory;
        public byte[] regV;
        public ushort index;
        public ushort programmCounter;
        public byte delayTimer;
        public byte soundTimer;
        public bool[,] screen;
        public byte stackPointer;
        public Stack<ushort> stack;


        public void Init()
        {
            memory = new byte[4096];
            regV = new byte[16];
            index = 0;
            programmCounter = 512;
            delayTimer = 0;
            soundTimer = 0;
            screen = new bool[64, 32];
            stackPointer = 0;
            stack = new Stack<ushort>(16);
            //Fill memory with sprites of HEX-chars
            byte[] hexSprites = { 0xF0, 0x90, 0x90, 0x90, 0xF0,
                                0x20, 0x60, 0x20, 0x20, 0x70,
                                0xF0, 0x10, 0xF0, 0x80, 0xF0,
                                0xF0, 0x10, 0xF0, 0x10, 0xF0,
                                0x90, 0x90, 0xF0, 0x10, 0x10,
                                0xF0, 0x80, 0xF0, 0x10, 0xF0,
                                0xF0, 0x80, 0xF0, 0x90, 0xF0,
                                0xF0, 0x10, 0x20, 0x40, 0x40,
                                0xF0, 0x90, 0xF0, 0x90, 0xF0,
                                0xF0, 0x90, 0xF0, 0x10, 0xF0,
                                0xF0, 0x90, 0xF0, 0x90, 0x90,
                                0xE0, 0x90, 0xE0, 0x90, 0xE0,
                                0xF0, 0x80, 0x80, 0x80, 0xF0,
                                0xE0, 0x90, 0x90, 0x90, 0xE0,
                                0xF0, 0x80, 0xF0, 0x80, 0xF0,
                                0xF0, 0x80, 0xF0, 0x80, 0x80 };
            Array.Copy(hexSprites, memory, hexSprites.Length);
        }
    }
}
