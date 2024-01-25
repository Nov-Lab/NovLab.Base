
namespace Test_NovLab
{
    partial class FrmAppTestNovLab
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.BtnExecAllAutoTest = new System.Windows.Forms.Button();
            this.LstTestMethod = new System.Windows.Forms.ListBox();
            this.LstTestResult = new System.Windows.Forms.ListBox();
            this.CMnuTestResult = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CMnuTestResult_MoveFirst = new System.Windows.Forms.ToolStripMenuItem();
            this.CMnuTestResult_MoveLast = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.CMnuTestResult_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.CMnuTestResult_CopyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.TxtTestResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MnuWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.CMnuTestResult.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnExecAllAutoTest
            // 
            this.BtnExecAllAutoTest.Location = new System.Drawing.Point(8, 32);
            this.BtnExecAllAutoTest.Name = "BtnExecAllAutoTest";
            this.BtnExecAllAutoTest.Size = new System.Drawing.Size(128, 32);
            this.BtnExecAllAutoTest.TabIndex = 1;
            this.BtnExecAllAutoTest.Text = "自動テスト全実行(&A)";
            this.BtnExecAllAutoTest.UseVisualStyleBackColor = true;
            this.BtnExecAllAutoTest.Click += new System.EventHandler(this.BtnExecAllAutoTest_Click);
            // 
            // LstTestMethod
            // 
            this.LstTestMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LstTestMethod.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LstTestMethod.FormattingEnabled = true;
            this.LstTestMethod.ItemHeight = 12;
            this.LstTestMethod.Location = new System.Drawing.Point(8, 88);
            this.LstTestMethod.Name = "LstTestMethod";
            this.LstTestMethod.Size = new System.Drawing.Size(784, 136);
            this.LstTestMethod.TabIndex = 3;
            this.LstTestMethod.DoubleClick += new System.EventHandler(this.LstTestMethod_DoubleClick);
            // 
            // LstTestResult
            // 
            this.LstTestResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LstTestResult.ContextMenuStrip = this.CMnuTestResult;
            this.LstTestResult.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LstTestResult.FormattingEnabled = true;
            this.LstTestResult.ItemHeight = 12;
            this.LstTestResult.Location = new System.Drawing.Point(8, 248);
            this.LstTestResult.Name = "LstTestResult";
            this.LstTestResult.Size = new System.Drawing.Size(784, 184);
            this.LstTestResult.TabIndex = 5;
            this.LstTestResult.SelectedIndexChanged += new System.EventHandler(this.LstTestResult_SelectedIndexChanged);
            // 
            // CMnuTestResult
            // 
            this.CMnuTestResult.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CMnuTestResult_MoveFirst,
            this.CMnuTestResult_MoveLast,
            this.toolStripMenuItem1,
            this.CMnuTestResult_Clear,
            this.CMnuTestResult_CopyAll});
            this.CMnuTestResult.Name = "CMnuTestResult";
            this.CMnuTestResult.Size = new System.Drawing.Size(145, 98);
            this.CMnuTestResult.Opening += new System.ComponentModel.CancelEventHandler(this.CMnuTestResult_Opening);
            // 
            // CMnuTestResult_MoveFirst
            // 
            this.CMnuTestResult_MoveFirst.Name = "CMnuTestResult_MoveFirst";
            this.CMnuTestResult_MoveFirst.Size = new System.Drawing.Size(144, 22);
            this.CMnuTestResult_MoveFirst.Text = "最上部へ移動";
            this.CMnuTestResult_MoveFirst.Click += new System.EventHandler(this.CMnuTestResult_MoveFirst_Click);
            // 
            // CMnuTestResult_MoveLast
            // 
            this.CMnuTestResult_MoveLast.Name = "CMnuTestResult_MoveLast";
            this.CMnuTestResult_MoveLast.Size = new System.Drawing.Size(144, 22);
            this.CMnuTestResult_MoveLast.Text = "最下部へ移動";
            this.CMnuTestResult_MoveLast.Click += new System.EventHandler(this.CMnuTestResult_MoveLast_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(141, 6);
            // 
            // CMnuTestResult_Clear
            // 
            this.CMnuTestResult_Clear.Name = "CMnuTestResult_Clear";
            this.CMnuTestResult_Clear.Size = new System.Drawing.Size(144, 22);
            this.CMnuTestResult_Clear.Text = "クリア(&L)";
            this.CMnuTestResult_Clear.Click += new System.EventHandler(this.CMnuTestResult_Clear_Click);
            // 
            // CMnuTestResult_CopyAll
            // 
            this.CMnuTestResult_CopyAll.Name = "CMnuTestResult_CopyAll";
            this.CMnuTestResult_CopyAll.Size = new System.Drawing.Size(144, 22);
            this.CMnuTestResult_CopyAll.Text = "すべてコピー(&C)";
            this.CMnuTestResult_CopyAll.Click += new System.EventHandler(this.CMnuTestResult_CopyAll_Click);
            // 
            // TxtTestResult
            // 
            this.TxtTestResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtTestResult.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TxtTestResult.Location = new System.Drawing.Point(8, 440);
            this.TxtTestResult.Multiline = true;
            this.TxtTestResult.Name = "TxtTestResult";
            this.TxtTestResult.ReadOnly = true;
            this.TxtTestResult.Size = new System.Drawing.Size(784, 113);
            this.TxtTestResult.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "テスト項目(&M):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "テスト結果(&R):";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuWindow});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MnuWindow
            // 
            this.MnuWindow.Name = "MnuWindow";
            this.MnuWindow.Size = new System.Drawing.Size(80, 20);
            this.MnuWindow.Text = "ウィンドウ(&W)";
            // 
            // FrmAppTestNovLab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 563);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtTestResult);
            this.Controls.Add(this.LstTestResult);
            this.Controls.Add(this.LstTestMethod);
            this.Controls.Add(this.BtnExecAllAutoTest);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmAppTestNovLab";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NovLabテスト";
            this.Load += new System.EventHandler(this.FrmAppTestNovLabBase_Load);
            this.CMnuTestResult.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnExecAllAutoTest;
        private System.Windows.Forms.ListBox LstTestMethod;
        private System.Windows.Forms.ListBox LstTestResult;
        private System.Windows.Forms.TextBox TxtTestResult;
        private System.Windows.Forms.ContextMenuStrip CMnuTestResult;
        private System.Windows.Forms.ToolStripMenuItem CMnuTestResult_Clear;
        private System.Windows.Forms.ToolStripMenuItem CMnuTestResult_MoveLast;
        private System.Windows.Forms.ToolStripMenuItem CMnuTestResult_MoveFirst;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem CMnuTestResult_CopyAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MnuWindow;
    }
}

