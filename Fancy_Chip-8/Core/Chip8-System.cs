using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core
{
    public sealed class Chip8_System
    {
        //Singleton pattern
        private Chip8_System() { }
        static readonly Lazy<Chip8_System> lazy = new Lazy<Chip8_System>(() => new Chip8_System());
        public static Chip8_System Instance { get { return lazy.Value; } }

        //4K RAM
        private byte[] memory = new byte[4096];
        //Registers from V1 to VF
        private byte[] v = new byte[16];
        //Index register
        ushort i;
        //Program counter
        ushort pc;
        //GFX output
        bool[] screen = new bool[64 * 32];

        public void test() { }
    }
}
