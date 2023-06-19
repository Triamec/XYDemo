namespace Triamec.Tam.Samples {
    partial class HelloWorldForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem exitMenuItem;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelloWorldForm));
            System.Windows.Forms.GroupBox motionGroupBox;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.GroupBox measurementGroupBox;
            System.Windows.Forms.Label label1;
            this._StartButton = new System.Windows.Forms.Button();
            this._StopButton = new System.Windows.Forms.Button();
            this._xPositionBox = new System.Windows.Forms.TextBox();
            this._yPositionBox = new System.Windows.Forms.TextBox();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._timer = new System.Windows.Forms.Timer(this.components);
            menuStrip = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            motionGroupBox = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            measurementGroupBox = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            menuStrip.SuspendLayout();
            motionGroupBox.SuspendLayout();
            measurementGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileToolStripMenuItem});
            resources.ApplyResources(menuStrip, "menuStrip");
            menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            exitMenuItem});
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            resources.ApplyResources(exitMenuItem, "exitMenuItem");
            exitMenuItem.Click += new System.EventHandler(this.OnExitMenuItemClick);
            // 
            // motionGroupBox
            // 
            motionGroupBox.Controls.Add(this._StartButton);
            motionGroupBox.Controls.Add(this._StopButton);
            resources.ApplyResources(motionGroupBox, "motionGroupBox");
            motionGroupBox.Name = "motionGroupBox";
            motionGroupBox.TabStop = false;
            // 
            // _StartButton
            // 
            resources.ApplyResources(this._StartButton, "_StartButton");
            this._StartButton.Name = "_StartButton";
            this._toolTip.SetToolTip(this._StartButton, resources.GetString("_StartButton.ToolTip"));
            this._StartButton.UseVisualStyleBackColor = true;
            this._StartButton.Click += new System.EventHandler(this.OnStartButtonClick);
            // 
            // _StopButton
            // 
            resources.ApplyResources(this._StopButton, "_StopButton");
            this._StopButton.Name = "_StopButton";
            this._toolTip.SetToolTip(this._StopButton, resources.GetString("_StopButton.ToolTip"));
            this._StopButton.UseVisualStyleBackColor = true;
            this._StopButton.Click += new System.EventHandler(this.OnStopButtonClick);
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // measurementGroupBox
            // 
            measurementGroupBox.Controls.Add(label1);
            measurementGroupBox.Controls.Add(this._xPositionBox);
            measurementGroupBox.Controls.Add(this._yPositionBox);
            measurementGroupBox.Controls.Add(label4);
            resources.ApplyResources(measurementGroupBox, "measurementGroupBox");
            measurementGroupBox.Name = "measurementGroupBox";
            measurementGroupBox.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // _xPositionBox
            // 
            resources.ApplyResources(this._xPositionBox, "_xPositionBox");
            this._xPositionBox.Name = "_xPositionBox";
            this._xPositionBox.ReadOnly = true;
            // 
            // _yPositionBox
            // 
            resources.ApplyResources(this._yPositionBox, "_yPositionBox");
            this._yPositionBox.Name = "_yPositionBox";
            this._yPositionBox.ReadOnly = true;
            // 
            // _timer
            // 
            this._timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // HelloWorldForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(measurementGroupBox);
            this.Controls.Add(motionGroupBox);
            this.Controls.Add(menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = menuStrip;
            this.Name = "HelloWorldForm";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            motionGroupBox.ResumeLayout(false);
            measurementGroupBox.ResumeLayout(false);
            measurementGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _StartButton;
        private System.Windows.Forms.ToolTip _toolTip;
        private System.Windows.Forms.Button _StopButton;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.TextBox _yPositionBox;
        private System.Windows.Forms.TextBox _xPositionBox;
    }
}

