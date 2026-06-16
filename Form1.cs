using Newtonsoft.Json;
using SimpleNN.NeuralNetwork;
using SimpleNN.Trainer;

namespace SimpleNN
{
    public partial class Form1 : Form
    {
        private LoadData TrainingData = null;
        private LoadData TestingData = null;
        private MainTrainer trainer = null;

        // Percorsi dei file scelti dall'utente (default = impostazioni dell'app).
        private string trainFilePath = "";
        private string testFilePath = "";
        private string singleFilePath = "";

        private readonly ToolTip toolTip = new ToolTip();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trainFilePath = Properties.Settings.Default.TrainingFile;
            testFilePath = Properties.Settings.Default.TestFile;
            singleFilePath = Properties.Settings.Default.SingleTestFile;

            lblTraningFile.Text = trainFilePath;
            lblTestFile.Text = testFilePath;
            lblSingleFile.Text = singleFilePath;

            // Spiegazione di ciascun file al passaggio del mouse.
            string trainTip = "Dati di ADDESTRAMENTO: CSV con molti record (feature + median_house_value). Usato dal pulsante Run per addestrare la rete.";
            string testTip = "Dati di TEST: CSV separato, mai visto in addestramento. Usato dal pulsante Test per misurare l'errore reale (RMSE in $).";
            string singleTip = "Record SINGOLO: CSV con una sola riga. Usato dal pulsante Single per predire quel valore e confrontarlo con quello reale.";

            toolTip.SetToolTip(lblTrainCaption, trainTip);
            toolTip.SetToolTip(lblTraningFile, trainTip);
            toolTip.SetToolTip(btnBrowseTrain, trainTip);
            toolTip.SetToolTip(lblTestCaption, testTip);
            toolTip.SetToolTip(lblTestFile, testTip);
            toolTip.SetToolTip(btnBrowseTest, testTip);
            toolTip.SetToolTip(lblSingleCaption, singleTip);
            toolTip.SetToolTip(lblSingleFile, singleTip);
            toolTip.SetToolTip(btnBrowseSingle, singleTip);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Apre un dialogo per scegliere un CSV; ritorna il percorso scelto oppure null.
        private string? BrowseCsv(string current)
        {
            using OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "File CSV (*.csv)|*.csv|Tutti i file (*.*)|*.*",
                CheckFileExists = true
            };
            if (!string.IsNullOrEmpty(current) && File.Exists(current))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(current);
                dlg.FileName = Path.GetFileName(current);
            }
            return dlg.ShowDialog(this) == DialogResult.OK ? dlg.FileName : null;
        }

        private void btnBrowseTrain_Click(object sender, EventArgs e)
        {
            string? chosen = BrowseCsv(trainFilePath);
            if (chosen == null) return;
            trainFilePath = chosen;
            lblTraningFile.Text = chosen;
        }

        private void btnBrowseTest_Click(object sender, EventArgs e)
        {
            string? chosen = BrowseCsv(testFilePath);
            if (chosen == null) return;
            testFilePath = chosen;
            lblTestFile.Text = chosen;
        }

        private void btnBrowseSingle_Click(object sender, EventArgs e)
        {
            string? chosen = BrowseCsv(singleFilePath);
            if (chosen == null) return;
            singleFilePath = chosen;
            lblSingleFile.Text = chosen;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                enableControls(false);
                DateTime start = DateTime.Now;

                // Parametri scelti dall'utente nella UI.
                int epochs = (int)numbxEpox.Value;
                int hiddenLayers = (int)numbxLayers.Value;
                int neurons = (int)numbxNeurons.Value;
                double learningRate = (double)numbxLearningRate.Value;

                // 1) TRAIN: carica + normalizza (calcola le statistiche sul training).
                LoadData train = new LoadData(trainFilePath, "median_house_value");
                train.FitNormalization();
                lstLog.Items.Insert(0, string.Format("Trovati {0} record in {1:F2}s",
                    train.data.Count, DateTime.Now.Subtract(start).TotalSeconds));

                // 2) Rete: hidden layer ReLU secondo la UI, 1 output lineare (impostato dentro Network).
                Network net = new Network(train.FeatureCount, hiddenLayers, neurons, ActivationFunction.ReLU, 1);
                net.LearningRate = learningRate;      // input e target sono normalizzati

                // 3) Training. Salvo trainer e dati di training nei campi per Test/Single.
                trainer = new MainTrainer(net, epochs);
                TrainingData = train;
                start = DateTime.Now;
                trainer.Fit(train.data, train.labels);
                lstLog.Items.Insert(0, string.Format("Training: {0} epoche in {1:F2}s",
                    trainer.epoche, DateTime.Now.Subtract(start).TotalSeconds));

                // 4) Mostra la curva della loss. Per valutare sul test set premere "Test".
                updateplot(trainer.lossLogs.Count);
            }
            catch (Exception ex)
            {
                lstLog.Items.Insert(0, ex.Message);
            }
            finally
            {
                enableControls(true);
            }
        }

        private void saveRun()
        {
            string fileName = string.Format("run_{0}_{1}_{2}.json", numbxEpox.Value, numbxLayers.Value, numbxNeurons.Value);
            string json = JsonConvert.SerializeObject(new { trainer.lossLogs });
            try
            {
                fileName = Path.Combine(Properties.Settings.Default.ResultsFolder, fileName);
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                File.WriteAllText(fileName, json);
            }
            catch (Exception e)
            {
                lstLog.Items.Add("Errore in fase salvataggio su disco: " + e.Message);
            }
        }

        private void updateplot(int currEpox)
        {
            grphErr.Plot.Clear();
            grphErr.Plot.XLabel("Numero epoche");
            grphErr.Plot.YLabel("Errore");
            double[] yvalues = new double[trainer.lossLogs.Count];
            yvalues = trainer.lossLogs.ToArray();
            double bestloss = 100000000;
            for (int i = 0; i < trainer.lossLogs.Count; i++)
            {
                if (trainer.lossLogs[i] < bestloss)
                    bestloss = trainer.lossLogs[i];
            }
            double[] xvalues = new double[currEpox];
            for (int i = 0; i < xvalues.Length; i++)
            {
                xvalues[i] = i + 1;
            }
            var legendloss = grphErr.Plot.Add.Scatter(xvalues, yvalues, color: ScottPlot.Color.FromColor(Color.Red));
            legendloss.LegendText = "Best loss: " + bestloss;
            //            var legendaccuracy = grphErr.Plot.Add.Scatter(xvalues, accuracyLogs.ToArray(), color: ScottPlot.Color.FromColor(Color.Green));
            grphErr.Plot.Axes.AutoScale();
            grphErr.Plot.ShowLegend(ScottPlot.Alignment.UpperLeft, ScottPlot.Orientation.Horizontal);
            grphErr.Refresh();
        }

        private void enableControls(bool enabled)
        {
            foreach (Control c in this.Controls)
            {
                c.Enabled = enabled;
            }
            if (enabled)
            {
                this.Cursor = Cursors.Default;
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;
            }
            Application.DoEvents();
        }

        private void numbxEpox_ValueChanged(object sender, EventArgs e)
        {
            string fileName = string.Format("run_{0}_{1}_{2}.json", numbxEpox.Value, numbxLayers.Value, numbxNeurons.Value);
            fileName = Path.Combine(Properties.Settings.Default.ResultsFolder, fileName);
            OldGraph(fileName);
        }

        private void OldGraph(string fileName, bool clearplot = true)
        {
            if (File.Exists(fileName))
            {
                string json = File.ReadAllText(fileName);
                // Deserializza json in una variabile dinamica
                dynamic deserializedData = JsonConvert.DeserializeObject<dynamic>(json);

                // Esempio: loggare i dati deserializzati
                if (deserializedData != null)
                {
                    if (clearplot)
                    {
                        grphErr.Plot.Clear();
                    }
                    grphErr.Plot.XLabel("Numero epoche");
                    grphErr.Plot.YLabel("Errore");
                    double[] yvalues = deserializedData.lossLogs.ToObject<double[]>();
                    double bestloss = 100000000;
                    for (int i = 0; i < yvalues.Length; i++)
                    {
                        if (yvalues[i] < bestloss)
                            bestloss = yvalues[i];
                    }
                    double[] xvalues = new double[Convert.ToInt32(numbxEpox.Value)];
                    for (int i = 0; i < xvalues.Length; i++)
                    {
                        xvalues[i] = i + 1;
                    }
                    FileInfo fi = new FileInfo(fileName);
                    var legendloss = grphErr.Plot.Add.Scatter(xvalues, yvalues, color: ScottPlot.Color.FromColor(Color.Green));
                    legendloss.LegendText = "Filename: " + fi.Name + " Best loss: " + bestloss;
                    grphErr.Plot.Axes.AutoScale();
                    grphErr.Plot.ShowLegend(ScottPlot.Alignment.UpperLeft, ScottPlot.Orientation.Horizontal);
                    grphErr.Refresh();
                }
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            if (trainer == null || TrainingData == null)
            {
                lstLog.Items.Insert(0, "Nessun modello da testare");
                return;
            }
            // Normalizza le feature con le STATISTICHE DEL TRAIN, poi RMSE in dollari.
            TestingData = new LoadData(testFilePath, "median_house_value");
            TestingData.NormalizeFeaturesWith(TrainingData.FeatureMean, TrainingData.FeatureStd);
            double rmse = trainer.EvaluateRmse(TestingData.data, TestingData.labels,
                                               TrainingData.LabelMean, TrainingData.LabelStd);
            grphErr.Plot.Add.HorizontalLine(rmse, color: ScottPlot.Color.FromColor(Color.Black));
            grphErr.Refresh();
            lstLog.Items.Insert(0, string.Format("TEST RMSE: ${0:N0} in {1:F2}s",
                rmse, DateTime.Now.Subtract(start).TotalSeconds));
        }

        private void btnSingle_Click(object sender, EventArgs e)
        {
            if (trainer == null || TrainingData == null)
            {
                btnRun_Click(sender, e);
                if (trainer == null || TrainingData == null) return;
            }
            // Il file "single" e' un CSV con un solo record: lo carico e normalizzo con le stats del train.
            LoadData single = new LoadData(singleFilePath, "median_house_value");
            if (single.data.Count == 0)
            {
                lstLog.Items.Insert(0, "Nessun record nel file single");
                return;
            }
            single.NormalizeFeaturesWith(TrainingData.FeatureMean, TrainingData.FeatureStd);
            double predScaled = trainer.network.Prediction(new List<double>(single.data[0]))[0];
            double prevision = predScaled * TrainingData.LabelStd + TrainingData.LabelMean;
            lstLog.Items.Insert(0, string.Format("Previsione: {0}, label: {1}",
                (int)prevision, (int)single.labels[0]));
        }
    }

}