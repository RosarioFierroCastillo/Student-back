namespace Huella_Softwarianos
{
    partial class ReportChecador
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 

        /*
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
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DatePicker2 = new System.Windows.Forms.DateTimePicker();
            this.DatePicker1 = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBoxTodo = new System.Windows.Forms.CheckBox();
            this.chkBoxIdVenta = new System.Windows.Forms.CheckBox();
            this.chkBoxIdProd = new System.Windows.Forms.CheckBox();
            this.chkBoxFechas = new System.Windows.Forms.CheckBox();
            this.btnReturn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtIdProducto = new System.Windows.Forms.TextBox();
            this.txtIdVenta = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Huella_Softwarianos.ReportChecador.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(1, 94);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(1693, 725);
            this.reportViewer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.DatePicker2);
            this.groupBox2.Controls.Add(this.DatePicker1);
            this.groupBox2.Location = new System.Drawing.Point(494, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(650, 76);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fecha de Reporte";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(308, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "a";
            // 
            // DatePicker2
            // 
            this.DatePicker2.Enabled = false;
            this.DatePicker2.Location = new System.Drawing.Point(354, 25);
            this.DatePicker2.Name = "DatePicker2";
            this.DatePicker2.Size = new System.Drawing.Size(264, 22);
            this.DatePicker2.TabIndex = 4;
            // 
            // DatePicker1
            // 
            this.DatePicker1.Location = new System.Drawing.Point(6, 27);
            this.DatePicker1.Name = "DatePicker1";
            this.DatePicker1.Size = new System.Drawing.Size(274, 22);
            this.DatePicker1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBoxTodo);
            this.groupBox1.Controls.Add(this.chkBoxIdVenta);
            this.groupBox1.Controls.Add(this.chkBoxIdProd);
            this.groupBox1.Controls.Add(this.chkBoxFechas);
            this.groupBox1.Location = new System.Drawing.Point(1, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(467, 76);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parámetros";
            // 
            // chkBoxTodo
            // 
            this.chkBoxTodo.AutoSize = true;
            this.chkBoxTodo.Location = new System.Drawing.Point(377, 27);
            this.chkBoxTodo.Name = "chkBoxTodo";
            this.chkBoxTodo.Size = new System.Drawing.Size(86, 20);
            this.chkBoxTodo.TabIndex = 3;
            this.chkBoxTodo.Text = "Ver Todo";
            this.chkBoxTodo.UseVisualStyleBackColor = true;
            // 
            // chkBoxIdVenta
            // 
            this.chkBoxIdVenta.AutoSize = true;
            this.chkBoxIdVenta.Location = new System.Drawing.Point(293, 27);
            this.chkBoxIdVenta.Name = "chkBoxIdVenta";
            this.chkBoxIdVenta.Size = new System.Drawing.Size(78, 20);
            this.chkBoxIdVenta.TabIndex = 2;
            this.chkBoxIdVenta.Text = "Id Venta";
            this.chkBoxIdVenta.UseVisualStyleBackColor = true;
            this.chkBoxIdVenta.Visible = false;
            // 
            // chkBoxIdProd
            // 
            this.chkBoxIdProd.AutoSize = true;
            this.chkBoxIdProd.Location = new System.Drawing.Point(175, 27);
            this.chkBoxIdProd.Name = "chkBoxIdProd";
            this.chkBoxIdProd.Size = new System.Drawing.Size(97, 20);
            this.chkBoxIdProd.TabIndex = 1;
            this.chkBoxIdProd.Text = "Id Producto";
            this.chkBoxIdProd.UseVisualStyleBackColor = true;
            this.chkBoxIdProd.Visible = false;
            // 
            // chkBoxFechas
            // 
            this.chkBoxFechas.AutoSize = true;
            this.chkBoxFechas.Location = new System.Drawing.Point(6, 27);
            this.chkBoxFechas.Name = "chkBoxFechas";
            this.chkBoxFechas.Size = new System.Drawing.Size(67, 20);
            this.chkBoxFechas.TabIndex = 0;
            this.chkBoxFechas.Text = "Fecha";
            this.chkBoxFechas.UseVisualStyleBackColor = true;
            this.chkBoxFechas.CheckedChanged += new System.EventHandler(this.chkBoxFechas_CheckedChanged_1);
            // 
            // btnReturn
            // 
            this.btnReturn.Location = new System.Drawing.Point(1197, 14);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(75, 31);
            this.btnReturn.TabIndex = 7;
            this.btnReturn.Text = "Regresar";
            this.btnReturn.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1197, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 31);
            this.button1.TabIndex = 6;
            this.button1.Text = "Mostrar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtIdProducto
            // 
            this.txtIdProducto.Location = new System.Drawing.Point(6, 48);
            this.txtIdProducto.Name = "txtIdProducto";
            this.txtIdProducto.Size = new System.Drawing.Size(100, 22);
            this.txtIdProducto.TabIndex = 0;
            // 
            // txtIdVenta
            // 
            this.txtIdVenta.Location = new System.Drawing.Point(199, 48);
            this.txtIdVenta.Name = "txtIdVenta";
            this.txtIdVenta.Size = new System.Drawing.Size(100, 22);
            this.txtIdVenta.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Id Prodcuto";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(196, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Id Venta";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txtIdVenta);
            this.groupBox3.Controls.Add(this.txtIdProducto);
            this.groupBox3.Location = new System.Drawing.Point(1377, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(305, 76);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ID\'s";
            // 
            // ReportChecador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1694, 812);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.reportViewer1);
            this.Name = "ReportChecador";
            this.Text = "ReportPersonas";
            this.Load += new System.EventHandler(this.ReportPersonas_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker DatePicker2;
        private System.Windows.Forms.DateTimePicker DatePicker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkBoxTodo;
        private System.Windows.Forms.CheckBox chkBoxIdVenta;
        private System.Windows.Forms.CheckBox chkBoxIdProd;
        private System.Windows.Forms.CheckBox chkBoxFechas;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtIdProducto;
        private System.Windows.Forms.TextBox txtIdVenta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        */
    }
}