using System;
using System.Collections.Generic;
using ElementalSpirit.Domain.Player;
using ElementalSpirit.Domain.Stage;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface IPortalManager
    {
        /// <summary>Danh sách cổng hiện tại của stage</summary>
        IReadOnlyList<Portal> Portals { get; }

        event Action<Portal>? OnPortalTriggered;

        void RebuildPortalsForStage(
            int currentStageIndex,
            int totalStages,
            float playAreaWidth,
            float playAreaHeight,
            float groundY);

        void UpdatePortalVisibility(bool allEnemiesCleared, RectangleF playerBounds, FacingDirection playerFacing);

        void CheckPlayerInteraction(RectangleF playerBounds);

        void HideAll();

        void RestoreVisibility(bool allEnemiesCleared);
    }
}