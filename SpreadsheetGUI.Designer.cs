using SpreadsheetGrid_Core;

/// <summary> 
/// Author:    Matthew Schnitter 
/// Partner:   None 
/// Date:      3/4/2022 
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 and Matthew Schnitter - This work may not be copied for use in Academic Coursework. 
/// 
/// I, Matthew Schnitter, certify that I wrote this code from scratch and did not copy it in part or whole from  
/// another source.  All references used in the completion of the assignment are cited in my README file. 
/// 
/// File Contents 
/// Auto-generated designer code
/// </summary>

namespace GUI
{
    /// <summary>
    /// GUI designer class
    /// </summary>
    partial class SpreadsheetGUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridWidget = new SpreadsheetGrid_Core.SpreadsheetGridWidget();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentLabel = new System.Windows.Forms.Label();
            this.contentBox = new System.Windows.Forms.TextBox();
            this.valueLabel = new System.Windows.Forms.Label();
            this.valueBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.helpButton = new System.Windows.Forms.Button();
            this.colToSumLabel = new System.Windows.Forms.Label();
            this.colToSumTextBox = new System.Windows.Forms.TextBox();
            this.sumColumn = new System.Windows.Forms.Button();
            this.sumOfColTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridWidget
            // 
            this.gridWidget.AutoSize = true;
            this.gridWidget.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.gridWidget.Location = new System.Drawing.Point(0, 100);
            this.gridWidget.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.gridWidget.MaximumSize = new System.Drawing.Size(3500, 3846);
            this.gridWidget.Name = "gridWidget";
            this.gridWidget.Size = new System.Drawing.Size(1301, 500);
            this.gridWidget.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1301, 33);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(158, 34);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // contentLabel
            // 
            this.contentLabel.AutoSize = true;
            this.contentLabel.Location = new System.Drawing.Point(12, 33);
            this.contentLabel.Name = "contentLabel";
            this.contentLabel.Size = new System.Drawing.Size(120, 25);
            this.contentLabel.TabIndex = 2;
            this.contentLabel.Text = "Cell Contents:";
            // 
            // contentBox
            // 
            this.contentBox.Location = new System.Drawing.Point(12, 63);
            this.contentBox.Name = "contentBox";
            this.contentBox.Size = new System.Drawing.Size(150, 31);
            this.contentBox.TabIndex = 3;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(199, 33);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(91, 25);
            this.valueLabel.TabIndex = 4;
            this.valueLabel.Text = "Cell Value:";
            // 
            // valueBox
            // 
            this.valueBox.Location = new System.Drawing.Point(199, 63);
            this.valueBox.Name = "valueBox";
            this.valueBox.ReadOnly = true;
            this.valueBox.Size = new System.Drawing.Size(150, 31);
            this.valueBox.TabIndex = 5;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(384, 35);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(96, 25);
            this.nameLabel.TabIndex = 6;
            this.nameLabel.Text = "Cell Name:";
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(384, 63);
            this.nameBox.Name = "nameBox";
            this.nameBox.ReadOnly = true;
            this.nameBox.Size = new System.Drawing.Size(150, 31);
            this.nameBox.TabIndex = 7;
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(808, 60);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(112, 34);
            this.helpButton.TabIndex = 8;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // colToSumLabel
            // 
            this.colToSumLabel.AutoSize = true;
            this.colToSumLabel.Location = new System.Drawing.Point(573, 36);
            this.colToSumLabel.Name = "colToSumLabel";
            this.colToSumLabel.Size = new System.Drawing.Size(117, 25);
            this.colToSumLabel.TabIndex = 9;
            this.colToSumLabel.Text = "Add Column:";
            // 
            // colToSumTextBox
            // 
            this.colToSumTextBox.Location = new System.Drawing.Point(698, 33);
            this.colToSumTextBox.Name = "colToSumTextBox";
            this.colToSumTextBox.Size = new System.Drawing.Size(31, 31);
            this.colToSumTextBox.TabIndex = 10;
            // 
            // sumColumn
            // 
            this.sumColumn.Location = new System.Drawing.Point(564, 64);
            this.sumColumn.Name = "sumColumn";
            this.sumColumn.Size = new System.Drawing.Size(128, 34);
            this.sumColumn.TabIndex = 11;
            this.sumColumn.Text = "Sum Column:";
            this.sumColumn.UseVisualStyleBackColor = true;
            this.sumColumn.Click += new System.EventHandler(this.sumColumn_Click);
            // 
            // sumOfColTextBox
            // 
            this.sumOfColTextBox.Location = new System.Drawing.Point(698, 66);
            this.sumOfColTextBox.Name = "sumOfColTextBox";
            this.sumOfColTextBox.ReadOnly = true;
            this.sumOfColTextBox.Size = new System.Drawing.Size(81, 31);
            this.sumOfColTextBox.TabIndex = 12;
            // 
            // SpreadsheetGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1301, 450);
            this.Controls.Add(this.sumOfColTextBox);
            this.Controls.Add(this.sumColumn);
            this.Controls.Add(this.colToSumTextBox);
            this.Controls.Add(this.colToSumLabel);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.contentBox);
            this.Controls.Add(this.contentLabel);
            this.Controls.Add(this.gridWidget);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpreadsheetGUI";
            this.Text = "Spreadsheet";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SpreadsheetGridWidget gridWidget;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private Label contentLabel;
        private TextBox contentBox;
        private Label valueLabel;
        private TextBox valueBox;
        private Label nameLabel;
        private TextBox nameBox;
        private Button helpButton;
        private Label colToSumLabel;
        private TextBox colToSumTextBox;
        private Button sumColumn;
        private TextBox sumOfColTextBox;
    }
}