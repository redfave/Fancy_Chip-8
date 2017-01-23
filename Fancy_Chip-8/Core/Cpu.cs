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
        private Cpu()
        {
            Reset();
        }
        private static readonly Lazy<Cpu> lazy = new Lazy<Cpu>(() => new Cpu());
        public static Cpu Instance { get { return lazy.Value; } }

        public byte[] memory;
        public byte[] registerV;
        public ushort index;
        public ushort programmCounter;
        public byte delayTimer;
        public byte soundTimer;
        public const ushort SceenWidth = 64;
        public const ushort SceenHeight = 32;
        public bool[] screen;
        public Stack<ushort> stack;


        public void Reset()
        {
            memory = new byte[4096];
            registerV = new byte[16];
            index = 0;
            programmCounter = 512;
            delayTimer = 0;
            soundTimer = 0;
            screen = new bool[SceenWidth*SceenHeight];
            stack = new Stack<ushort>(16);
            //Fill memory with sprites of HEX-chars
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
            Array.Copy(hexSprites, memory, hexSprites.Length);
        }
    }
}
