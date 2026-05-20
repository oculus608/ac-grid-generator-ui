using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace AcGridGeneratorUi
{
    public partial class Form1 : Form
    {
        private GridConfig _config;
        private Car[] _allAvailableCars;
        private BindingList<CarGridDisplayItem> _selectedCarsBinding;
        private bool _isLoaded = false;
        private string _currentStrategySelection = "Fixed";

        // Unified Pro Slate Dark Theme Palette Rules
        private readonly Color DarkBg = Color.FromArgb(24, 24, 24);
        private readonly Color PanelBg = Color.FromArgb(36, 36, 36);
        private readonly Color ControlInputBg = Color.FromArgb(48, 48, 48);
        private readonly Color TextLight = Color.FromArgb(245, 245, 245);
        private readonly Color TextDim = Color.FromArgb(160, 160, 160);
        private readonly Color AccentBlue = Color.FromArgb(52, 152, 219);
        private readonly Color HighlightBg = Color.FromArgb(41, 128, 185);

        private bool _isHeaderButtonHovered = false;
        private bool _isHeaderButtonPressed = false;

        public Form1()
        {
            InitializeComponent();
            ApplyPremiumDarkTheme();
            this.Load += Form1_Load;
            this.Resize += Form1_Resize;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCarInventory();
                LoadOrInitializeConfiguration();
                InitializeEventHandlers();
                SyncOverlayLayoutBounds();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Data Layer Initialization Failed:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SyncOverlayLayoutBounds();
        }

        private void SyncOverlayLayoutBounds()
        {
            // Seamlessly locks the dropdown flush against the text box with zero gaps
            if (txtSmartSearch != null && lstSmartSuggestions != null)
            {
                lstSmartSuggestions.Left = txtSmartSearch.Left;
                lstSmartSuggestions.Top = txtSmartSearch.Bottom;
                lstSmartSuggestions.Width = txtSmartSearch.Width;
                lstSmartSuggestions.BringToFront();
            }
        }


        private void ApplyPremiumDarkTheme()
        {
            this.BackColor = DarkBg;
            this.ForeColor = TextLight;

            pnlLeft.BackColor = PanelBg;
            pnlRight.BackColor = DarkBg;

            Font headerFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            Font standardFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            lblPresetName.Font = headerFont; lblPresetName.ForeColor = AccentBlue;
            lblStrategy.Font = headerFont; lblStrategy.ForeColor = AccentBlue;
            lblSkill.Font = headerFont; lblSkill.ForeColor = AccentBlue;
            lblAggression.Font = headerFont; lblAggression.ForeColor = AccentBlue;
            lblSelectCarHeader.Font = headerFont; lblSelectCarHeader.ForeColor = AccentBlue;

            lblStrategyDesc.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblStrategyDesc.ForeColor = TextDim;
            lblSkillVal.Font = standardFont; lblSkillVal.ForeColor = TextLight;
            lblAggressionVal.Font = standardFont; lblAggressionVal.ForeColor = TextLight;

            txtPresetName.BackColor = ControlInputBg; txtPresetName.ForeColor = TextLight; txtPresetName.BorderStyle = BorderStyle.FixedSingle;
            txtSmartSearch.BackColor = ControlInputBg; txtSmartSearch.ForeColor = TextLight; txtSmartSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSmartSearch.Font = standardFont;

            txtStrategyHeader.BackColor = ControlInputBg; txtStrategyHeader.ForeColor = TextLight; txtStrategyHeader.BorderStyle = BorderStyle.FixedSingle;
            txtStrategyHeader.Font = standardFont;

            numSkill.BackColor = PanelBg;
            numAggression.BackColor = PanelBg;

            StyleSuggestionsListBox(lstSmartSuggestions, standardFont);
            StyleSuggestionsListBox(lstStrategySuggestions, standardFont);

            dgvSelectedCars.BackgroundColor = ControlInputBg;
            dgvSelectedCars.ForeColor = TextLight;
            dgvSelectedCars.BorderStyle = BorderStyle.None;
            dgvSelectedCars.GridColor = Color.FromArgb(64, 64, 64);
            dgvSelectedCars.EnableHeadersVisualStyles = false;
            dgvSelectedCars.RowHeadersVisible = false;
            dgvSelectedCars.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 56, 56);

            dgvSelectedCars.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(56, 56, 56);
            dgvSelectedCars.ColumnHeadersDefaultCellStyle.ForeColor = TextLight;
            dgvSelectedCars.ColumnHeadersDefaultCellStyle.Font = headerFont;
            dgvSelectedCars.ColumnHeadersHeight = 35;

            dgvSelectedCars.DefaultCellStyle.BackColor = ControlInputBg;
            dgvSelectedCars.DefaultCellStyle.ForeColor = TextLight;
            dgvSelectedCars.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 70);
            dgvSelectedCars.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvSelectedCars.DefaultCellStyle.Font = standardFont;
            dgvSelectedCars.RowTemplate.Height = 32;

            // Locate the end of your ApplyPremiumDarkTheme() method and add these lines:
            numSkill.BackColor = PanelBg;
            numAggression.BackColor = PanelBg;
        }

        private void StyleSuggestionsListBox(ListBox lb, Font standardFont)
        {
            lb.BackColor = ControlInputBg;
            lb.ForeColor = TextLight;
            lb.BorderStyle = BorderStyle.FixedSingle;
            lb.Font = standardFont;
            lb.DrawMode = DrawMode.OwnerDrawFixed;
            lb.ItemHeight = 26;
            lb.DrawItem += LstSmartSuggestions_DrawItem;
        }

        private void LstSmartSuggestions_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            ListBox lb = (ListBox)sender;
            Color bg = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? HighlightBg : lb.BackColor;

            using (SolidBrush brushBg = new SolidBrush(bg))
            using (SolidBrush brushText = new SolidBrush(lb.ForeColor))
            {
                e.Graphics.FillRectangle(brushBg, e.Bounds);
                string text = lb.Items[e.Index].ToString();
                int textY = e.Bounds.Y + (e.Bounds.Height - TextRenderer.MeasureText(text, lb.Font).Height) / 2;
                e.Graphics.DrawString(text, lb.Font, brushText, e.Bounds.X + 6, textY);
            }
            e.DrawFocusRectangle();
        }

        private void LoadCarInventory()
        {
            _allAvailableCars = CarProvider.GetInstalledCars() ?? Array.Empty<Car>();
        }

        private void LoadOrInitializeConfiguration()
        {
            string path = AppSettings.ParametersJson;
            if (File.Exists(path))
            {
                try { _config = JsonSerializer.Deserialize<GridConfig>(File.ReadAllText(path)) ?? CreateDefaultConfig(); }
                catch { _config = CreateDefaultConfig(); }
            }
            else
            {
                _config = CreateDefaultConfig();
                WriteConfigurationToDisk();
            }

            _config.AcRootPath = AppSettings.AssettoCorsaRoot;
            _config.PresetFolder = AppSettings.Presets;
            PopulateUiFromConfig();
        }

        private GridConfig CreateDefaultConfig() => new() { GridStrategy = "Fixed", BaseSkill = 85, BaseAggression = 50, PresetName = string.Empty, CarAllocations = new List<CarAllocation>() };

        private void PopulateUiFromConfig()
        {
            _isLoaded = false;
            txtPresetName.Text = _config.PresetName;

            _currentStrategySelection = (new[] { "Fixed", "Franchise", "Lottery" }).Contains(_config.GridStrategy) ? _config.GridStrategy : "Fixed";
            txtStrategyHeader.Text = $" {_currentStrategySelection}";
            UpdateStrategyLabelDescription();

            numSkill.Value = Math.Clamp(_config.BaseSkill, 70, 100);
            lblSkillVal.Text = $"{numSkill.Value}%";

            numAggression.Value = Math.Clamp(_config.BaseAggression, 0, 100);
            lblAggressionVal.Text = $"{numAggression.Value}%";

            var displayItems = _config.CarAllocations.Select(alloc => {
                string name = _allAvailableCars.FirstOrDefault(c => c.Id == alloc.CarId)?.Name ?? alloc.CarId;
                return new CarGridDisplayItem(alloc, name);
            }).ToList();

            _selectedCarsBinding = new BindingList<CarGridDisplayItem>(displayItems);
            dgvSelectedCars.DataSource = _selectedCarsBinding;
            ConfigureDataGridColumns();
            _isLoaded = true;
        }

        private void ConfigureDataGridColumns()
        {
            if (dgvSelectedCars.Columns.Count > 0)
            {
                dgvSelectedCars.Columns["CarId"].Visible = false;
                dgvSelectedCars.Columns["CarName"].HeaderText = "Car Model Name";
                dgvSelectedCars.Columns["CarName"].ReadOnly = true;
                dgvSelectedCars.Columns["CarName"].FillWeight = 240;

                dgvSelectedCars.Columns["Count"].HeaderText = "Count";
                dgvSelectedCars.Columns["Count"].FillWeight = 70;

                dgvSelectedCars.Columns["Ballast"].HeaderText = "Ballast (kg)";
                dgvSelectedCars.Columns["Ballast"].FillWeight = 85;

                dgvSelectedCars.Columns["Restrictor"].HeaderText = "Restrictor (%)";
                dgvSelectedCars.Columns["Restrictor"].FillWeight = 95;

                if (!dgvSelectedCars.Columns.Contains("RemoveAction"))
                {
                    DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
                    btnCol.Name = "RemoveAction";
                    btnCol.HeaderText = "";
                    btnCol.Text = "Remove";
                    btnCol.UseColumnTextForButtonValue = true;
                    btnCol.FlatStyle = FlatStyle.Flat;
                    btnCol.DefaultCellStyle.BackColor = ControlInputBg; // Matches cell text background
                    btnCol.DefaultCellStyle.ForeColor = TextLight;      // Matches main grid text
                    dgvSelectedCars.Columns.Add(btnCol);
                    dgvSelectedCars.Columns["RemoveAction"].FillWeight = 80;
                }
            }
        }

        private void InitializeEventHandlers()
        {
            txtPresetName.TextChanged += (s, e) => TriggerChangeDebounce();
            numSkill.Scroll += (s, e) => { lblSkillVal.Text = $"{numSkill.Value}%"; TriggerChangeDebounce(); };
            numAggression.Scroll += (s, e) => { lblAggressionVal.Text = $"{numAggression.Value}%"; TriggerChangeDebounce(); };

            // --- STRATEGY CUSTOM OVERLAY CONTROL HANDLING ---
            txtStrategyHeader.Click += (s, e) => ToggleStrategyDropdown();
            txtStrategyHeader.KeyDown += TxtStrategyHeader_KeyDown;

            txtStrategyHeader.Enter += (s, e) => { txtStrategyHeader.SelectionLength = 0; };
            txtStrategyHeader.GotFocus += (s, e) => { txtStrategyHeader.SelectionLength = 0; };
            txtStrategyHeader.MouseUp += (s, e) => { txtStrategyHeader.SelectionLength = 0; };

            lstStrategySuggestions.Items.Clear();
            lstStrategySuggestions.Items.AddRange(new object[] { "Fixed", "Franchise", "Lottery" });
            lstStrategySuggestions.KeyDown += LstStrategySuggestions_KeyDown;
            lstStrategySuggestions.Click += (s, e) => CommitStrategySelection();

            // --- CAR SMART SUGGESTIONS CONTROL HANDLING ---
            txtSmartSearch.TextChanged += TxtSmartSearch_TextChanged;
            txtSmartSearch.KeyDown += TxtSmartSearch_KeyDown;
            txtSmartSearch.Enter += (s, e) => { this.BeginInvoke(new Action(() => txtSmartSearch.SelectAll())); };

            lstSmartSuggestions.KeyDown += LstSmartSuggestions_KeyDown;
            lstSmartSuggestions.DoubleClick += (s, e) => CommitSmartSelection();

            this.Click += (s, e) => HideAllPopups();
            pnlLeft.Click += (s, e) => HideAllPopups();
            dgvSelectedCars.CellClick += (s, e) => HideAllPopups();

            dgvSelectedCars.CellValueChanged += (s, e) => TriggerChangeDebounce();
            dgvSelectedCars.CellContentClick += dgvSelectedCars_CellContentClick;

            tmrSaveDebounce.Interval = 350;
            tmrSaveDebounce.Tick += (s, e) => { tmrSaveDebounce.Stop(); CommitUiToModelAndSave(); };

            // 1. Manually paint a clean custom button in the header cell
            dgvSelectedCars.CellPainting += (s, e) => {
                // Only intercept the header row (index -1)
                if (e.RowIndex == -1 && e.ColumnIndex >= 0)
                {
                    e.PaintBackground(e.CellBounds, true);

                    // Default base color block for standard label headers
                    Color headerBgColor = Color.FromArgb(56, 56, 56);

                    // Check if this specific cell is our interactive "Clear Grid" button column
                    bool isClearGridButton = (e.ColumnIndex == dgvSelectedCars.Columns["RemoveAction"].Index);

                    if (isClearGridButton)
                    {
                        // Apply dynamic hover states for the button partition
                        if (_isHeaderButtonPressed)
                        {
                            headerBgColor = Color.FromArgb(40, 40, 40); // Dark active press feedback
                        }
                        else if (_isHeaderButtonHovered)
                        {
                            headerBgColor = Color.FromArgb(85, 85, 85); // Lighter mouse hover feedback
                        }
                        else
                        {
                            headerBgColor = Color.FromArgb(64, 64, 64); // Button idle state (slightly lighter than regular headers)
                        }
                    }

                    // Draw a seamless flat rectangle over the entire cell bounding block
                    // Note: For buttons we give it a tiny inset border; for text we fill it completely flush
                    Rectangle fillRect = isClearGridButton
                        ? new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4, e.CellBounds.Width - 8, e.CellBounds.Height - 8)
                        : e.CellBounds;

                    using (SolidBrush brush = new SolidBrush(headerBgColor))
                    {
                        e.Graphics.FillRectangle(brush, fillRect);
                    }

                    // Determine correct text alignment layout rules
                    TextFormatFlags textFlags = TextFormatFlags.VerticalCenter;

                    // Determine correct text alignment layout rules
                    Rectangle textRect = fillRect;

                    // Center the button text, but left-align data headers with a padding margin block
                    if (isClearGridButton)
                    {
                        textFlags |= TextFormatFlags.HorizontalCenter;
                    }
                    else
                    {
                        textFlags |= TextFormatFlags.Left;
                        // Inject an elegant 8-pixel horizontal indentation so text doesn't stick hard against cell edges
                        textRect = new Rectangle(fillRect.X + 8, fillRect.Y, fillRect.Width - 8, fillRect.Height);
                    }

                    // Draw the text string crisp and clear on top of our canvas color block
                    string headerText = isClearGridButton
                        ? "Clear Grid"
                        : dgvSelectedCars.Columns[e.ColumnIndex].HeaderText;

                    TextRenderer.DrawText(e.Graphics, headerText, dgvSelectedCars.Font, textRect, TextLight, textFlags);

                    e.Handled = true;
                }
            };

            // 2. Capture when the user clicks the custom button area in the header
            dgvSelectedCars.CellClick += (s, e) => {
                if (e.RowIndex == -1 && e.ColumnIndex == dgvSelectedCars.Columns["RemoveAction"].Index)
                {
                    // Prompt a clean non-intrusive safety check confirmation layout popup
                    var result = MessageBox.Show("Are you sure you want to clear all cars from the grid?",
                        "Clear Grid Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _selectedCarsBinding.Clear(); // Wipe selection queue collection out instantly
                        TriggerChangeDebounce();       // Auto-save empty roster directly out to parameters.json
                    }
                }
            };

            // Track when the mouse cursor enters, leaves, presses, or releases the Clear Grid header
            dgvSelectedCars.CellMouseEnter += (s, e) => {
                if (e.RowIndex == -1 && e.ColumnIndex == dgvSelectedCars.Columns["RemoveAction"].Index)
                {
                    _isHeaderButtonHovered = true;
                    dgvSelectedCars.InvalidateCell(e.ColumnIndex, e.RowIndex); // Force repaint
                }
            };

            dgvSelectedCars.CellMouseLeave += (s, e) => {
                if (e.RowIndex == -1 && e.ColumnIndex == dgvSelectedCars.Columns["RemoveAction"].Index)
                {
                    _isHeaderButtonHovered = false;
                    _isHeaderButtonPressed = false;
                    dgvSelectedCars.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            };

            dgvSelectedCars.CellMouseDown += (s, e) => {
                if (e.RowIndex == -1 && e.ColumnIndex == dgvSelectedCars.Columns["RemoveAction"].Index && e.Button == MouseButtons.Left)
                {
                    _isHeaderButtonPressed = true;
                    dgvSelectedCars.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            };

            dgvSelectedCars.CellMouseUp += (s, e) => {
                if (e.RowIndex == -1 && e.ColumnIndex == dgvSelectedCars.Columns["RemoveAction"].Index && e.Button == MouseButtons.Left)
                {
                    _isHeaderButtonPressed = false;
                    dgvSelectedCars.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            };
        }

        private void ToggleStrategyDropdown()
        {
            if (lstStrategySuggestions.Visible)
            {
                lstStrategySuggestions.Visible = false;
            }
            else
            {
                HideAllPopups();
                lstStrategySuggestions.SelectedItem = _currentStrategySelection;
                lstStrategySuggestions.Visible = true;
                lstStrategySuggestions.Focus();
            }
            txtStrategyHeader.SelectionLength = 0;
        }

        private void TxtStrategyHeader_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                ToggleStrategyDropdown();
                e.Handled = true;
            }
        }

        private void LstStrategySuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitStrategySelection();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                lstStrategySuggestions.Visible = false;
                txtStrategyHeader.Focus();
                e.Handled = true;
            }
        }

        private void CommitStrategySelection()
        {
            if (lstStrategySuggestions.SelectedItem != null)
            {
                _currentStrategySelection = lstStrategySuggestions.SelectedItem.ToString();
                txtStrategyHeader.Text = $" {_currentStrategySelection}";
                lstStrategySuggestions.Visible = false;
                UpdateStrategyLabelDescription();
                txtStrategyHeader.Focus();
                txtStrategyHeader.SelectionLength = 0;
                TriggerChangeDebounce();
            }
        }

        private void TxtSmartSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSmartSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword) || txtSmartSearch.SelectionLength == txtSmartSearch.TextLength)
            {
                lstSmartSuggestions.Visible = false;
                return;
            }

            var matches = _allAvailableCars
                .Where(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Name)
                .Take(12)
                .ToList();

            if (matches.Any())
            {
                HideAllPopups();
                lstSmartSuggestions.BeginUpdate();
                lstSmartSuggestions.Items.Clear();
                foreach (var car in matches) lstSmartSuggestions.Items.Add(new CarListItem { Id = car.Id, Name = car.Name });
                lstSmartSuggestions.SelectedIndex = 0;
                lstSmartSuggestions.EndUpdate();

                lstSmartSuggestions.Height = Math.Min(220, matches.Count * lstSmartSuggestions.ItemHeight + 4);

                SyncOverlayLayoutBounds();
                lstSmartSuggestions.Visible = true;
            }
            else
            {
                lstSmartSuggestions.Visible = false;
            }
        }

        private void TxtSmartSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (lstSmartSuggestions.Visible)
            {
                // Intercept Down Arrow to scroll down the passive list
                if (e.KeyCode == Keys.Down)
                {
                    if (lstSmartSuggestions.SelectedIndex < lstSmartSuggestions.Items.Count - 1)
                    {
                        lstSmartSuggestions.SelectedIndex++;
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Prevents caret from jumping around
                }
                // Intercept Up Arrow to scroll up the passive list
                else if (e.KeyCode == Keys.Up)
                {
                    if (lstSmartSuggestions.SelectedIndex > 0)
                    {
                        lstSmartSuggestions.SelectedIndex--;
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                // Intercept Enter to select without leaving the textbox
                else if (e.KeyCode == Keys.Enter)
                {
                    CommitSmartSelection();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                // Intercept Escape to close the dropdown frame instantly
                else if (e.KeyCode == Keys.Escape)
                {
                    lstSmartSuggestions.Visible = false;
                    e.Handled = true;
                }
            }
        }


        private void LstSmartSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { CommitSmartSelection(); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape) { lstSmartSuggestions.Visible = false; txtSmartSearch.Focus(); e.Handled = true; }
        }

        private void CommitSmartSelection()
        {
            if (lstSmartSuggestions.SelectedItem is CarListItem selectedCar)
            {
                var existing = _selectedCarsBinding.FirstOrDefault(c => c.CarId == selectedCar.Id);
                if (existing != null)
                {
                    existing.Count++;
                    dgvSelectedCars.Refresh();
                }
                else
                {
                    _selectedCarsBinding.Add(new CarGridDisplayItem(new CarAllocation { CarId = selectedCar.Id, Count = 1 }, selectedCar.Name));
                }

                lstSmartSuggestions.Visible = false;
                txtSmartSearch.Clear();
                txtSmartSearch.Focus();
                ConfigureDataGridColumns();
                TriggerChangeDebounce();
            }
        }

        private void HideAllPopups()
        {
            if (lstSmartSuggestions != null) lstSmartSuggestions.Visible = false;
            if (lstStrategySuggestions != null) lstStrategySuggestions.Visible = false;
        }

        private void UpdateStrategyLabelDescription()
        {
            if (_config?.Documentation != null && _config.Documentation.TryGetValue(_currentStrategySelection, out string doc))
                lblStrategyDesc.Text = doc;
        }

        private void dgvSelectedCars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvSelectedCars.Columns[e.ColumnIndex].Name == "RemoveAction")
            {
                _selectedCarsBinding.RemoveAt(e.RowIndex);
                TriggerChangeDebounce();
            }
        }

        private void TriggerChangeDebounce() { if (_isLoaded) { tmrSaveDebounce.Stop(); tmrSaveDebounce.Start(); } }

        private void CommitUiToModelAndSave()
        {
            if (_config == null) return;
            _config.PresetName = txtPresetName.Text;
            _config.GridStrategy = _currentStrategySelection;
            _config.BaseSkill = numSkill.Value;
            _config.BaseAggression = numAggression.Value;
            _config.CarAllocations = _selectedCarsBinding.Select(item => item.GetUnderlyingAllocation()).ToList();
            WriteConfigurationToDisk();
        }

        private void WriteConfigurationToDisk()
        {
            try { File.WriteAllText(AppSettings.ParametersJson, JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true })); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }
    }
}
