using System;
using Xunit;
using Moq;
using MonsterHunterWorldTrainer.Core;

namespace MonsterHunterWorldTrainer.Tests
{
    /// <summary>
    /// Unit tests for TrainerEngine using a mocked GameMemoryManager.
    /// Ensures toggles and memory writes behave as expected.
    /// </summary>
    public class TrainerEngineTests
    {
        [Fact]
        public void ToggleInfiniteHealth_ShouldSetHealthTo200()
        {
            // Arrange
            var mockMemory = new Mock<GameMemoryManager>("dummy");
            mockMemory.Setup(m => m.WriteFloat(It.IsAny<IntPtr>(), 200.0f));
            var engine = new TrainerEngine(mockMemory.Object);

            // Act
            engine.ToggleInfiniteHealth();

            // Assert
            Assert.True(engine.InfiniteHealthEnabled);
            mockMemory.Verify(m => m.WriteFloat(It.IsAny<IntPtr>(), 200.0f), Times.Once);
        }

        [Fact]
        public void ToggleInfiniteHealth_Twice_ShouldDisable()
        {
            // Arrange
            var mockMemory = new Mock<GameMemoryManager>("dummy");
            var engine = new TrainerEngine(mockMemory.Object);

            // Act
            engine.ToggleInfiniteHealth(); // enable
            engine.ToggleInfiniteHealth(); // disable

            // Assert
            Assert.False(engine.InfiniteHealthEnabled);
            // Should have written only once (on enable)
            mockMemory.Verify(m => m.WriteFloat(It.IsAny<IntPtr>(), 200.0f), Times.Once);
        }

        [Fact]
        public void SetMaxZenny_ShouldWrite9999999()
        {
            // Arrange
            var mockMemory = new Mock<GameMemoryManager>("dummy");
            mockMemory.Setup(m => m.WriteInt32(It.IsAny<IntPtr>(), 9999999));
            var engine = new TrainerEngine(mockMemory.Object);

            // Act
            engine.SetMaxZenny();

            // Assert
            mockMemory.Verify(m => m.WriteInt32(It.IsAny<IntPtr>(), 9999999), Times.Once);
        }

        [Fact]
        public void ToggleOneHitKill_ShouldSetDamageMultiplier()
        {
            // Arrange
            var mockMemory = new Mock<GameMemoryManager>("dummy");
            var engine = new TrainerEngine(mockMemory.Object);

            // Act
            engine.ToggleOneHitKill();

            // Assert
            Assert.True(engine.OneHitKillEnabled);
            mockMemory.Verify(m => m.WriteFloat(It.IsAny<IntPtr>(), 99999.0f), Times.Once);
        }

        [Fact]
        public void ToggleOneHitKill_Off_ShouldResetTo1()
        {
            // Arrange
            var mockMemory = new Mock<GameMemoryManager>("dummy");
            var engine = new TrainerEngine(mockMemory.Object);

            // Act
            engine.ToggleOneHitKill(); // on
            engine.ToggleOneHitKill(); // off

            // Assert
            Assert.False(engine.OneHitKillEnabled);
            mockMemory.Verify(m => m.WriteFloat(It.IsAny<IntPtr>(), 1.0f), Times.Once);
        }
    }
}
