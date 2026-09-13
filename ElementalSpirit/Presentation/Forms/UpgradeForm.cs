using System;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Spirit;
using ElementalSpirit.GameEngine;
using ElementalSpirit.Localization;
using ElementalSpirit.Services;
using ElementalSpirit.Services.Abstractions;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.Presentation.Forms
{
    public class UpgradeForm : Form
    {
        private readonly ISpiritManager _spirits;
        private readonly PlayerWallet _wallet;
        private readonly IUpgradeService _upgrades;
        private readonly ILocalizationService _localization;

        private readonly ListBox _spiritList;
        private readonly Label _detailLabel;
        private readonly Label _walletLabel;
        private readonly Label _messageLabel;
        private readonly Button _upgradeButton;
        private readonly Button _equip1Button;
        private readonly Button _equip2Button;
        private readonly Button _closeButton;

        public UpgradeForm(ISpiritManager spirits, PlayerWallet wallet, IUpgradeService upgrades, ILocalizationService localization)
        {
            _spirits = spirits;
            _wallet = wallet;
            _upgrades = upgrades;
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            Text = _localization.Translate("upgrade.title");
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
                Text = _localization.Translate("upgrade.heading"),
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
                Text = _localization.Translate("upgrade.button.upgrade"),
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
                Text = _localization.Translate("upgrade.button.equip1"),
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
                Text = _localization.Translate("upgrade.button.equip2"),
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
                Text = _localization.Translate("upgrade.button.close"),
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
                if (_spirits.Equipped[0]?.Id == spirit.Id) eq = _localization.Translate("upgrade.list.slot1");
                if (_spirits.Equipped[1]?.Id == spirit.Id) eq = _localization.Translate("upgrade.list.slot2");

                string cost = spirit.Level >= 5
                    ? _localization.Translate("upgrade.list.max")
                    : $"{_upgrades.GetUpgradeCost(spirit)}{_localization.Translate("upgrade.list.shardsSuffix")}";

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
                ? _localization.Translate("upgrade.detail.maxLevel")
                : $"{_upgrades.GetUpgradeCost(spirit)}{_localization.Translate("upgrade.detail.shardsSuffix")}";

            _detailLabel.Text =
                $"{spirit.Name}\n" +
                $"{_localization.Translate("upgrade.detail.element")} {spirit.Element}\n" +
                $"{_localization.Translate("upgrade.detail.level")} {spirit.Level}/5\n" +
                $"{_localization.Translate("upgrade.detail.cooldown")} {spirit.CooldownDuration:0.#}s\n\n" +
                $"{_localization.Translate("upgrade.detail.nextUpgrade")}\n{costText}";
        }

        private void RefreshWallet()
        {
            _walletLabel.Text = string.Format(
                _localization.Translate("upgrade.wallet"),
                _wallet.Gold, _wallet.SpiritShards, _wallet.Crystals);
        }

        private void UpgradeSelected()
        {
            var spirit = GetSelectedSpirit();
            if (spirit == null) return;

            if (spirit.Level >= 5)
            {
                _messageLabel.Text = string.Format(_localization.Translate("upgrade.msg.alreadyMax"), spirit.Name);
                _messageLabel.ForeColor = Color.FromArgb(255, 200, 120);
                return;
            }

            int cost = _upgrades.GetUpgradeCost(spirit);
            if (_upgrades.TryUpgrade(spirit, _wallet))
            {
                _messageLabel.Text = string.Format(
                    _localization.Translate("upgrade.msg.upgraded"), spirit.Name, spirit.Level, cost);
                _messageLabel.ForeColor = Color.FromArgb(180, 255, 200);
            }
            else
            {
                _messageLabel.Text = string.Format(
                    _localization.Translate("upgrade.msg.needShards"), cost, _wallet.SpiritShards);
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
                _messageLabel.Text = string.Format(
                    _localization.Translate("upgrade.msg.equipped"), spirit.Name, slot + 1);
                _messageLabel.ForeColor = Color.FromArgb(180, 255, 200);
                ReloadList();
            }
            else
            {
                _messageLabel.Text = _localization.Translate("upgrade.msg.cannotEquip");
                _messageLabel.ForeColor = Color.FromArgb(255, 200, 120);
            }
        }
    }
}