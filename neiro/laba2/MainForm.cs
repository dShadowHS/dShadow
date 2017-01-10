using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace Perceptron_Duo
{
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox alphaBox;
		private System.Windows.Forms.TextBox errorLimitBox;
		private System.Windows.Forms.TextBox txb_ErrLearn;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txb_iterCnt;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button btnLearn;
		private System.ComponentModel.Container components = null;
        private Button btn_Exit;
        private Label label3;
        private TextBox txb_IterNum;

        private const int MAX_CLS = 10;
        private const int MIN_DIM = 2;
        private const int MAX_DIM = 10;
        private const int MAX_ITER_NUM = 25000;

        private int Dim = 2;
        private int ipoint;
        private int ClsNum = 2;
        private int HideLayerSize;
        private double[,] srcCoord;
        private int[] srcClass;
        private double[,] tstCoord;
        private int[] tstClass;
        private DataPoint[] dpoints;
        private DataPoint[] tstpoints;
        double[][] input;
        double[][] output;
        int[] tstrez;

        private int pointsCnt = 0;
        private int iterCnt = 0;
        private int iterNum = MAX_ITER_NUM;
        private double		learningRate = 0.5;
		private double		sigmoidAlphaValue = 2.0;
		private double		learningErrorLimit = 0.1;
        private bool        needToStop = false;

        ActivationNetwork network;
        ActivationLayer layer;

        private Button btnLoad;
        private Button btnTest;
        private ListBox lst_Data;
        private ListBox lst_TestData;
        private ListBox lst_Rez;
        private OpenFileDialog openDataFileDlg;
        private Label label5;
        private Label label6;
        private Label label4;
        private Label label7;
        private Thread	workerThread = null;

		public MainForm( )
		{
			InitializeComponent();

			UpdateSettings();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txb_IterNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txb_ErrLearn = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txb_iterCnt = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.errorLimitBox = new System.Windows.Forms.TextBox();
            this.alphaBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.learningRateBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.btnLearn = new System.Windows.Forms.Button();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.lst_Data = new System.Windows.Forms.ListBox();
            this.lst_TestData = new System.Windows.Forms.ListBox();
            this.lst_Rez = new System.Windows.Forms.ListBox();
            this.openDataFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txb_IterNum
            // 
            resources.ApplyResources(this.txb_IterNum, "txb_IterNum");
            this.txb_IterNum.Name = "txb_IterNum";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txb_ErrLearn
            // 
            resources.ApplyResources(this.txb_ErrLearn, "txb_ErrLearn");
            this.txb_ErrLearn.Name = "txb_ErrLearn";
            this.txb_ErrLearn.ReadOnly = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // txb_iterCnt
            // 
            resources.ApplyResources(this.txb_iterCnt, "txb_iterCnt");
            this.txb_iterCnt.Name = "txb_iterCnt";
            this.txb_iterCnt.ReadOnly = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // errorLimitBox
            // 
            resources.ApplyResources(this.errorLimitBox, "errorLimitBox");
            this.errorLimitBox.Name = "errorLimitBox";
            // 
            // alphaBox
            // 
            resources.ApplyResources(this.alphaBox, "alphaBox");
            this.alphaBox.Name = "alphaBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // learningRateBox
            // 
            resources.ApplyResources(this.learningRateBox, "learningRateBox");
            this.learningRateBox.Name = "learningRateBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Name = "label1";
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.Name = "stopButton";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // btnLearn
            // 
            resources.ApplyResources(this.btnLearn, "btnLearn");
            this.btnLearn.Name = "btnLearn";
            this.btnLearn.Click += new System.EventHandler(this.btnLearn_Click);
            // 
            // btn_Exit
            // 
            resources.ApplyResources(this.btn_Exit, "btn_Exit");
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lst_Data
            // 
            resources.ApplyResources(this.lst_Data, "lst_Data");
            this.lst_Data.FormattingEnabled = true;
            this.lst_Data.Name = "lst_Data";
            // 
            // lst_TestData
            // 
            resources.ApplyResources(this.lst_TestData, "lst_TestData");
            this.lst_TestData.FormattingEnabled = true;
            this.lst_TestData.Name = "lst_TestData";
            // 
            // lst_Rez
            // 
            resources.ApplyResources(this.lst_Rez, "lst_Rez");
            this.lst_Rez.FormattingEnabled = true;
            this.lst_Rez.Name = "lst_Rez";
            // 
            // openDataFileDlg
            // 
            resources.ApplyResources(this.openDataFileDlg, "openDataFileDlg");
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txb_IterNum);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lst_Rez);
            this.Controls.Add(this.txb_ErrLearn);
            this.Controls.Add(this.lst_TestData);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lst_Data);
            this.Controls.Add(this.txb_iterCnt);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.errorLimitBox);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.alphaBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLearn);
            this.Controls.Add(this.learningRateBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		[STAThread]
		static void Main( ) 
		{
			Application.Run( new MainForm( ) );
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if ( ( workerThread != null ) && ( workerThread.IsAlive ) )
			{
				needToStop = true;
				workerThread.Join( );
			}
		}


        private void NormalizeData(DataPoint[] points)
        {
            double[] minVals = new double[Dim];
            double[] maxVals = new double[Dim];

            for (int j = 0; j < Dim; j++)
            {
                minVals[j] = double.MaxValue;               //  a |-----------| b     ==>   0 |---------| 1
                maxVals[j] = double.MinValue;               //          x                          x1
            }                                       
                                                            //            x1 = (x - a) / (b - a)
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < Dim; j++)
                {
                    if (minVals[j] > points[i][j]) minVals[j] = points[i][j];
                    if (maxVals[j] < points[i][j]) maxVals[j] = points[i][j];
                }
            }

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < Dim; j++)
                {
                    points[i][j] = (points[i][j] - minVals[j]) / (maxVals[j] - minVals[j]); // - 0.5;
                }
            }
        }

        private void Initialize()
        {
            HideLayerSize = (int)Math.Sqrt(Dim*ClsNum);
            NormalizeData(dpoints);

            input = new double[dpoints.Length][];
            output = new double[dpoints.Length][];

            for (int i = 0; i < dpoints.Length; i++)
            {
                input[i] = new double[Dim];
                output[i] = new double[ClsNum];

                for (int j = 0; j < Dim; j++)
                {
                    input[i][j] = dpoints[i][j];
                }
                output[i][dpoints[i].Cls] = 1;
            }
        }

        private void ReadData(string filename)
        {
            // Формат строки файла данных: x0, x1,...,xi,... , class
            // где xi - координаты точки (0 <= i <= MAX_DIM), class - номер её класса (0,..., MAX_CLS)  

            string line;
            using (StreamReader sr = new StreamReader(filename))
            {
                try
                {
                    string[] DataStr;

                    // Определяем число строк файла
                    int strNum = System.IO.File.ReadAllLines(filename).Length;

                    line = sr.ReadLine();
                    DataStr = line.Split(';');
                    Dim = DataStr.Length - 1;

                    srcCoord = new double[strNum, Dim];
                    srcClass = new int[strNum];

                    ipoint = 0;               
                    ClsNum = 0;

                    do
                    {
                        DataStr = line.Split(';');
                        if ((DataStr.Length < MIN_DIM + 1) || (DataStr.Length > MAX_DIM + 1))
                            throw new ApplicationException("Ошибка");

                        for (int j = 0; j < Dim; j++)
                        {
                            srcCoord[ipoint, j] = double.Parse(DataStr[j]);
                        }
                        srcClass[ipoint] = int.Parse(DataStr[Dim]);

                        if (srcClass[ipoint] >= MAX_CLS)
                            continue;

                        if (srcClass[ipoint] >= ClsNum)
                            ClsNum = srcClass[ipoint] + 1;

                        ipoint++;

                    } while ((ipoint < strNum) && ((line = sr.ReadLine()) != null));
                }
                catch (ApplicationException)
                {
                    MessageBox.Show("Ошибка формата строки файла", "ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                catch (Exception)
                {
                    MessageBox.Show("Ошибка чтения файла", "ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    if (sr != null)
                        sr.Close();
                }
            }

            dpoints = new DataPoint[ipoint];
            lst_Data.Items.Clear();

            for (int i = 0; i < ipoint; i++)
            {
                DataPoint dp = new DataPoint(Dim, srcClass[i]);

                for (int j = 0; j < Dim; j++)
                {
                    dp[j] = srcCoord[i, j];
                }

                dpoints[i] = dp;
                lst_Data.Items.Add(dp.ToString());
            }
        }

        private void ReadTestData(string filename)
        {
            // Формат строки файла данных: x0, x1,...,xi,... , class
            // где xi - координаты точки (0 <= i <= MAX_DIM), class - номер её класса (0,..., MAX_CLS)  

            if (!System.IO.File.Exists(filename))
            {
                MessageBox.Show("Не найден файл тестовых данных: " + filename, "ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string line;
                using (StreamReader sr = new StreamReader(filename))
                {
                    try
                    {
                        string[] DataStr;

                        // Определяем число строк файла
                        int strNum = System.IO.File.ReadAllLines(filename).Length;

                        line = sr.ReadLine();
                        DataStr = line.Split(';');
                        Dim = DataStr.Length - 1;

                        tstCoord = new double[strNum, Dim];
                        tstClass = new int[strNum];

                        ipoint = 0;               
                        ClsNum = 0;

                        do
                        {
                            DataStr = line.Split(';');
                            if ((DataStr.Length < MIN_DIM + 1) || (DataStr.Length > MAX_DIM + 1))
                                throw new ApplicationException("Ошибка");

                            for (int j = 0; j < Dim; j++)
                            {
                                tstCoord[ipoint, j] = double.Parse(DataStr[j]);
                            }
                            tstClass[ipoint] = int.Parse(DataStr[Dim]);

                            if (tstClass[ipoint] >= MAX_CLS)
                                continue;

                            if (tstClass[ipoint] >= ClsNum)
                                ClsNum = tstClass[ipoint] + 1;

                            ipoint++;

                        } while ((ipoint < strNum) && ((line = sr.ReadLine()) != null));
                    }
                    catch (ApplicationException)
                    {
                        MessageBox.Show("Ошибка формата строки файла", "ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка чтения файла", "ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    finally
                    {
                        if (sr != null)
                            sr.Close();
                    }
                }

                tstpoints = new DataPoint[ipoint];
                lst_TestData.Items.Clear();

                for (int i = 0; i < ipoint; i++)
                {
                    DataPoint dp = new DataPoint(Dim, tstClass[i]);

                    for (int j = 0; j < Dim; j++)
                    {
                        dp[j] = tstCoord[i, j];
                    }

                    tstpoints[i] = dp;
                    lst_TestData.Items.Add(dp.ToString());
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DialogResult Res = openDataFileDlg.ShowDialog();
            if (Res == DialogResult.OK)
            {
                string filename = openDataFileDlg.FileName;
                ReadData(filename);
            }
            string FNameTstData = openDataFileDlg.FileName;
            FNameTstData = FNameTstData.TrimEnd(new char[3] { 'c', 's', 'v' });
            FNameTstData = FNameTstData.Insert(FNameTstData.Length, "tst");
            ReadTestData(FNameTstData);

            btnLearn.Enabled = true;
            btnTest.Enabled = false;
            txb_iterCnt.Text = "0";
            txb_ErrLearn.Text = "0";
            lst_Rez.Items.Clear();
            lst_Rez.Update();
        }

        private int IndMaxRez(double[] rez)
        {
            double maxrez = double.MinValue;
            int imaxrez = 0;
            for (int i = 0; i < rez.Length; i++)
            {
                if (rez[i] > maxrez)
                {
                    maxrez = rez[i];
                    imaxrez = i;
                }
            }
            return imaxrez;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            NormalizeData(tstpoints);

            input = new double[tstpoints.Length][];
            output = new double[tstpoints.Length][];
            tstrez = new int [tstpoints.Length];

            for (int i = 0; i < tstpoints.Length; i++)
            {
                input[i] = new double[Dim];
                output[i] = new double[ClsNum];
                for (int j = 0; j < Dim; j++)
                {
                    input[i][j] = tstpoints[i][j];
                }
                output[i][dpoints[i].Cls] = 0.0;
            }

            for (pointsCnt = 0; pointsCnt < tstpoints.Length; pointsCnt++)
            {
                output[pointsCnt] = network.Compute(input[pointsCnt]);
                tstrez[pointsCnt] = IndMaxRez(output[pointsCnt]); 
            }

            lst_Rez.Items.Add("Test results (normalized data): ");

            for (pointsCnt = 0; pointsCnt < tstpoints.Length; pointsCnt++)
            {
                string sb = "";
                for (int j = 0; j < Dim; j++)
                {
                    sb += String.Format("{0:0.000}", tstpoints[pointsCnt][j]) + ";";
                }
                sb += tstrez[pointsCnt];
                lst_Rez.Items.Add(sb);
            }
            int diffcnt = 0;
            for (pointsCnt = 0; pointsCnt < tstpoints.Length; pointsCnt++)
            {
                if (tstpoints[pointsCnt].Cls != tstrez[pointsCnt]) diffcnt++;
            }
            lst_Rez.Items.Add("");
            lst_Rez.Items.Add("% errors: " + String.Format("{0:0.00}", (double)diffcnt/tstpoints.Length*100.0));
        }

        private void UpdateSettings( )
		{
			learningRateBox.Text	= learningRate.ToString( );
			alphaBox.Text			= sigmoidAlphaValue.ToString( );
			errorLimitBox.Text		= learningErrorLimit.ToString( );
            txb_IterNum.Text        = iterNum.ToString();
        }

		private void EnableControls( bool enable )
		{
			learningRateBox.Enabled		= enable;
			alphaBox.Enabled			= enable;
			errorLimitBox.Enabled		= enable;
			btnLearn.Enabled			= enable;
			stopButton.Enabled			= !enable;
		}

		private void btnLearn_Click(object sender, System.EventArgs e)
		{
			try
			{
				learningRate = Math.Max( 0.00001, Math.Min( 1, double.Parse( learningRateBox.Text ) ) );
			}
			catch
			{
				learningRate = 0.1;
			}
			try
			{
				sigmoidAlphaValue = Math.Max( 0.01, Math.Min( 100, double.Parse( alphaBox.Text ) ) );
			}
			catch
			{
				sigmoidAlphaValue = 2;
			}
			try
			{
				learningErrorLimit = Math.Max( 0, double.Parse( errorLimitBox.Text ) );
			}
			catch
			{
				learningErrorLimit = 0.1;
			}
            iterNum = int.Parse(txb_IterNum.Text);
            UpdateSettings( );

			EnableControls( false );
            btnTest.Enabled = true;

            needToStop = false;
            workerThread = new Thread(new ThreadStart(SearchSolution));
            workerThread.Start();
        }

		private void stopButton_Click(object sender, System.EventArgs e)
		{
			needToStop = true;
			workerThread.Join( );
			workerThread = null;
		}

        private void ShowRezult()
        {
            lst_Rez.Items.Clear();

            lst_Rez.Items.Add("Hidden layer: ");
            layer = network[0];
            for (int i = 0; i < layer.NeuronsCount; i++)
            {
                lst_Rez.Items.Add("Neuron " + (i+1));
                for (int j = 0; j < layer.InputsCount; j++)
                {
                    lst_Rez.Items.Add("w[" + j + "] = " + Math.Round(layer[i][j], 2).ToString());
                }
                lst_Rez.Items.Add("T = " + layer[i].Threshold.ToString());
            }
            lst_Rez.Items.Add("\n\r");

            lst_Rez.Items.Add("Second layer: ");
            layer = network[1];
            for (int i = 0; i < layer.NeuronsCount; i++)
            {
                lst_Rez.Items.Add("Neuron " + (i + 1));
                for (int j = 0; j < layer.InputsCount; j++)
                {
                    lst_Rez.Items.Add("w[" + j + "] = " + Math.Round(layer[i][j], 2).ToString());
                }
                lst_Rez.Items.Add("T = " + layer[i].Threshold.ToString());
            }
            lst_Rez.Items.Add("\n\r");
        }

        void SearchSolution()
        {
            Initialize();

            network = new ActivationNetwork(
                    (IActivationFunction)new SigmoidFunction(sigmoidAlphaValue), Dim, HideLayerSize, ClsNum);
            
            BackPropagationLearning teacher = new BackPropagationLearning(network);
            
            teacher.LearningRate = learningRate;

            iterCnt = 1;

            while (!needToStop)
            {
                double error = teacher.RunEpoch(input, output);

                txb_iterCnt.Text = iterCnt.ToString();
                txb_ErrLearn.Text = error.ToString();

                iterCnt++;
                if ((iterCnt > iterNum) || (error <= learningErrorLimit)) needToStop = true;
            }

            EnableControls(true);
            ShowRezult();
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
