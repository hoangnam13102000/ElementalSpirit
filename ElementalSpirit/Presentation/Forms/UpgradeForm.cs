using System;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Spirit;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Services;

namespace ElementalSpirit.Presentation.Forms
{
    public class UpgradeForm : Form
    {
        private readonly SpiritManager _spirits;
        private readonly PlayerWallet _wallet;
        private readonly UpgradeService _upgrades;

        private readonly ListBox _spiritList;
        private readonly Label _detailLabel;
        private readonly Label _walletLabel;
        private readonly Label _messageLabel;
        private readonly Button _upgradeButton;
        private readonly Button _equip1Button;
        private readonly Button _equip2Button;
        private readonly Button _closeButton;

        public UpgradeForm(SpiritManager spirits, PlayerWallet wallet, UpgradeService upgrades)
        {
            _spirits = spirits;
            _wallet = wallet;
            _upgrades = upgrades;

            Text = "Spirit Upgrade - Elemental Spirit";
            ClientSize = new Size(520, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(24, 28, 40);
            ForeColor = Color.FromArgb(220, 230, 255);
            KeyPreview = true;

            var title = new Label
            {
                Text = "SPIRIT UPGRADE",
                Font = new Font("Consolas", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(160, 200, 255),
                Location = new Point(16, 12),
                AutoSize = true
            };

            _walletLabel = new Label
            {
                Font = new Font("Consolas", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 255, 160),
                Location = new Point(16, 42),
                AutoSize = true
            };

            _spiritList = new ListBox
            {
                Location = new Point(16, 70),
                Size = new Size(280, 260),
                Font = new Font("Consolas", 10f),
                BackColor = Color.FromArgb(18, 22, 32),
                ForeColor = Color.FromArgb(210, 220, 240),
                BorderStyle = BorderStyle.FixedSingle
            };
            _spiritList.SelectedIndexChanged += (_, _) => RefreshDetail();

            _detailLabel = new Label
            {
                Location = new Point(310, 70),
                Size = new Size(190, 150),
                Font = new Font("Consolas", 10f),
                ForeColor = Color.FromArgb(200, 210, 230)
            };

            _messageLabel = new Label
            {
                Location = new Point(16, 340),
                Size = new Size(480, 24),
                Font = new Font("Consolas", 10f),
                ForeColor = Color.FromArgb(180, 255, 200)
            };

            _upgradeButton = new Button
            {
                Text = "Upgrade",
                Location = new Point(310, 230),
                Size = new Size(190, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(70, 50, 110),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f, FontStyle.Bold)
            };
            _upgradeButton.Click += (_, _) => UpgradeSelected();

            _equip1Button = new Button
            {
                Text = "Equip Slot 1",
                Location = new Point(310, 270),
                Size = new Size(90, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 70, 110),
                ForeColor = Color.White,
                Font = new Font("Consolas", 9f)
            };
            _equip1Button.Click += (_, _) => EquipSelected(0);

            _equip2Button = new Button
            {
                Text = "Equip Slot 2",
                Location = new Point(410, 270),
                Size = new Size(90, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 70, 110),
                ForeColor = Color.White,
                Font = new Font("Consolas", 9f)
            };
            _equip2Button.Click += (_, _) => EquipSelected(1);

            _closeButton = new Button
            {
                Text = "Close (Esc)",
                Location = new Point(310, 308),
                Size = new Size(190, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 80),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f),
                DialogResult = DialogResult.OK
            };

            Controls.Add(title);
            Controls.Add(_walletLabel);
            Controls.Add(_spiritList);
            Controls.Add(_detailLabel);
            Controls.Add(_messageLabel);
            Controls.Add(_upgradeButton);
            Controls.Add(_equip1Button);
            Controls.Add(_equip2Button);
            Controls.Add(_closeButton);

            CancelButton = _closeButton;
            KeyDown += OnKeyDown;

            ReloadList();
            RefreshWallet();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ReloadList()
        {
            int selected = _spiritList.SelectedIndex;
            _spiritList.Items.Clear();

            foreach (var spirit in _spirits.Unlocked)
            {
                string eq = "";
                if (_spirits.Equipped[0]?.Id == spirit.Id) eq = " [Slot1]";
                if (_spirits.Equipped[1]?.Id == spirit.Id) eq = " [Slot2]";

                string cost = spirit.Level >= 5
                    ? "MAX"
                    : $"{_upgrades.GetUpgradeCost(spirit)} shards";

                _spiritList.Items.Add($"{spirit.Name}  Lv.{spirit.Level}  ({cost}){eq}");
            }

            if (_spiritList.Items.Count > 0)
                _spiritList.SelectedIndex = Math.Clamp(selected, 0, _spiritList.Items.Count - 1);
        }

        private ISpirit? GetSelectedSpirit()
        {
            int index = _spiritList.SelectedIndex;
            if (index < 0 || index >= _spirits.Unlocked.Count) return null;
            return _spirits.Unlocked[index];
        }

        private void RefreshDetail()
        {
            var spirit = GetSelectedSpirit();
            if (spirit == null) { _detailLabel.Text = ""; return; }

            string costText = spirit.Level >= 5
                ? "MAX LEVEL"
                : $"{_upgrades.GetUpgradeCost(spirit)} Spirit Shards";

            _detailLabel.Text =
                $"{spirit.Name}\n" +
                $"Element: {spirit.Element}\n" +
                $"Level: {spirit.Level}/5\n" +
                $"Cooldown: {spirit.CooldownDuration:0.#}s\n\n" +
                $"Next upgrade:\n{costText}";
        }

        private void RefreshWallet()
        {
            _walletLabel.Text =
                $"Gold: {_wallet.Gold}   Shards: {_wallet.SpiritShards}   Crystals: {_wallet.Crystals}";
        }

        private void UpgradeSelected()
        {
            var spirit = GetSelectedSpirit();
            if (spirit == null) return;

            if (spirit.Level >= 5)
            {
                _messageLabel.Text = $"{spirit.Name} is already max level.";
                _messageLabel.ForeColor = Color.FromArgb(255, 200, 120);
                return;
            }

            int cost = _upgrades.GetUpgradeCost(spirit);
            if (_upgrades.TryUpgrade(spirit, _wallet))
            {
                _messageLabel.Text = $"{spirit.Name} → Lv.{spirit.Level} (-{cost} shards)";
                _messageLabel.ForeColor = Color.FromArgb(180, 255, 200);
            }
            else
            {
                _messageLabel.Text = $"Need {cost} shards (have {_wallet.SpiritShards}).";
                _messageLabel.ForeColor = Color.FromArgb(255, 120, 120);
            }

            RefreshWallet();
            ReloadList();
            RefreshDetail();
        }

        private void EquipSelected(int slot)
        {
            var spirit = GetSelectedSpirit();
            if (spirit == null) return;

            if (_spirits.Equip(slot, spirit))
            {
                _messageLabel.Text = $"{spirit.Name} equipped to slot {slot + 1}";
                _messageLabel.ForeColor = Color.FromArgb(180, 255, 200);
                ReloadList();
            }
            else
            {
                _messageLabel.Text = "Cannot equip (already in other slot?).";
                _messageLabel.ForeColor = Color.FromArgb(255, 200, 120);
            }
        }
    }
}