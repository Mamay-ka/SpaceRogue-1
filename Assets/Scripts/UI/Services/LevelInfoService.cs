using Gameplay.Services;
using System;
using UI.Game;
using UnityEngine;

namespace UI.Services
{
    public sealed class LevelInfoService : IDisposable
    {
        private readonly LevelNumberView _levelNumberView;
        private readonly EnemiesCountView _enemiesCountView;
        private readonly CurrentLevelProgress _currentLevelProgress;

        public LevelInfoService(LevelInfoView levelInfoView, CurrentLevelProgress currentLevelProgress)
        {
            _levelNumberView = levelInfoView.LevelNumberView;
            _enemiesCountView = levelInfoView.EnemiesCountView;
            _currentLevelProgress = currentLevelProgress;
            

            _levelNumberView.Hide();
            _enemiesCountView.Hide();
            _currentLevelProgress.LevelCreated += InitLevelInfoView;
            _currentLevelProgress.DefeatedEnemiesCountChange += UpdateEnemiesSpawnedCount;
        }

        public void Dispose()
        {
            _currentLevelProgress.LevelCreated -= InitLevelInfoView;
            _currentLevelProgress.DefeatedEnemiesCountChange -= UpdateEnemiesSpawnedCount;
        }

        private void InitLevelInfoView(Level level)
        {
            _levelNumberView.InitNumber(level.CurrentLevelNumber);
            _levelNumberView.Show();
            
            var preset = level.CurrentLevelPreset;
            SetEnemiesSpawnedCount(preset.EnemiesCountToWin, level.EnemiesCreatedCount);
        }

        private void SetEnemiesSpawnedCount(int enemiesCountToWin, int enemiesCount)
        {
            var countToWin = Mathf.Clamp(enemiesCountToWin, 1, enemiesCount);
            _enemiesCountView.Init(0, countToWin);
            _enemiesCountView.Show();
        }

        private void UpdateEnemiesSpawnedCount(int defeatedEnemiesCount)
        {
            _enemiesCountView.UpdateCounter(defeatedEnemiesCount);
        }
    }
}