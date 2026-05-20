import unittest
from unittest.mock import MagicMock, patch
from trainer.features import TrainerFeatures
from trainer.memory_reader import MemoryReader


class TestTrainerFeatures(unittest.TestCase):
    """Unit tests for TrainerFeatures class."""

    def setUp(self):
        """Create a mock MemoryReader and TrainerFeatures instance."""
        self.mock_mr = MagicMock(spec=MemoryReader)
        self.features = TrainerFeatures(self.mock_mr)

    def test_toggle_feature_unknown(self):
        """Toggling an unknown feature should return False."""
        result = self.features.toggle_feature("nonexistent_feature", True)
        self.assertFalse(result)

    def test_toggle_feature_valid(self):
        """Toggling a valid feature should return True and update state."""
        result = self.features.toggle_feature("infinite_health", True)
        self.assertTrue(result)
        self.assertTrue(self.features._features["infinite_health"])

    def test_apply_health_disabled(self):
        """apply_health should not write memory when feature is disabled."""
        self.features._features["infinite_health"] = False
        self.features.apply_health()
        self.mock_mr.get_pointer_chain.assert_not_called()
        self.mock_mr.write_float.assert_not_called()

    def test_apply_health_enabled(self):
        """apply_health should write max health when feature is enabled."""
        self.features._features["infinite_health"] = True
        self.mock_mr.get_pointer_chain.return_value = 0x12345678
        self.features.apply_health()
        self.mock_mr.get_pointer_chain.assert_called_once()
        self.mock_mr.write_float.assert_called_once_with(0x12345678, 150.0)

    def test_apply_health_pointer_failure(self):
        """apply_health should not write if pointer chain fails."""
        self.features._features["infinite_health"] = True
        self.mock_mr.get_pointer_chain.return_value = None
        self.features.apply_health()
        self.mock_mr.write_float.assert_not_called()

    def test_apply_zenny_enabled(self):
        """apply_zenny should write max zenny when feature is enabled."""
        self.features._features["max_zenny"] = True
        self.mock_mr.get_pointer_chain.return_value = 0x87654321
        self.features.apply_zenny()
        self.mock_mr.write_int.assert_called_once_with(0x87654321, 999999)

    def test_apply_all_calls_all(self):
        """apply_all should call all apply methods."""
        with patch.object(self.features, 'apply_health') as mock_health, \
             patch.object(self.features, 'apply_stamina') as mock_stamina, \
             patch.object(self.features, 'apply_zenny') as mock_zenny:
            self.features.apply_all()
            mock_health.assert_called_once()
            mock_stamina.assert_called_once()
            mock_zenny.assert_called_once()


if __name__ == '__main__':
    unittest.main()
