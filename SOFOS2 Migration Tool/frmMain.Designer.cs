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
            this.pcbSales = new System.Windows.Forms.PictureBox();
            this.pcbPurchasing = new System.Windows.Forms.PictureBox();
            this.pcbInventory = new System.Windows.Forms.PictureBox();
            this.pcbPayment = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPurchasing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPayment)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRecomputeInventory
            // 
            this.btnRecomputeInventory.Location = new System.Drawing.Point(23, 50);
            this.btnRecomputeInventory.Name = "btnRecomputeInventory";
            this.btnRecomputeInventory.Size = new System.Drawing.Size(190, 78);
            this.btnRecomputeInventory.TabIndex = 0;
            this.btnRecomputeInventory.Text = "Inventory\r\nRunningQty, RValue and Ave. Cost\r\n";
            this.btnRecomputeInventory.UseVisualStyleBackColor = true;
            this.btnRecomputeInventory.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPayment
            // 
            this.btnPayment.Location = new System.Drawing.Point(23, 168);
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(115, 39);
            this.btnPayment.TabIndex = 1;
            this.btnPayment.Text = "Payment";
            this.btnPayment.UseVisualStyleBackColor = true;
            this.btnPayment.Click += new System.EventHandler(this.btnPayment_Click);
            // 
            // btnPurchasing
            // 
            this.btnPurchasing.Location = new System.Drawing.Point(23, 33);
            this.btnPurchasing.Name = "btnPurchasing";
            this.btnPurchasing.Size = new System.Drawing.Size(115, 39);
            this.btnPurchasing.TabIndex = 2;
            this.btnPurchasing.Text = "Purchasing";
            this.btnPurchasing.UseVisualStyleBackColor = true;
            this.btnPurchasing.Click += new System.EventHandler(this.btnPurchasing_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(23, 78);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(115, 39);
            this.btnInventory.TabIndex = 3;
            this.btnInventory.Text = "Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // btnSales
            // 
            this.btnSales.Location = new System.Drawing.Point(23, 123);
            this.btnSales.Name = "btnSales";
            this.btnSales.Size = new System.Drawing.Size(115, 39);
            this.btnSales.TabIndex = 4;
            this.btnSales.Text = "Sales";
            this.btnSales.UseVisualStyleBackColor = true;
            this.btnSales.Click += new System.EventHandler(this.btnSales_Click);
            // 
            // btnRecomputePayment
            // 
            this.btnRecomputePayment.Location = new System.Drawing.Point(23, 134);
            this.btnRecomputePayment.Name = "btnRecomputePayment";
            this.btnRecomputePayment.Size = new System.Drawing.Size(190, 78);
            this.btnRecomputePayment.TabIndex = 5;
            this.btnRecomputePayment.Text = "Payment";
            this.btnRecomputePayment.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRecomputeInventory);
            this.groupBox1.Controls.Add(this.btnRecomputePayment);
            this.groupBox1.Location = new System.Drawing.Point(15, 295);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(237, 263);
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
            this.groupBox2.Location = new System.Drawing.Point(15, 17);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 225);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modules";
            // 
            // pcbSales
            // 
            this.pcbSales.BackColor = System.Drawing.Color.Transparent;
            this.pcbSales.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbSales.Location = new System.Drawing.Point(144, 123);
            this.pcbSales.Name = "pcbSales";
            this.pcbSales.Size = new System.Drawing.Size(39, 39);
            this.pcbSales.TabIndex = 10;
            this.pcbSales.TabStop = false;
            // 
            // pcbPurchasing
            // 
            this.pcbPurchasing.BackColor = System.Drawing.Color.Transparent;
            this.pcbPurchasing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbPurchasing.Location = new System.Drawing.Point(144, 33);
            this.pcbPurchasing.Name = "pcbPurchasing";
            this.pcbPurchasing.Size = new System.Drawing.Size(39, 39);
            this.pcbPurchasing.TabIndex = 9;
            this.pcbPurchasing.TabStop = false;
            // 
            // pcbInventory
            // 
            this.pcbInventory.BackColor = System.Drawing.Color.Transparent;
            this.pcbInventory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbInventory.Location = new System.Drawing.Point(144, 78);
            this.pcbInventory.Name = "pcbInventory";
            this.pcbInventory.Size = new System.Drawing.Size(39, 39);
            this.pcbInventory.TabIndex = 11;
            this.pcbInventory.TabStop = false;
            // 
            // pcbPayment
            // 
            this.pcbPayment.BackColor = System.Drawing.Color.Transparent;
            this.pcbPayment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbPayment.Location = new System.Drawing.Point(144, 168);
            this.pcbPayment.Name = "pcbPayment";
            this.pcbPayment.Size = new System.Drawing.Size(39, 39);
            this.pcbPayment.TabIndex = 12;
            this.pcbPayment.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 574);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(808, 621);
            this.MinimumSize = new System.Drawing.Size(808, 621);
            this.Name = "frmMain";
            this.Text = "Migration Tool v.0.0.1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPurchasing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPayment)).EndInit();
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
    }
}

