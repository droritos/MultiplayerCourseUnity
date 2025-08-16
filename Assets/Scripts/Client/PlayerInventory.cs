using UnityEngine;
using UnityEngine.Events;

namespace Game.Client
{
    public class PlayerInventory
    {
        public event UnityAction OnBombCountChanged;
        public event UnityAction OnBombUseFailed;

        public int MaxPossessedBombCount { get; private set; } = 1; // Default to 1 bomb

        private int _currentBombCount; // _currentBombCount/MaxPossessedBombCount - GUI

        public int CurrentBombCount
        {
            get => _currentBombCount;
            set
            {
                int newValue = Mathf.Clamp(value, 0, MaxPossessedBombCount);
                if (_currentBombCount != newValue)
                {
                    _currentBombCount = newValue;
                    OnBombCountChanged?.Invoke();
                }
            }
        }

        public PlayerInventory()
        {
            UpdateInventory();
        }

        public void UpdateInventory()
        {
            CurrentBombCount = MaxPossessedBombCount; // Initialize with max bomb count
        }

        public void AddBombCount()
        {
            MaxPossessedBombCount++;
            UpdateInventory();
        }

        public bool TryUseBomb()
        {
            if (CurrentBombCount > 0)
            {
                CurrentBombCount--;
                return true; // Bomb used successfully
            }
            else
            {
                OnBombUseFailed?.Invoke(); // Trigger UI or sound feedback
                return false; // No bombs available to use
            }
        }
    }
}
