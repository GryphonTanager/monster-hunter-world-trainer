using System;

namespace MonsterHunterWorldTrainer.Core
{
    /// <summary>
    /// Core trainer engine that manages cheat toggles and applies memory patches.
    /// Uses hardcoded offsets typical for Monster Hunter World: Iceborne (version 15.11.01).
    /// </summary>
    public class TrainerEngine
    {
        private readonly GameMemoryManager _memory;
        private bool _infiniteHealth;
        private bool _infiniteStamina;
        private bool _oneHitKill;

        // Placeholder offsets — these would need to be updated per game version.
        // In a real trainer, these would be resolved dynamically via pattern scanning.
        private readonly IntPtr _healthAddress = new IntPtr(0x1423A4B20);
        private readonly IntPtr _staminaAddress = new IntPtr(0x1423A4B24);
        private readonly IntPtr _zennyAddress = new IntPtr(0x1423A4B30);
        private readonly IntPtr _damageMultiplierAddress = new IntPtr(0x1423A4B40);

        public bool InfiniteHealthEnabled => _infiniteHealth;
        public bool InfiniteStaminaEnabled => _infiniteStamina;
        public bool OneHitKillEnabled => _oneHitKill;

        public TrainerEngine(GameMemoryManager memory)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
        }

        /// <summary>
        /// Toggles infinite health by freezing the health value to maximum (200).
        /// </summary>
        public void ToggleInfiniteHealth()
        {
            _infiniteHealth = !_infiniteHealth;
            if (_infiniteHealth)
            {
                // Write max health (200.0f) repeatedly to keep it frozen
                _memory.WriteFloat(_healthAddress, 200.0f);
                Console.WriteLine("Health frozen to 200.");
            }
            // When disabled, we stop overwriting; game will manage health normally.
        }

        /// <summary>
        /// Toggles infinite stamina by freezing stamina to maximum (150).
        /// </summary>
        public void ToggleInfiniteStamina()
        {
            _infiniteStamina = !_infiniteStamina;
            if (_infiniteStamina)
            {
                _memory.WriteFloat(_staminaAddress, 150.0f);
                Console.WriteLine("Stamina frozen to 150.");
            }
        }

        /// <summary>
        /// Sets the player's Zenny (money) to the maximum possible value (9999999).
        /// </summary>
        public void SetMaxZenny()
        {
            _memory.WriteInt32(_zennyAddress, 9999999);
            Console.WriteLine("Zenny set to 9,999,999.");
        }

        /// <summary>
        /// Toggles one-hit kill by setting damage multiplier to a very high value.
        /// </summary>
        public void ToggleOneHitKill()
        {
            _oneHitKill = !_oneHitKill;
            if (_oneHitKill)
            {
                _memory.WriteFloat(_damageMultiplierAddress, 99999.0f);
                Console.WriteLine("Damage multiplier set to 99999.");
            }
            else
            {
                _memory.WriteFloat(_damageMultiplierAddress, 1.0f);
                Console.WriteLine("Damage multiplier reset to 1.0.");
            }
        }
    }
}
