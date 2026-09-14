using System;
using ElementalSpirit.Domain.Projectile;

namespace ElementalSpirit.Domain.Skill
{
    public sealed class SlashVariant
    {
        public string Id { get; }
        public string Name { get; }
        public ProjectileType ProjectileType { get; }
        public string IconAssetKey { get; }
        public string AlternateIconAssetKey { get; }
        public float IconScale { get; }

        public SlashVariant(
            string id,
            string name,
            ProjectileType projectileType,
            string iconAssetKey,
            string alternateIconAssetKey,
            float iconScale)
        {
            Id = string.IsNullOrWhiteSpace(id)
                ? throw new ArgumentException("Slash variant id is required.", nameof(id))
                : id;
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("Slash variant name is required.", nameof(name))
                : name;
            ProjectileType = projectileType;
            IconAssetKey = string.IsNullOrWhiteSpace(iconAssetKey)
                ? throw new ArgumentException("Slash variant icon is required.", nameof(iconAssetKey))
                : iconAssetKey;
            AlternateIconAssetKey = alternateIconAssetKey ?? string.Empty;
            IconScale = iconScale;
        }

        public static SlashVariant Wind()
        {
            return new SlashVariant(
                id: "slash.wind",
                name: "Slash",
                projectileType: ProjectileType.Slash,
                iconAssetKey: "Characters/Skill/Slash/WindSlash/Double Wind Slashes_Frame_01.png",
                alternateIconAssetKey: string.Empty,
                iconScale: 0.82f);
        }
    }
}
