import sys
import time
from trainer.memory_reader import MemoryReader
from trainer.features import TrainerFeatures
from trainer.hotkey_manager import HotkeyManager


def main():
    """Main entry point for the Monster Hunter World: Iceborne Trainer."""
    print("Monster Hunter World: Iceborne Trainer v1.0.0")
    print("=" * 40)

    # Initialize components
    memory_reader = MemoryReader()
    if not memory_reader.attach():
        input("Press Enter to exit...")
        sys.exit(1)

    features = TrainerFeatures(memory_reader)
    hotkey_mgr = HotkeyManager()

    # Register hotkeys (key combos are examples; adjust as needed)
    hotkey_mgr.register_hotkey("ctrl+shift+h", lambda: features.toggle_feature("infinite_health", not features._features["infinite_health"]))
    hotkey_mgr.register_hotkey("ctrl+shift+s", lambda: features.toggle_feature("infinite_stamina", not features._features["infinite_stamina"]))
    hotkey_mgr.register_hotkey("ctrl+shift+z", lambda: features.toggle_feature("max_zenny", not features._features["max_zenny"]))

    hotkey_mgr.start_listener()

    print("Hotkeys:")
    print("  Ctrl+Shift+H - Toggle Infinite Health")
    print("  Ctrl+Shift+S - Toggle Infinite Stamina")
    print("  Ctrl+Shift+Z - Toggle Max Zenny")
    print("  Ctrl+C       - Exit")
    print()

    try:
        # Main loop: apply features continuously
        while True:
            features.apply_all()
            time.sleep(0.05)  # 20 Hz update rate
    except KeyboardInterrupt:
        print("\nShutting down...")
    finally:
        hotkey_mgr.stop_listener()
        memory_reader.detach()


if __name__ == "__main__":
    main()
