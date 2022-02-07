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
            this.pcbRecomputeSalesCreditLimit = new System.Windows.Forms.PictureBox();
            this.btnRecomputeSalesCreditLimit = new System.Windows.Forms.Button();
            this.pcbRecomputePayment = new System.Windows.Forms.PictureBox();
            this.pcbRecomputeInventory = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pcbCreditLimit = new System.Windows.Forms.PictureBox();
            this.btnCreditLimit = new System.Windows.Forms.Button();
            this.pcbPayment = new System.Windows.Forms.PictureBox();
            this.pcbInventory = new System.Windows.Forms.PictureBox();
            this.pcbSales = new System.Windows.Forms.PictureBox();
            this.pcbPurchasing = new System.Windows.Forms.PictureBox();
            this.dtpDateParam = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputeSalesCreditLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputePayment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputeInventory)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcbCreditLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPayment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPurchasing)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRecomputeInventory
            // 
            this.btnRecomputeInventory.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRecomputeInventory.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecomputeInventory.Location = new System.Drawing.Point(62, 26);
            this.btnRecomputeInventory.Name = "btnRecomputeInventory";
            this.btnRecomputeInventory.Size = new System.Drawing.Size(147, 39);
            this.btnRecomputeInventory.TabIndex = 7;
            this.btnRecomputeInventory.Text = "Inventory";
            this.btnRecomputeInventory.UseVisualStyleBackColor = true;
            this.btnRecomputeInventory.Click += new System.EventHandler(this.btnRecomputeInventory_Click);
            // 
            // btnPayment
            // 
            this.btnPayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPayment.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPayment.Location = new System.Drawing.Point(62, 152);
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(147, 39);
            this.btnPayment.TabIndex = 5;
            this.btnPayment.Text = "Payment";
            this.btnPayment.UseVisualStyleBackColor = true;
            this.btnPayment.Click += new System.EventHandler(this.btnPayment_Click);
            // 
            // btnPurchasing
            // 
            this.btnPurchasing.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPurchasing.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPurchasing.Location = new System.Drawing.Point(62, 17);
            this.btnPurchasing.Name = "btnPurchasing";
            this.btnPurchasing.Size = new System.Drawing.Size(147, 39);
            this.btnPurchasing.TabIndex = 2;
            this.btnPurchasing.Text = "Purchasing";
            this.btnPurchasing.UseVisualStyleBackColor = true;
            this.btnPurchasing.Click += new System.EventHandler(this.btnPurchasing_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInventory.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInventory.Location = new System.Drawing.Point(62, 62);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(147, 39);
            this.btnInventory.TabIndex = 3;
            this.btnInventory.Text = "Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // btnSales
            // 
            this.btnSales.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSales.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSales.Location = new System.Drawing.Point(62, 107);
            this.btnSales.Name = "btnSales";
            this.btnSales.Size = new System.Drawing.Size(147, 39);
            this.btnSales.TabIndex = 4;
            this.btnSales.Text = "Sales";
            this.btnSales.UseVisualStyleBackColor = true;
            this.btnSales.Click += new System.EventHandler(this.btnSales_Click);
            // 
            // btnRecomputePayment
            // 
            this.btnRecomputePayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRecomputePayment.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecomputePayment.Location = new System.Drawing.Point(62, 119);
            this.btnRecomputePayment.Name = "btnRecomputePayment";
            this.btnRecomputePayment.Size = new System.Drawing.Size(147, 39);
            this.btnRecomputePayment.TabIndex = 9;
            this.btnRecomputePayment.Text = "Payment";
            this.btnRecomputePayment.UseVisualStyleBackColor = true;
            this.btnRecomputePayment.Click += new System.EventHandler(this.btnRecomputePayment_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.Controls.Add(this.pcbRecomputeSalesCreditLimit);
            this.groupBox1.Controls.Add(this.btnRecomputeSalesCreditLimit);
            this.groupBox1.Controls.Add(this.pcbRecomputePayment);
            this.groupBox1.Controls.Add(this.pcbRecomputeInventory);
            this.groupBox1.Controls.Add(this.btnRecomputeInventory);
            this.groupBox1.Controls.Add(this.btnRecomputePayment);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(32, 341);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(316, 184);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Re-compute";
            // 
            // pcbRecomputeSalesCreditLimit
            // 
            this.pcbRecomputeSalesCreditLimit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbRecomputeSalesCreditLimit.BackColor = System.Drawing.Color.Transparent;
            this.pcbRecomputeSalesCreditLimit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbRecomputeSalesCreditLimit.Location = new System.Drawing.Point(215, 72);
            this.pcbRecomputeSalesCreditLimit.Name = "pcbRecomputeSalesCreditLimit";
            this.pcbRecomputeSalesCreditLimit.Size = new System.Drawing.Size(39, 39);
            this.pcbRecomputeSalesCreditLimit.TabIndex = 16;
            this.pcbRecomputeSalesCreditLimit.TabStop = false;
            // 
            // btnRecomputeSalesCreditLimit
            // 
            this.btnRecomputeSalesCreditLimit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRecomputeSalesCreditLimit.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecomputeSalesCreditLimit.Location = new System.Drawing.Point(62, 72);
            this.btnRecomputeSalesCreditLimit.Name = "btnRecomputeSalesCreditLimit";
            this.btnRecomputeSalesCreditLimit.Size = new System.Drawing.Size(147, 39);
            this.btnRecomputeSalesCreditLimit.TabIndex = 8;
            this.btnRecomputeSalesCreditLimit.Text = "Sales Credit Limit";
            this.btnRecomputeSalesCreditLimit.UseVisualStyleBackColor = true;
            this.btnRecomputeSalesCreditLimit.Click += new System.EventHandler(this.btnRecomputeSalesCreditLimit_Click);
            // 
            // pcbRecomputePayment
            // 
            this.pcbRecomputePayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbRecomputePayment.BackColor = System.Drawing.Color.Transparent;
            this.pcbRecomputePayment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbRecomputePayment.Location = new System.Drawing.Point(215, 119);
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
            this.pcbRecomputeInventory.Location = new System.Drawing.Point(215, 26);
            this.pcbRecomputeInventory.Name = "pcbRecomputeInventory";
            this.pcbRecomputeInventory.Size = new System.Drawing.Size(39, 39);
            this.pcbRecomputeInventory.TabIndex = 13;
            this.pcbRecomputeInventory.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox2.Controls.Add(this.pcbCreditLimit);
            this.groupBox2.Controls.Add(this.btnCreditLimit);
            this.groupBox2.Controls.Add(this.pcbPayment);
            this.groupBox2.Controls.Add(this.pcbInventory);
            this.groupBox2.Controls.Add(this.pcbSales);
            this.groupBox2.Controls.Add(this.pcbPurchasing);
            this.groupBox2.Controls.Add(this.btnPurchasing);
            this.groupBox2.Controls.Add(this.btnPayment);
            this.groupBox2.Controls.Add(this.btnInventory);
            this.groupBox2.Controls.Add(this.btnSales);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(32, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(316, 256);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modules";
            // 
            // pcbCreditLimit
            // 
            this.pcbCreditLimit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbCreditLimit.BackColor = System.Drawing.Color.Transparent;
            this.pcbCreditLimit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbCreditLimit.Location = new System.Drawing.Point(215, 198);
            this.pcbCreditLimit.Name = "pcbCreditLimit";
            this.pcbCreditLimit.Size = new System.Drawing.Size(39, 39);
            this.pcbCreditLimit.TabIndex = 16;
            this.pcbCreditLimit.TabStop = false;
            // 
            // btnCreditLimit
            // 
            this.btnCreditLimit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCreditLimit.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreditLimit.Location = new System.Drawing.Point(62, 199);
            this.btnCreditLimit.Name = "btnCreditLimit";
            this.btnCreditLimit.Size = new System.Drawing.Size(147, 39);
            this.btnCreditLimit.TabIndex = 6;
            this.btnCreditLimit.Text = "Credit limit - CI";
            this.btnCreditLimit.UseVisualStyleBackColor = true;
            this.btnCreditLimit.Click += new System.EventHandler(this.btnCreditLimit_Click);
            // 
            // pcbPayment
            // 
            this.pcbPayment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pcbPayment.BackColor = System.Drawing.Color.Transparent;
            this.pcbPayment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pcbPayment.Location = new System.Drawing.Point(215, 152);
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
            this.pcbInventory.Location = new System.Drawing.Point(215, 62);
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
            this.pcbSales.Location = new System.Drawing.Point(215, 107);
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
            this.pcbPurchasing.Location = new System.Drawing.Point(215, 17);
            this.pcbPurchasing.Name = "pcbPurchasing";
            this.pcbPurchasing.Size = new System.Drawing.Size(39, 39);
            this.pcbPurchasing.TabIndex = 9;
            this.pcbPurchasing.TabStop = false;
            // 
            // dtpDateParam
            // 
            this.dtpDateParam.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dtpDateParam.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDateParam.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateParam.Location = new System.Drawing.Point(231, 41);
            this.dtpDateParam.Name = "dtpDateParam";
            this.dtpDateParam.Size = new System.Drawing.Size(117, 25);
            this.dtpDateParam.TabIndex = 1;
            this.dtpDateParam.ValueChanged += new System.EventHandler(this.dtpDateParam_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(179, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 19);
            this.label1.TabIndex = 10;
            this.label1.Text = "Date :";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 535);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpDateParam);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Migration Tool v.0.0.1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputeSalesCreditLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputePayment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbRecomputeInventory)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcbCreditLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPayment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbPurchasing)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.DateTimePicker dtpDateParam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pcbCreditLimit;
        private System.Windows.Forms.Button btnCreditLimit;
        private System.Windows.Forms.PictureBox pcbRecomputeSalesCreditLimit;
        private System.Windows.Forms.Button btnRecomputeSalesCreditLimit;
    }
}

