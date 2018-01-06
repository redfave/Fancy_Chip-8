using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fancy_Chip_8.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fancy_Chip_8.Core.Tests
{
    [TestClass()]
    public class Chip8SystemTests
    {
        [TestMethod()]
        public void IncreaseProgramCountTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.IncreaseProgramCount();
            Assert.AreEqual(testSystem.ProgramCounter, testSystem.ProgramStart + 2);
        }

        [TestMethod()]
        public void IncreaseProgramCountExceptionTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.JumpToAdress(0x201);
            Assert.ThrowsException<InvalidOperationException>(() => testSystem.IncreaseProgramCount());
        }

        [TestMethod()]
        public void WriteToMemoryTest()
        {
            Random random = new Random();
            byte[] bogusProgramm = new byte[]{
                0x41, 0xF6, 0x3C, 0x57, 0x72, 0xF2, 0x3D, 0xD9
            };
            Chip8System testSystem = new Chip8System();
            testSystem.WriteToMemory(bogusProgramm);
            Assert.AreEqual(testSystem.Memory, bogusProgramm);
        }

        [TestMethod()]
        public void ClearDisplayTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReturnFromSubroutineTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void JumpToAddressDirectlyTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.JumpToAddressDirectly(0xFFF);
            Assert.AreEqual(testSystem.ProgramCounter, 0xFFF);
        }

        [TestMethod()]
        public void CallSubroutineTest()
        {
            Chip8System testSystem = new Chip8System();
            ushort programmCounterState = testSystem.ProgramCounter;
            testSystem.CallSubroutine(0xFFF);
            Assert.AreEqual(testSystem.ProgramCounter, 0xFFF);
            Assert.AreEqual(programmCounterState, testSystem.Stack.First());
            Assert.IsTrue(testSystem.Stack.Count == 1);
        }

        [TestMethod()]
        public void SkipIfXIsEqualTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SkipIfXIsNotEqualTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SkipIfXIsEqualYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetXTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddXTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(0x2, 0xF2);
            Assert.AreEqual(testSystem.RegisterV[0x2], 0xF2);
        }

        [TestMethod()]
        public void SetXToYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void OrXAndYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AndXAndYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void XorXAndYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddXAndYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SubYFromXTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ShiftXRightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SubXFromYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ShiftXLeftTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SkipNextInstructionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void JumpToAdressTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetXToRandomNumberTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisplaySpriteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetXToDelayTimeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetDelayTimerTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(0x2, 0xF2);
            testSystem.SetDelayTimer(0x2);
            Assert.AreEqual(testSystem.DelayTimer, 0xF2);
        }

        [TestMethod()]
        public void SetSoundTimerTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(0x2, 0xF2);
            testSystem.SetSoundTimer(0x2);
            Assert.AreEqual(testSystem.SoundTimer, 0xF2);
        }

        [TestMethod()]
        public void AddXToIndexTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetIndexToSpriteAddressTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetProgramCounterToXTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DelayTimerDecreaseTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(1, 5);
            testSystem.SetDelayTimer(1);
            byte delayTimerStartState = testSystem.DelayTimer;
            testSystem.DelayTimerDecrease();
            Assert.AreEqual(delayTimerStartState, testSystem.DelayTimer + 1);
        }

        [TestMethod()]
        public void SoundTimerDecreaseTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(1, 5);
            testSystem.SetSoundTimer(1);
            byte soundTimerStartState = testSystem.SoundTimer;
            testSystem.SoundTimerDecrease();
            Assert.AreEqual(soundTimerStartState, testSystem.SoundTimer + 1);
        }
    }
}