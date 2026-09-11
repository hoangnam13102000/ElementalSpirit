using System;
using System.Drawing;
using System.Windows.Forms;
using ElementalSpirit.Data;
using ElementalSpirit.Domain.Currency;
using ElementalSpirit.Domain.Equipment;
using ElementalSpirit.Domain.Inventory;
using ElementalSpirit.Services;

namespace ElementalSpirit.Presentation.Forms
{
    public class ShopForm : Form
    {
        private readonly PlayerWallet _wallet;
        private readonly Inventory _inventory;
        private readonly ShopService _shop;
        private readonly Action? _onInventoryChanged;

        private readonly ListBox _itemList;
        private readonly Label _detailLabel;
        private readonly Label _walletLabel;
        private readonly Label _messageLabel;
        private readonly Button _buyButton;
        private readonly Button _equipButton;
        private readonly Button _closeButton;

        public ShopForm(
            PlayerWallet wallet,
            Inventory inventory,
            ShopService shop,
            Action? onInventoryChanged = null)
        {
            _wallet = wallet;
            _inventory = inventory;
            _shop = shop;
            _onInventoryChanged = onInventoryChanged;

            Text = "Shop - Elemental Spirit";
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
                Text = "EQUIPMENT SHOP",
                Font = new Font("Consolas", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 220, 140),
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

            _itemList = new ListBox
            {
                Location = new Point(16, 70),
                Size = new Size(280, 260),
                Font = new Font("Consolas", 10f),
                BackColor = Color.FromArgb(18, 22, 32),
                ForeColor = Color.FromArgb(210, 220, 240),
                BorderStyle = BorderStyle.FixedSingle
            };
            _itemList.SelectedIndexChanged += (_, _) => RefreshDetail();

            _detailLabel = new Label
            {
                Location = new Point(310, 70),
                Size = new Size(190, 160),
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

            _buyButton = new Button
            {
                Text = "Buy",
                Location = new Point(310, 240),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 90, 60),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f, FontStyle.Bold)
            };
            _buyButton.Click += (_, _) => BuySelected();

            _equipButton = new Button
            {
                Text = "Equip",
                Location = new Point(410, 240),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 70, 110),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f, FontStyle.Bold)
            };
            _equipButton.Click += (_, _) => EquipSelected();

            _closeButton = new Button
            {
                Text = "Close (Esc)",
                Location = new Point(310, 290),
                Size = new Size(190, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 80),
                ForeColor = Color.White,
                Font = new Font("Consolas", 10f),
                DialogResult = DialogResult.OK
            };

            Controls.Add(title);
            Controls.Add(_walletLabel);
            Controls.Add(_itemList);
            Controls.Add(_detailLabel);
            Controls.Add(_messageLabel);
            Controls.Add(_buyButton);
            Controls.Add(_equipButton);
            Controls.Add(_closeButton);

            AcceptButton = _buyButton;
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
            _itemList.Items.Clear();
            foreach (var item in EquipmentCatalog.All)
            {
                string owned = _inventory.Owns(item.Id) ? "[OWNED]" : $"{item.PriceGold}G";
                string equipped = "";
                var eq = _inventory.GetEquipped(item.Slot);
                if (eq != null && eq.Id == item.Id) equipped = " *EQ*";
                _itemList.Items.Add($"{item.Name}  ({owned}){equipped}");
            }
            if (_itemList.Items.Count > 0)
                _itemList.SelectedIndex = 0;
        }

        private Equipment? GetSelectedItem()
        {
            int index = _itemList.SelectedIndex;
            if (index < 0 || index >= EquipmentCatalog.All.Count) return null;
            return EquipmentCatalog.All[index];
        }

        private void RefreshDetail()
        {
            var item = GetSelectedItem();
            if (item == null) { _detailLabel.Text = ""; return; }

            string owned = _inventory.Owns(item.Id) ? "Yes" : "No";
            var equipped = _inventory.GetEquipped(item.Slot);
            string isEq = equipped != null && equipped.Id == item.Id ? "Yes" : "No";

            _detailLabel.Text =
                $"{item.Name}\n" +
                $"Slot: {item.Slot}\n" +
                $"Price: {item.PriceGold} Gold\n" +
                $"+DMG: {item.BonusDamage}\n" +
                $"+HP: {item.BonusMaxHp}\n" +
                $"+DEF: {item.BonusDefense}\n\n" +
                $"Owned: {owned}\n" +
                $"Equipped: {isEq}";
        }

        private void RefreshWallet()
        {
            _walletLabel.Text =
                $"Gold: {_wallet.Gold}   Shards: {_wallet.SpiritShards}   Crystals: {_wallet.Crystals}";
        }

        private void BuySelected()
        {
            var item = GetSelectedItem();
            if (item == null) return;

            var result = _shop.TryBuy(item.Id, _wallet, _inventory);
            switch (result)
            {
                case PurchaseResult.Success:
                    _messageLabel.Text = $"Bought {item.Name} (-{item.PriceGold} Gold)";
                    _messageLabel.ForeColor = Color.FromArgb(180, 255, 200);
                    break;
                case PurchaseResult.AlreadyOwned:
                    _messageLabel.Text = "Already owned.";
                    _messageLabel.ForeColor = Color.FromArgb(255, 200, 120);
                    break;
                case PurchaseResult.NotEnoughGold:
                    _messageLabel.Text = $"Need {item.PriceGold} Gold (have {_wallet.Gold}).";
                    _messageLabel.ForeColor = Color.FromArgb(255, 120, 120);
                    break;
                default:
                    _messageLabel.Text = "Cannot buy.";
                    _messageLabel.ForeColor = Color.FromArgb(255, 120, 120);
                    break;
            }

            RefreshWallet();
            ReloadList();
            RefreshDetail();
            _onInventoryChanged?.Invoke();
        }

        private void EquipSelected()
        {
            var item = GetSelectedItem();
            if (item == null) return;

            if (!_inventory.Owns(item.Id))
            {
                _messageLabel.Text = "Buy it first.";
                _messageLabel.ForeColor = Color.FromArgb(255, 200, 120);
                return;
            }

            if (_inventory.TryEquip(item.Id))
            {
                _messageLabel.Text = $"Equipped {item.Name}";
                _messageLabel.ForeColor = Color.FromArgb(180, 255, 200);
                ReloadList();
                RefreshDetail();
                _onInventoryChanged?.Invoke();
            }
        }
    }
}