from typing import Dict, Callable, Optional
from .memory_reader import MemoryReader


class TrainerFeatures:
    """Provides togglable cheats for Monster Hunter World: Iceborne."""

    # Example static offsets (these would need to be updated per game version)
    HEALTH_OFFSETS = (0x12345678, 0x90, 0x4)  # Placeholder
    STAMINA_OFFSETS = (0x12345678, 0x90, 0x8)  # Placeholder
    ZENNY_OFFSETS = (0x87654321, 0x10, 0x0)   # Placeholder

    def __init__(self, memory_reader: MemoryReader):
        self.mr = memory_reader
        self._features: Dict[str, bool] = {
            "infinite_health": False,
            "infinite_stamina": False,
            "max_zenny": False,
        }

    def toggle_feature(self, feature_name: str, enabled: bool) -> bool:
        """Enable or disable a feature by name.
        
        Args:
            feature_name: Name of the feature (e.g., 'infinite_health').
            enabled: True to enable, False to disable.
        
        Returns:
            True if toggle was successful, False otherwise.
        """
        if feature_name not in self._features:
            print(f"Unknown feature: {feature_name}")
            return False
        self._features[feature_name] = enabled
        print(f"{feature_name} {'enabled' if enabled else 'disabled'}")
        return True

    def apply_health(self):
        """Set player health to maximum if infinite health is enabled."""
        if not self._features.get("infinite_health", False):
            return
        addr = self.mr.get_pointer_chain(self.mr.base_address, self.HEALTH_OFFSETS)
        if addr:
            self.mr.write_float(addr, 150.0)  # Max health value

    def apply_stamina(self):
        """Set player stamina to maximum if infinite stamina is enabled."""
        if not self._features.get("infinite_stamina", False):
            return
        addr = self.mr.get_pointer_chain(self.mr.base_address, self.STAMINA_OFFSETS)
        if addr:
            self.mr.write_float(addr, 100.0)  # Max stamina value

    def apply_zenny(self):
        """Set zenny to 999999 if max zenny is enabled."""
        if not self._features.get("max_zenny", False):
            return
        addr = self.mr.get_pointer_chain(self.mr.base_address, self.ZENNY_OFFSETS)
        if addr:
            self.mr.write_int(addr, 999999)

    def apply_all(self):
        """Apply all enabled features in a single call (for hotkey loop)."""
        self.apply_health()
        self.apply_stamina()
        self.apply_zenny()
