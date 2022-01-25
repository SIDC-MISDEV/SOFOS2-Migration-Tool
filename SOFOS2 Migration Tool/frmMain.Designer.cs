namespace SOFOS2_Migration_Tool
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRecomputeInventory = new System.Windows.Forms.Button();
            this.btnPayment = new System.Windows.Forms.Button();
            this.btnPurchasing = new System.Windows.Forms.Button();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnSales = new System.Windows.Forms.Button();
            this.btnRecomputePayment = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pcbPayment = new System.Windows.Forms.PictureBox();
            this.pcbInventory = new System.Windows.Forms.PictureBox();
            this.pcbSales = new System.Windows.Forms.PictureBox();
            this.pcbPurchasing = new System.Windows.Forms.PictureBox();
            this.pcbRecomputePayment = new System.Windows.Forms.PictureBox();
            this.pcbRecomputeInventory = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPayment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPurchasing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputePayment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputeInventory)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRecomputeInventory
            // 
            this.btnRecomputeInventory.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRecomputeInventory.Location = new System.Drawing.Point(78, 37);
            this.btnRecomputeInventory.Name = "btnRecomputeInventory";
            this.btnRecomputeInventory.Size = new System.Drawing.Size(115, 39);
            this.btnRecomputeInventory.TabIndex = 0;
            this.btnRecomputeInventory.Text = "Inventory";
            this.btnRecomputeInventory.UseVisualStyleBackColor = true;
            this.btnRecomputeInventory.Click += new System.EventHandler(this.btnRecomputeInventory_Click);
            // 
            // btnPayment
            // 
            this.btnPayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPayment.Location = new System.Drawing.Point(78, 160);
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(115, 39);
            this.btnPayment.TabIndex = 1;
            this.btnPayment.Text = "Payment";
            this.btnPayment.UseVisualStyleBackColor = true;
            this.btnPayment.Click += new System.EventHandler(this.btnPayment_Click);
            // 
            // btnPurchasing
            // 
            this.btnPurchasing.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPurchasing.Location = new System.Drawing.Point(78, 25);
            this.btnPurchasing.Name = "btnPurchasing";
            this.btnPurchasing.Size = new System.Drawing.Size(115, 39);
            this.btnPurchasing.TabIndex = 2;
            this.btnPurchasing.Text = "Purchasing";
            this.btnPurchasing.UseVisualStyleBackColor = true;
            this.btnPurchasing.Click += new System.EventHandler(this.btnPurchasing_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInventory.Location = new System.Drawing.Point(78, 70);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(115, 39);
            this.btnInventory.TabIndex = 3;
            this.btnInventory.Text = "Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // btnSales
            // 
            this.btnSales.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSales.Location = new System.Drawing.Point(78, 115);
            this.btnSales.Name = "btnSales";
            this.btnSales.Size = new System.Drawing.Size(115, 39);
            this.btnSales.TabIndex = 4;
            this.btnSales.Text = "Sales";
            this.btnSales.UseVisualStyleBackColor = true;
            this.btnSales.Click += new System.EventHandler(this.btnSales_Click);
            // 
            // btnRecomputePayment
            // 
            this.btnRecomputePayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRecomputePayment.Location = new System.Drawing.Point(78, 82);
            this.btnRecomputePayment.Name = "btnRecomputePayment";
            this.btnRecomputePayment.Size = new System.Drawing.Size(115, 39);
            this.btnRecomputePayment.TabIndex = 5;
            this.btnRecomputePayment.Text = "Payment";
            this.btnRecomputePayment.UseVisualStyleBackColor = true;
            this.btnRecomputePayment.Click += new System.EventHandler(this.btnRecomputePayment_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pcbRecomputePayment);
            this.groupBox1.Controls.Add(this.pcbRecomputeInventory);
            this.groupBox1.Controls.Add(this.btnRecomputeInventory);
            this.groupBox1.Controls.Add(this.btnRecomputePayment);
            this.groupBox1.Location = new System.Drawing.Point(39, 262);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 159);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Re-compute";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pcbPayment);
            this.groupBox2.Controls.Add(this.pcbInventory);
            this.groupBox2.Controls.Add(this.pcbSales);
            this.groupBox2.Controls.Add(this.pcbPurchasing);
            this.groupBox2.Controls.Add(this.btnPurchasing);
            this.groupBox2.Controls.Add(this.btnPayment);
            this.groupBox2.Controls.Add(this.btnInventory);
            this.groupBox2.Controls.Add(this.btnSales);
            this.groupBox2.Location = new System.Drawing.Point(39, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(316, 225);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modules";
            // 
            // pcbPayment
            // 
            this.pcbPayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbPayment.BackColor = System.Drawing.Color.Transparent;
            this.pcbPayment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbPayment.Location = new System.Drawing.Point(199, 160);
            this.pcbPayment.Name = "pcbPayment";
            this.pcbPayment.Size = new System.Drawing.Size(39, 39);
            this.pcbPayment.TabIndex = 12;
            this.pcbPayment.TabStop = false;
            // 
            // pcbInventory
            // 
            this.pcbInventory.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbInventory.BackColor = System.Drawing.Color.Transparent;
            this.pcbInventory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbInventory.Location = new System.Drawing.Point(199, 70);
            this.pcbInventory.Name = "pcbInventory";
            this.pcbInventory.Size = new System.Drawing.Size(39, 39);
            this.pcbInventory.TabIndex = 11;
            this.pcbInventory.TabStop = false;
            // 
            // pcbSales
            // 
            this.pcbSales.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbSales.BackColor = System.Drawing.Color.Transparent;
            this.pcbSales.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbSales.Location = new System.Drawing.Point(199, 115);
            this.pcbSales.Name = "pcbSales";
            this.pcbSales.Size = new System.Drawing.Size(39, 39);
            this.pcbSales.TabIndex = 10;
            this.pcbSales.TabStop = false;
            // 
            // pcbPurchasing
            // 
            this.pcbPurchasing.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbPurchasing.BackColor = System.Drawing.Color.Transparent;
            this.pcbPurchasing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbPurchasing.Location = new System.Drawing.Point(199, 25);
            this.pcbPurchasing.Name = "pcbPurchasing";
            this.pcbPurchasing.Size = new System.Drawing.Size(39, 39);
            this.pcbPurchasing.TabIndex = 9;
            this.pcbPurchasing.TabStop = false;
            // 
            // pcbRecomputePayment
            // 
            this.pcbRecomputePayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbRecomputePayment.BackColor = System.Drawing.Color.Transparent;
            this.pcbRecomputePayment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbRecomputePayment.Location = new System.Drawing.Point(199, 82);
            this.pcbRecomputePayment.Name = "pcbRecomputePayment";
            this.pcbRecomputePayment.Size = new System.Drawing.Size(39, 39);
            this.pcbRecomputePayment.TabIndex = 14;
            this.pcbRecomputePayment.TabStop = false;
            // 
            // pcbRecomputeInventory
            // 
            this.pcbRecomputeInventory.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbRecomputeInventory.BackColor = System.Drawing.Color.Transparent;
            this.pcbRecomputeInventory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbRecomputeInventory.Location = new System.Drawing.Point(199, 37);
            this.pcbRecomputeInventory.Name = "pcbRecomputeInventory";
            this.pcbRecomputeInventory.Size = new System.Drawing.Size(39, 39);
            this.pcbRecomputeInventory.TabIndex = 13;
            this.pcbRecomputeInventory.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 453);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Migration Tool v.0.0.1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbPayment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPurchasing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputePayment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputeInventory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRecomputeInventory;
        private System.Windows.Forms.Button btnPayment;
        private System.Windows.Forms.Button btnPurchasing;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnSales;
        private System.Windows.Forms.Button btnRecomputePayment;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pcbPurchasing;
        private System.Windows.Forms.PictureBox pcbSales;
        private System.Windows.Forms.PictureBox pcbPayment;
        private System.Windows.Forms.PictureBox pcbInventory;
        private System.Windows.Forms.PictureBox pcbRecomputePayment;
        private System.Windows.Forms.PictureBox pcbRecomputeInventory;
    }
}

