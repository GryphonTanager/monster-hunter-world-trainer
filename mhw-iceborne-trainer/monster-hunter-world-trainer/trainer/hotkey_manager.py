import threading
import time
from typing import Dict, Callable

# Try importing keyboard; fallback if not available
_has_keyboard = False
try:
    import keyboard
    _has_keyboard = True
except ImportError:
    pass


class HotkeyManager:
    """Manages hotkey bindings for toggling trainer features."""

    def __init__(self):
        self._bindings: Dict[str, Callable] = {}
        self._listener_thread: threading.Thread = None
        self._running = False

    def register_hotkey(self, key_combo: str, callback: Callable):
        """Register a hotkey combination with a callback.
        
        Args:
            key_combo: e.g., 'ctrl+shift+h' for infinite health.
            callback: Function to call when hotkey is pressed.
        """
        self._bindings[key_combo] = callback
        if _has_keyboard:
            keyboard.add_hotkey(key_combo, callback)
        print(f"Registered hotkey: {key_combo}")

    def start_listener(self):
        """Start a background thread to listen for hotkeys (fallback mode)."""
        if _has_keyboard:
            print("Using keyboard library - no fallback needed.")
            return
        if self._running:
            return
        self._running = True
        self._listener_thread = threading.Thread(target=self._listen_loop, daemon=True)
        self._listener_thread.start()

    def stop_listener(self):
        """Stop the hotkey listener."""
        self._running = False
        if _has_keyboard:
            for key in self._bindings:
                keyboard.remove_hotkey(key)

    def _listen_loop(self):
        """Fallback polling loop for hotkeys when keyboard library is unavailable."""
        # Simple polling implementation (not recommended for production)
        # This is a placeholder; real implementation would use platform-specific hooks
        while self._running:
            time.sleep(0.1)
            # In a real trainer, we'd poll keyboard state here
