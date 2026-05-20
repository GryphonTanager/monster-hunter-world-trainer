import pymem
import pymem.process
from typing import Optional, Tuple


class MemoryReader:
    """Handles reading and writing to Monster Hunter World: Iceborne process memory."""

    PROCESS_NAME = "MonsterHunterWorld.exe"

    def __init__(self):
        self.pm: Optional[pymem.Pymem] = None
        self.base_address: Optional[int] = None

    def attach(self) -> bool:
        """Attach to the Monster Hunter World process."""
        try:
            self.pm = pymem.Pymem(self.PROCESS_NAME)
            self.base_address = pymem.process.module_from_name(
                self.pm.process_handle, self.PROCESS_NAME
            ).lpBaseOfDll
            return True
        except pymem.exception.ProcessNotFound:
            print("MonsterHunterWorld.exe not found. Is the game running?")
            return False
        except Exception as e:
            print(f"Failed to attach: {e}")
            return False

    def detach(self):
        """Detach from the process."""
        if self.pm:
            self.pm.close_process()
            self.pm = None
            self.base_address = None

    def read_int(self, address: int) -> int:
        """Read a 4-byte integer from the given address."""
        return self.pm.read_int(address)

    def write_int(self, address: int, value: int):
        """Write a 4-byte integer to the given address."""
        self.pm.write_int(address, value)

    def read_float(self, address: int) -> float:
        """Read a 4-byte float from the given address."""
        return self.pm.read_float(address)

    def write_float(self, address: int, value: float):
        """Write a 4-byte float to the given address."""
        self.pm.write_float(address, value)

    def get_pointer_chain(self, base: int, offsets: Tuple[int, ...]) -> Optional[int]:
        """Resolve a multi-level pointer chain to the final address.
        
        Args:
            base: Base address to start from.
            offsets: Tuple of offsets to follow sequentially.
        
        Returns:
            The final address after following all offsets, or None on failure.
        """
        try:
            addr = self.pm.read_int(base)
            for offset in offsets[:-1]:
                addr = self.pm.read_int(addr + offset)
            return addr + offsets[-1]
        except Exception:
            return None
