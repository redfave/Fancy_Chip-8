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
            Assert.AreEqual(testSystem.ProgramStart + 2, testSystem.ProgramCounter);
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
            byte[] bogusProgramm = new byte[]{
                0x41, 0xF6, 0x3C, 0x57, 0x72, 0xF2, 0x3D, 0xD9
            };
            Chip8System testSystem = new Chip8System();
            testSystem.WriteToMemory(bogusProgramm);
            CollectionAssert.AreEqual(bogusProgramm.ToList(), testSystem.Memory.Skip(testSystem.ProgramStart).Take(8).ToList());
        }

        [TestMethod()]
        public void ClearDisplayTest()
        {
            //TODO Test should fail in this state 
            Chip8System testSystem = new Chip8System();
            testSystem.SetIndexToSpriteAddress(0xA);
            testSystem.DisplaySprite(10, 10, 5);
            foreach (bool x in testSystem.Screen)
            {
                if (x == true)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod()]
        public void ReturnFromSubroutineTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.JumpToAddressDirectly(0xFF1);
            ushort programmCounterState = testSystem.ProgramCounter;
            testSystem.CallSubroutine(0xFFF);
            testSystem.AddX(0x02, 0xF2);
            testSystem.SetMemoryToX(0x02);
            testSystem.ReturnFromSubroutine();
            Assert.AreEqual(programmCounterState, testSystem.ProgramCounter);
            Assert.AreEqual(0, testSystem.Stack.Count);
        }

        [TestMethod()]
        public void JumpToAddressDirectlyTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.JumpToAddressDirectly(0xFFF);
            Assert.AreEqual(0xFFF, testSystem.ProgramCounter);
        }

        [TestMethod()]
        public void CallSubroutineTest()
        {
            Chip8System testSystem = new Chip8System();
            ushort programmCounterState = testSystem.ProgramCounter;
            testSystem.CallSubroutine(0xFFF);
            Assert.AreEqual(0xFFF, testSystem.ProgramCounter);
            Assert.AreEqual(testSystem.Stack.First(), programmCounterState);
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
            Chip8System testSystem = new Chip8System();
            testSystem.SetX(0x2, 0xF2);
            Assert.AreEqual(0xF2, testSystem.RegisterV[0x2]);
        }

        [TestMethod()]
        public void AddXTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(0x2, 0xF2);
            Assert.AreEqual(0xF2, testSystem.RegisterV[0x2]);
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
        public void AddXAndYWithoutCarrier()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.SetX(0x1, 0x1);
            testSystem.SetX(0x2, 0x2);
            testSystem.AddXAndY(0x1, 0x2);
            Assert.AreEqual(0x3, testSystem.RegisterV[0x1]);
            Assert.AreEqual(0x0, testSystem.RegisterV[0xF]);
        }

        [TestMethod()]
        public void AddXAndYWithCarrier()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.SetX(0x1, 0x4);
            testSystem.SetX(0x2, 0xFF);
            testSystem.AddXAndY(0x1, 0x2);
            Assert.AreEqual(0x04 - 0x01, testSystem.RegisterV[0x1]);
            Assert.AreEqual(0x1, testSystem.RegisterV[0xF]);
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
            Chip8System testSystem = new Chip8System();
            testSystem.SetIndexToSpriteAddress(0xA);
            Assert.AreEqual(0xA * 5, testSystem.Index);
        }

        [TestMethod()]
        public void SetIndexToSpriteAddressExceptionTest()
        {
            Chip8System testSystem = new Chip8System();
            Assert.ThrowsException<ArgumentException>(() => testSystem.SetIndexToSpriteAddress(0x10));
        }

        [TestMethod()]
        public void SetMemoryToXThreeDigitTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.SetIndex(0xF2);
            testSystem.SetX(0x02, 0xC4);
            testSystem.SetMemoryToX(0x02);
            Assert.AreEqual(1, testSystem.Memory[testSystem.Index]);
            Assert.AreEqual(9, testSystem.Memory[testSystem.Index + 1]);
            Assert.AreEqual(6, testSystem.Memory[testSystem.Index + 2]);
        }

        [TestMethod()]
        public void SetMemoryToXTwoDigitTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.SetIndex(0xF2);
            testSystem.SetX(0x02, 0x60);
            testSystem.SetMemoryToX(0x02);
            Assert.AreEqual(0, testSystem.Memory[testSystem.Index]);
            Assert.AreEqual(9, testSystem.Memory[testSystem.Index + 1]);
            Assert.AreEqual(6, testSystem.Memory[testSystem.Index + 2]);
        }

        [TestMethod()]
        public void SetMemoryToXOneDigitTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.SetIndex(0xF2);
            testSystem.SetX(0x02, 0x6);
            testSystem.SetMemoryToX(0x02);
            Assert.AreEqual(testSystem.Memory[testSystem.Index], 0);
            Assert.AreEqual(0, testSystem.Memory[testSystem.Index]);
            Assert.AreEqual(0, testSystem.Memory[testSystem.Index + 1]);
            Assert.AreEqual(6, testSystem.Memory[testSystem.Index + 2]);
        }

        [TestMethod()]
        public void SetMemoryToXExceptionTest()
        {
            Chip8System testSystem = new Chip8System();
            Assert.ThrowsException<ArgumentException>(() => testSystem.SetMemoryToX(0x10));
        }

        [TestMethod()]
        public void DelayTimerDecreaseTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(1, 5);
            testSystem.SetDelayTimer(1);
            byte delayTimerStartState = testSystem.DelayTimer;
            testSystem.DelayTimerDecrease();
            Assert.AreEqual(testSystem.DelayTimer + 1, delayTimerStartState);
        }

        [TestMethod()]
        public void SoundTimerDecreaseTest()
        {
            Chip8System testSystem = new Chip8System();
            testSystem.AddX(1, 5);
            testSystem.SetSoundTimer(1);
            byte soundTimerStartState = testSystem.SoundTimer;
            testSystem.SoundTimerDecrease();
            Assert.AreEqual(testSystem.SoundTimer + 1, soundTimerStartState);
        }
    }
}