
namespace Test_NovLab
{
    partial class FrmTestAsyncMethod
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
            this.components = new System.ComponentModel.Container();
            this.BtnTaskExceptionTest1 = new System.Windows.Forms.Button();
            this.BtnTaskExceptionTest2 = new System.Windows.Forms.Button();
            this.BtnTaskExceptionTest3 = new System.Windows.Forms.Button();
            this.BtnTaskExceptionTest4 = new System.Windows.Forms.Button();
            this.BtnTaskExceptionTest5 = new System.Windows.Forms.Button();
            this.TmrCheckTask = new System.Windows.Forms.Timer(this.components);
            this.LstMessage = new System.Windows.Forms.ListBox();
            this.LblTest1 = new System.Windows.Forms.Label();
            this.LblTest2 = new System.Windows.Forms.Label();
            this.LblTest3 = new System.Windows.Forms.Label();
            this.LblTest4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.GrpTaskExceptionTest = new System.Windows.Forms.GroupBox();
            this.GrpTestAsyncMethod = new System.Windows.Forms.GroupBox();
            this.GrpRequestOperation = new System.Windows.Forms.GroupBox();
            this.BtnRequestExit = new System.Windows.Forms.Button();
            this.BtnRequestCancel = new System.Windows.Forms.Button();
            this.BtnRequestException = new System.Windows.Forms.Button();
            this.BtnClearAll = new System.Windows.Forms.Button();
            this.BtnRunAll = new System.Windows.Forms.Button();
            this.LblWithResultWithArg = new System.Windows.Forms.Label();
            this.BtnWithResultWithArg = new System.Windows.Forms.Button();
            this.LblWithResultNoArg = new System.Windows.Forms.Label();
            this.BtnWithResultNoArg = new System.Windows.Forms.Button();
            this.LblNoResultWithArg = new System.Windows.Forms.Label();
            this.BtnNoResultWithArg = new System.Windows.Forms.Button();
            this.LblNoResultNoArg = new System.Windows.Forms.Label();
            this.BtnNoResultNoArg = new System.Windows.Forms.Button();
            this.TmrRefresh = new System.Windows.Forms.Timer(this.components);
            this.StatusStrip.SuspendLayout();
            this.GrpTaskExceptionTest.SuspendLayout();
            this.GrpTestAsyncMethod.SuspendLayout();
            this.GrpRequestOperation.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnTaskExceptionTest1
            // 
            this.BtnTaskExceptionTest1.Location = new System.Drawing.Point(8, 24);
            this.BtnTaskExceptionTest1.Name = "BtnTaskExceptionTest1";
            this.BtnTaskExceptionTest1.Size = new System.Drawing.Size(184, 24);
            this.BtnTaskExceptionTest1.TabIndex = 0;
            this.BtnTaskExceptionTest1.Text = "①待機なし(バックグラウンド動作)";
            this.BtnTaskExceptionTest1.UseVisualStyleBackColor = true;
            this.BtnTaskExceptionTest1.Click += new System.EventHandler(this.BtnTaskExceptionTest1_Click);
            // 
            // BtnTaskExceptionTest2
            // 
            this.BtnTaskExceptionTest2.Location = new System.Drawing.Point(8, 56);
            this.BtnTaskExceptionTest2.Name = "BtnTaskExceptionTest2";
            this.BtnTaskExceptionTest2.Size = new System.Drawing.Size(184, 24);
            this.BtnTaskExceptionTest2.TabIndex = 2;
            this.BtnTaskExceptionTest2.Text = "②中断なし、同期的に待機";
            this.BtnTaskExceptionTest2.UseVisualStyleBackColor = true;
            this.BtnTaskExceptionTest2.Click += new System.EventHandler(this.BtnTaskExceptionTest2_Click);
            // 
            // BtnTaskExceptionTest3
            // 
            this.BtnTaskExceptionTest3.Location = new System.Drawing.Point(8, 88);
            this.BtnTaskExceptionTest3.Name = "BtnTaskExceptionTest3";
            this.BtnTaskExceptionTest3.Size = new System.Drawing.Size(184, 24);
            this.BtnTaskExceptionTest3.TabIndex = 4;
            this.BtnTaskExceptionTest3.Text = "③中断なし、非同期で待機";
            this.BtnTaskExceptionTest3.UseVisualStyleBackColor = true;
            this.BtnTaskExceptionTest3.Click += new System.EventHandler(this.BtnTaskExceptionTest3_Click);
            // 
            // BtnTaskExceptionTest4
            // 
            this.BtnTaskExceptionTest4.Location = new System.Drawing.Point(8, 120);
            this.BtnTaskExceptionTest4.Name = "BtnTaskExceptionTest4";
            this.BtnTaskExceptionTest4.Size = new System.Drawing.Size(184, 24);
            this.BtnTaskExceptionTest4.TabIndex = 6;
            this.BtnTaskExceptionTest4.Text = "④中断あり、同期的に待機";
            this.BtnTaskExceptionTest4.UseVisualStyleBackColor = true;
            this.BtnTaskExceptionTest4.Click += new System.EventHandler(this.BtnTaskExceptionTest4_Click);
            // 
            // BtnTaskExceptionTest5
            // 
            this.BtnTaskExceptionTest5.Location = new System.Drawing.Point(8, 152);
            this.BtnTaskExceptionTest5.Name = "BtnTaskExceptionTest5";
            this.BtnTaskExceptionTest5.Size = new System.Drawing.Size(184, 24);
            this.BtnTaskExceptionTest5.TabIndex = 8;
            this.BtnTaskExceptionTest5.Text = "⑤中断あり、非同期で待機";
            this.BtnTaskExceptionTest5.UseVisualStyleBackColor = true;
            this.BtnTaskExceptionTest5.Click += new System.EventHandler(this.BtnTaskExceptionTest5_Click);
            // 
            // TmrCheckTask
            // 
            this.TmrCheckTask.Enabled = true;
            this.TmrCheckTask.Interval = 1000;
            this.TmrCheckTask.Tick += new System.EventHandler(this.TmrCheckTask_Tick);
            // 
            // LstMessage
            // 
            this.LstMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LstMessage.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LstMessage.FormattingEnabled = true;
            this.LstMessage.ItemHeight = 12;
            this.LstMessage.Location = new System.Drawing.Point(16, 400);
            this.LstMessage.Name = "LstMessage";
            this.LstMessage.Size = new System.Drawing.Size(624, 112);
            this.LstMessage.TabIndex = 2;
            // 
            // LblTest1
            // 
            this.LblTest1.Location = new System.Drawing.Point(200, 24);
            this.LblTest1.Name = "LblTest1";
            this.LblTest1.Size = new System.Drawing.Size(432, 23);
            this.LblTest1.TabIndex = 1;
            this.LblTest1.Text = "待機ロジックなし。例外が発生しても中断されない";
            this.LblTest1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblTest2
            // 
            this.LblTest2.Location = new System.Drawing.Point(200, 56);
            this.LblTest2.Name = "LblTest2";
            this.LblTest2.Size = new System.Drawing.Size(432, 23);
            this.LblTest2.TabIndex = 3;
            this.LblTest2.Text = "mytask.RunSynchronously で実行";
            this.LblTest2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblTest3
            // 
            this.LblTest3.Location = new System.Drawing.Point(200, 88);
            this.LblTest3.Name = "LblTest3";
            this.LblTest3.Size = new System.Drawing.Size(432, 23);
            this.LblTest3.TabIndex = 5;
            this.LblTest3.Text = "mytask.IsCompleted をチェックしながら Task.Delay で非同期的に待機";
            this.LblTest3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblTest4
            // 
            this.LblTest4.Location = new System.Drawing.Point(200, 120);
            this.LblTest4.Name = "LblTest4";
            this.LblTest4.Size = new System.Drawing.Size(432, 23);
            this.LblTest4.TabIndex = 7;
            this.LblTest4.Text = "mytask.Wait で待機する。中断場所は mytask.Wait の場所になる";
            this.LblTest4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(200, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(432, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "await mytask で待機する。中断場所は Program.cs の Applicaton.Run の場所になる";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1});
            this.StatusStrip.Location = new System.Drawing.Point(0, 523);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(657, 22);
            this.StatusStrip.TabIndex = 3;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.MarqueeAnimationSpeed = 20;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // GrpTaskExceptionTest
            // 
            this.GrpTaskExceptionTest.Controls.Add(this.BtnTaskExceptionTest1);
            this.GrpTaskExceptionTest.Controls.Add(this.BtnTaskExceptionTest2);
            this.GrpTaskExceptionTest.Controls.Add(this.label1);
            this.GrpTaskExceptionTest.Controls.Add(this.BtnTaskExceptionTest3);
            this.GrpTaskExceptionTest.Controls.Add(this.LblTest4);
            this.GrpTaskExceptionTest.Controls.Add(this.BtnTaskExceptionTest4);
            this.GrpTaskExceptionTest.Controls.Add(this.LblTest3);
            this.GrpTaskExceptionTest.Controls.Add(this.BtnTaskExceptionTest5);
            this.GrpTaskExceptionTest.Controls.Add(this.LblTest2);
            this.GrpTaskExceptionTest.Controls.Add(this.LblTest1);
            this.GrpTaskExceptionTest.Location = new System.Drawing.Point(8, 208);
            this.GrpTaskExceptionTest.Name = "GrpTaskExceptionTest";
            this.GrpTaskExceptionTest.Size = new System.Drawing.Size(640, 184);
            this.GrpTaskExceptionTest.TabIndex = 1;
            this.GrpTaskExceptionTest.TabStop = false;
            this.GrpTaskExceptionTest.Text = "(参考) Task クラスを用いた場合の、未ハンドル例外発生時の挙動検証テスト";
            // 
            // GrpTestAsyncMethod
            // 
            this.GrpTestAsyncMethod.Controls.Add(this.GrpRequestOperation);
            this.GrpTestAsyncMethod.Controls.Add(this.BtnClearAll);
            this.GrpTestAsyncMethod.Controls.Add(this.BtnRunAll);
            this.GrpTestAsyncMethod.Controls.Add(this.LblWithResultWithArg);
            this.GrpTestAsyncMethod.Controls.Add(this.BtnWithResultWithArg);
            this.GrpTestAsyncMethod.Controls.Add(this.LblWithResultNoArg);
            this.GrpTestAsyncMethod.Controls.Add(this.BtnWithResultNoArg);
            this.GrpTestAsyncMethod.Controls.Add(this.LblNoResultWithArg);
            this.GrpTestAsyncMethod.Controls.Add(this.BtnNoResultWithArg);
            this.GrpTestAsyncMethod.Controls.Add(this.LblNoResultNoArg);
            this.GrpTestAsyncMethod.Controls.Add(this.BtnNoResultNoArg);
            this.GrpTestAsyncMethod.Location = new System.Drawing.Point(8, 8);
            this.GrpTestAsyncMethod.Name = "GrpTestAsyncMethod";
            this.GrpTestAsyncMethod.Size = new System.Drawing.Size(640, 184);
            this.GrpTestAsyncMethod.TabIndex = 0;
            this.GrpTestAsyncMethod.TabStop = false;
            this.GrpTestAsyncMethod.Text = "AsyncMethod クラスのテスト：デバッグ版の場合、未ハンドル例外発生時はその場所で即座に中断";
            // 
            // GrpRequestOperation
            // 
            this.GrpRequestOperation.Controls.Add(this.BtnRequestExit);
            this.GrpRequestOperation.Controls.Add(this.BtnRequestCancel);
            this.GrpRequestOperation.Controls.Add(this.BtnRequestException);
            this.GrpRequestOperation.Location = new System.Drawing.Point(472, 24);
            this.GrpRequestOperation.Name = "GrpRequestOperation";
            this.GrpRequestOperation.Size = new System.Drawing.Size(160, 128);
            this.GrpRequestOperation.TabIndex = 10;
            this.GrpRequestOperation.TabStop = false;
            this.GrpRequestOperation.Text = "「引数あり」タイプのみ有効";
            // 
            // BtnRequestExit
            // 
            this.BtnRequestExit.Location = new System.Drawing.Point(16, 24);
            this.BtnRequestExit.Name = "BtnRequestExit";
            this.BtnRequestExit.Size = new System.Drawing.Size(128, 24);
            this.BtnRequestExit.TabIndex = 0;
            this.BtnRequestExit.Text = "終了要求する";
            this.BtnRequestExit.UseVisualStyleBackColor = true;
            this.BtnRequestExit.Click += new System.EventHandler(this.BtnRequestExit_Click);
            // 
            // BtnRequestCancel
            // 
            this.BtnRequestCancel.Location = new System.Drawing.Point(16, 56);
            this.BtnRequestCancel.Name = "BtnRequestCancel";
            this.BtnRequestCancel.Size = new System.Drawing.Size(128, 24);
            this.BtnRequestCancel.TabIndex = 1;
            this.BtnRequestCancel.Text = "キャンセル要求する";
            this.BtnRequestCancel.UseVisualStyleBackColor = true;
            this.BtnRequestCancel.Click += new System.EventHandler(this.BtnRequestCancel_Click);
            // 
            // BtnRequestException
            // 
            this.BtnRequestException.Location = new System.Drawing.Point(16, 88);
            this.BtnRequestException.Name = "BtnRequestException";
            this.BtnRequestException.Size = new System.Drawing.Size(128, 24);
            this.BtnRequestException.TabIndex = 2;
            this.BtnRequestException.Text = "例外要求する";
            this.BtnRequestException.UseVisualStyleBackColor = true;
            this.BtnRequestException.Click += new System.EventHandler(this.BtnRequestException_Click);
            // 
            // BtnClearAll
            // 
            this.BtnClearAll.Location = new System.Drawing.Point(192, 152);
            this.BtnClearAll.Name = "BtnClearAll";
            this.BtnClearAll.Size = new System.Drawing.Size(168, 24);
            this.BtnClearAll.TabIndex = 9;
            this.BtnClearAll.Text = "↑クリア";
            this.BtnClearAll.UseVisualStyleBackColor = true;
            this.BtnClearAll.Click += new System.EventHandler(this.BtnClearAll_Click);
            // 
            // BtnRunAll
            // 
            this.BtnRunAll.Location = new System.Drawing.Point(16, 152);
            this.BtnRunAll.Name = "BtnRunAll";
            this.BtnRunAll.Size = new System.Drawing.Size(168, 24);
            this.BtnRunAll.TabIndex = 8;
            this.BtnRunAll.Text = "↑一斉に実行";
            this.BtnRunAll.UseVisualStyleBackColor = true;
            this.BtnRunAll.Click += new System.EventHandler(this.BtnRunAll_Click);
            // 
            // LblWithResultWithArg
            // 
            this.LblWithResultWithArg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblWithResultWithArg.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LblWithResultWithArg.Location = new System.Drawing.Point(192, 120);
            this.LblWithResultWithArg.Name = "LblWithResultWithArg";
            this.LblWithResultWithArg.Size = new System.Drawing.Size(264, 23);
            this.LblWithResultWithArg.TabIndex = 7;
            this.LblWithResultWithArg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnWithResultWithArg
            // 
            this.BtnWithResultWithArg.Location = new System.Drawing.Point(16, 120);
            this.BtnWithResultWithArg.Name = "BtnWithResultWithArg";
            this.BtnWithResultWithArg.Size = new System.Drawing.Size(168, 24);
            this.BtnWithResultWithArg.TabIndex = 6;
            this.BtnWithResultWithArg.Text = "戻り値あり、引数あり";
            this.BtnWithResultWithArg.UseVisualStyleBackColor = true;
            this.BtnWithResultWithArg.Click += new System.EventHandler(this.BtnWithResultWithArg_Click);
            // 
            // LblWithResultNoArg
            // 
            this.LblWithResultNoArg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblWithResultNoArg.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LblWithResultNoArg.Location = new System.Drawing.Point(192, 88);
            this.LblWithResultNoArg.Name = "LblWithResultNoArg";
            this.LblWithResultNoArg.Size = new System.Drawing.Size(264, 23);
            this.LblWithResultNoArg.TabIndex = 5;
            this.LblWithResultNoArg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnWithResultNoArg
            // 
            this.BtnWithResultNoArg.Location = new System.Drawing.Point(16, 88);
            this.BtnWithResultNoArg.Name = "BtnWithResultNoArg";
            this.BtnWithResultNoArg.Size = new System.Drawing.Size(168, 24);
            this.BtnWithResultNoArg.TabIndex = 4;
            this.BtnWithResultNoArg.Text = "戻り値あり、引数なし";
            this.BtnWithResultNoArg.UseVisualStyleBackColor = true;
            this.BtnWithResultNoArg.Click += new System.EventHandler(this.BtnWithResultNoArg_Click);
            // 
            // LblNoResultWithArg
            // 
            this.LblNoResultWithArg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblNoResultWithArg.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LblNoResultWithArg.Location = new System.Drawing.Point(192, 56);
            this.LblNoResultWithArg.Name = "LblNoResultWithArg";
            this.LblNoResultWithArg.Size = new System.Drawing.Size(264, 23);
            this.LblNoResultWithArg.TabIndex = 3;
            this.LblNoResultWithArg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnNoResultWithArg
            // 
            this.BtnNoResultWithArg.Location = new System.Drawing.Point(16, 56);
            this.BtnNoResultWithArg.Name = "BtnNoResultWithArg";
            this.BtnNoResultWithArg.Size = new System.Drawing.Size(168, 24);
            this.BtnNoResultWithArg.TabIndex = 2;
            this.BtnNoResultWithArg.Text = "戻り値なし、引数あり";
            this.BtnNoResultWithArg.UseVisualStyleBackColor = true;
            this.BtnNoResultWithArg.Click += new System.EventHandler(this.BtnNoResultWithArg_Click);
            // 
            // LblNoResultNoArg
            // 
            this.LblNoResultNoArg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblNoResultNoArg.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LblNoResultNoArg.Location = new System.Drawing.Point(192, 24);
            this.LblNoResultNoArg.Name = "LblNoResultNoArg";
            this.LblNoResultNoArg.Size = new System.Drawing.Size(264, 23);
            this.LblNoResultNoArg.TabIndex = 1;
            this.LblNoResultNoArg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnNoResultNoArg
            // 
            this.BtnNoResultNoArg.Location = new System.Drawing.Point(16, 24);
            this.BtnNoResultNoArg.Name = "BtnNoResultNoArg";
            this.BtnNoResultNoArg.Size = new System.Drawing.Size(168, 24);
            this.BtnNoResultNoArg.TabIndex = 0;
            this.BtnNoResultNoArg.Text = "戻り値なし、引数なし";
            this.BtnNoResultNoArg.UseVisualStyleBackColor = true;
            this.BtnNoResultNoArg.Click += new System.EventHandler(this.BtnNoResultNoArg_Click);
            // 
            // TmrRefresh
            // 
            this.TmrRefresh.Enabled = true;
            this.TmrRefresh.Interval = 50;
            this.TmrRefresh.Tick += new System.EventHandler(this.TmrRefresh_Tick);
            // 
            // FrmTestAsyncMethod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 545);
            this.Controls.Add(this.GrpTestAsyncMethod);
            this.Controls.Add(this.GrpTaskExceptionTest);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.LstMessage);
            this.Name = "FrmTestAsyncMethod";
            this.Text = "AsyncMethod クラスのテスト";
            this.Load += new System.EventHandler(this.FrmTestAsyncMethod_Load);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.GrpTaskExceptionTest.ResumeLayout(false);
            this.GrpTestAsyncMethod.ResumeLayout(false);
            this.GrpRequestOperation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnTaskExceptionTest1;
        private System.Windows.Forms.Button BtnTaskExceptionTest2;
        private System.Windows.Forms.Button BtnTaskExceptionTest3;
        private System.Windows.Forms.Button BtnTaskExceptionTest4;
        private System.Windows.Forms.Button BtnTaskExceptionTest5;
        private System.Windows.Forms.Timer TmrCheckTask;
        private System.Windows.Forms.ListBox LstMessage;
        private System.Windows.Forms.Label LblTest1;
        private System.Windows.Forms.Label LblTest2;
        private System.Windows.Forms.Label LblTest3;
        private System.Windows.Forms.Label LblTest4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.GroupBox GrpTaskExceptionTest;
        private System.Windows.Forms.GroupBox GrpTestAsyncMethod;
        private System.Windows.Forms.Label LblNoResultNoArg;
        private System.Windows.Forms.Button BtnNoResultNoArg;
        private System.Windows.Forms.Label LblWithResultWithArg;
        private System.Windows.Forms.Button BtnWithResultWithArg;
        private System.Windows.Forms.Label LblWithResultNoArg;
        private System.Windows.Forms.Button BtnWithResultNoArg;
        private System.Windows.Forms.Label LblNoResultWithArg;
        private System.Windows.Forms.Button BtnNoResultWithArg;
        private System.Windows.Forms.Button BtnRequestExit;
        private System.Windows.Forms.Timer TmrRefresh;
        private System.Windows.Forms.Button BtnRequestException;
        private System.Windows.Forms.Button BtnRequestCancel;
        private System.Windows.Forms.Button BtnRunAll;
        private System.Windows.Forms.Button BtnClearAll;
        private System.Windows.Forms.GroupBox GrpRequestOperation;
    }
}