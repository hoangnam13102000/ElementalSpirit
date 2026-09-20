using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Stage;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public sealed class PortalManager : IPortalManager
    {
        private const float PortalActivationRadius = 180f;
        private readonly List<Portal> _portals = new();

        public IReadOnlyList<Portal> Portals => _portals.AsReadOnly();

        public event Action<Portal>? OnPortalTriggered;

        public void RebuildPortalsForStage(
            int currentStageIndex,
            int totalStages,
            float playAreaWidth,
            float playAreaHeight,
            float groundY)
        {
            _portals.Clear();

            bool isFirstStage = currentStageIndex == 0;
            bool isLastStage = currentStageIndex >= totalStages - 1;

            if (!isFirstStage)
            {
                var backPortal = new Portal(
                    type: PortalType.BackPortal,
                    targetStageIndex: currentStageIndex - 1,
                    x: 10f,
                    y: groundY - 140f,
                    width: 70f,
                    height: 140f);
                _portals.Add(backPortal);
            }

            if (!isLastStage)
            {
                var forwardPortal = new Portal(
                    type: PortalType.ForwardPortal,
                    targetStageIndex: currentStageIndex + 1,
                    x: playAreaWidth - 80f,
                    y: groundY - 140f,
                    width: 70f,
                    height: 140f);
                forwardPortal.IsVisible = false; // Ẩn ban đầu, hiện khi hết quái
                _portals.Add(forwardPortal);
            }
        }

        public void UpdatePortalVisibility(bool allEnemiesCleared, RectangleF playerBounds, FacingDirection playerFacing)
        {
            foreach (var portal in _portals)
            {
                if (portal.Type == PortalType.BackPortal)
                {
                    portal.IsVisible = true;
                }
                else if (portal.Type == PortalType.ForwardPortal)
                {
                    portal.IsVisible = allEnemiesCleared;
                }
            }
        }

        public void CheckPlayerInteraction(RectangleF playerBounds)
        {
            foreach (var portal in _portals)
            {
                if (portal.Intersects(playerBounds))
                {
                    OnPortalTriggered?.Invoke(portal);
                    break; // Mỗi frame chỉ kích hoạt 1 cổng
                }
            }
        }

        public void HideAll()
        {
            foreach (var portal in _portals)
            {
                portal.IsVisible = false;
            }
        }

        public void RestoreVisibility(bool allEnemiesCleared)
        {
            UpdatePortalVisibility(allEnemiesCleared, RectangleF.Empty, FacingDirection.Right);
        }
    }
}