namespace AcGridGeneratorUi
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblPresetName = new System.Windows.Forms.Label();
            this.txtPresetName = new System.Windows.Forms.TextBox();
            this.lblStrategy = new System.Windows.Forms.Label();
            this.txtStrategyHeader = new System.Windows.Forms.TextBox();
            this.lstStrategySuggestions = new System.Windows.Forms.ListBox();
            this.lblStrategyDesc = new System.Windows.Forms.Label();
            this.lblSkill = new System.Windows.Forms.Label();
            this.numSkill = new System.Windows.Forms.TrackBar();
            this.lblSkillVal = new System.Windows.Forms.Label();
            this.lblAggression = new System.Windows.Forms.Label();
            this.numAggression = new System.Windows.Forms.TrackBar();
            this.lblAggressionVal = new System.Windows.Forms.Label();

            this.pnlRight = new System.Windows.Forms.Panel();
            this.lblSelectCarHeader = new System.Windows.Forms.Label();
            this.txtSmartSearch = new System.Windows.Forms.TextBox();
            this.dgvSelectedCars = new System.Windows.Forms.DataGridView();
            this.lstSmartSuggestions = new System.Windows.Forms.ListBox();
            this.tmrSaveDebounce = new System.Windows.Forms.Timer(this.components);

            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSkill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAggression)).BeginInit();
            this.pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelectedCars)).BeginInit();
            this.SuspendLayout();

            // Default Form Sizing Rules
            this.Size = new System.Drawing.Size(1250, 720);
            this.MinimumSize = new System.Drawing.Size(950, 620);
            this.Text = "Assetto Corsa Grid Preset Manager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // Sidebar Left Settings Panel
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Width = 300;
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(20);

            this.lblPresetName.Text = "Preset Name";
            this.lblPresetName.Location = new System.Drawing.Point(20, 20);
            this.lblPresetName.AutoSize = true;

            this.txtPresetName.Location = new System.Drawing.Point(20, 42);
            this.txtPresetName.Size = new System.Drawing.Size(260, 25);

            this.lblStrategy.Text = "Grid Strategy";
            this.lblStrategy.Location = new System.Drawing.Point(20, 90);
            this.lblStrategy.AutoSize = true;

            // Selector Fake Dropdown Header
            this.txtStrategyHeader.Location = new System.Drawing.Point(20, 112);
            this.txtStrategyHeader.Size = new System.Drawing.Size(260, 25);
            this.txtStrategyHeader.ReadOnly = true;
            this.txtStrategyHeader.Cursor = System.Windows.Forms.Cursors.Hand;

            // Floating Strategy Suggestions List View (Positioned perfectly inside pnlLeft)
            this.lstStrategySuggestions.Location = new System.Drawing.Point(20, 138);
            this.lstStrategySuggestions.Width = 260;
            this.lstStrategySuggestions.Height = 82;
            this.lstStrategySuggestions.Visible = false;
            this.lstStrategySuggestions.IntegralHeight = false;

            this.lblStrategyDesc.Location = new System.Drawing.Point(20, 145);
            this.lblStrategyDesc.Size = new System.Drawing.Size(260, 75);

            this.lblSkill.Text = "Base Skill";
            this.lblSkill.Location = new System.Drawing.Point(20, 240);
            this.lblSkill.AutoSize = true;

            this.lblSkillVal.Location = new System.Drawing.Point(230, 240);
            this.lblSkillVal.Size = new System.Drawing.Size(50, 20);
            this.lblSkillVal.TextAlign = System.Drawing.ContentAlignment.TopRight;

            this.numSkill.Location = new System.Drawing.Point(20, 262);
            this.numSkill.Size = new System.Drawing.Size(260, 45);
            this.numSkill.Minimum = 70;
            this.numSkill.Maximum = 100;
            this.numSkill.TickStyle = System.Windows.Forms.TickStyle.None;

            this.lblAggression.Text = "Base Aggression";
            this.lblAggression.Location = new System.Drawing.Point(20, 320);
            this.lblAggression.AutoSize = true;

            this.lblAggressionVal.Location = new System.Drawing.Point(230, 320);
            this.lblAggressionVal.Size = new System.Drawing.Size(50, 20);
            this.lblAggressionVal.TextAlign = System.Drawing.ContentAlignment.TopRight;

            this.numAggression.Location = new System.Drawing.Point(20, 342);
            this.numAggression.Size = new System.Drawing.Size(260, 45);
            this.numAggression.Minimum = 0;
            this.numAggression.Maximum = 100;
            this.numAggression.TickStyle = System.Windows.Forms.TickStyle.None;

            this.pnlLeft.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lstStrategySuggestions, this.lblPresetName, this.txtPresetName, this.lblStrategy,
                this.txtStrategyHeader, this.lblStrategyDesc, this.lblSkill, this.lblSkillVal,
                this.numSkill, this.lblAggression, this.lblAggressionVal, this.numAggression
            });
            this.lstStrategySuggestions.BringToFront();

            // Main Core Workspace Right Panel
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Padding = new System.Windows.Forms.Padding(20, 15, 20, 20);

            this.lblSelectCarHeader.Text = "Select Car:";
            this.lblSelectCarHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSelectCarHeader.Height = 22;

            this.txtSmartSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSmartSearch.Height = 25;

            // Grid Layout Container Frame
            this.dgvSelectedCars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSelectedCars.AllowUserToAddRows = false;
            this.dgvSelectedCars.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSelectedCars.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // Car Suggestion Overlay List Box
            this.lstSmartSuggestions.Location = new System.Drawing.Point(0, 0);
            this.lstSmartSuggestions.Width = 450;
            this.lstSmartSuggestions.Height = 200;
            this.lstSmartSuggestions.Visible = false;
            this.lstSmartSuggestions.IntegralHeight = false;

            //this.pnlTopSearchContainer.Controls.AddRange(new System.Windows.Forms.Control[] {
            //    this.txtSmartSearch, this.lblSelectCarHeader, this.lstSmartSuggestions
            //});

            //this.pnlGridContainer.Controls.AddRange(new System.Windows.Forms.Control[] {
            //    this.dgvSelectedCars
            //});

            this.lstSmartSuggestions.BringToFront();

            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.pnlRight, this.pnlLeft });

            this.pnlRight.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lstSmartSuggestions, // Now it floats perfectly inside the same space
                this.dgvSelectedCars,
                this.txtSmartSearch,
                this.lblSelectCarHeader
            });

            //this.pnlRight.Controls.AddRange(new System.Windows.Forms.Control[] {
            //    this.pnlGridContainer, this.pnlTopSearchContainer
            //});

            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSkill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAggression)).EndInit();
            this.pnlRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelectedCars)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Label lblPresetName;
        private System.Windows.Forms.TextBox txtPresetName;
        private System.Windows.Forms.Label lblStrategy;
        private System.Windows.Forms.TextBox txtStrategyHeader;
        private System.Windows.Forms.Label lblStrategyDesc;
        private System.Windows.Forms.Label lblSkill;
        private System.Windows.Forms.TrackBar numSkill;
        private System.Windows.Forms.Label lblSkillVal;
        private System.Windows.Forms.Label lblAggression;
        private System.Windows.Forms.TrackBar numAggression;
        private System.Windows.Forms.Label lblAggressionVal;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblSelectCarHeader;
        private System.Windows.Forms.TextBox txtSmartSearch;
        private System.Windows.Forms.ListBox lstSmartSuggestions;
        private System.Windows.Forms.ListBox lstStrategySuggestions;
        private System.Windows.Forms.DataGridView dgvSelectedCars;
        private System.Windows.Forms.Timer tmrSaveDebounce;
    }
}
