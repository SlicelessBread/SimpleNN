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

        // Curves currently drawn on the graph (by file name), so the same run isn't plotted twice
        // and several runs can be overlaid to compare them. A new training Run resets the set.
        private readonly HashSet<string> plottedRuns = new HashSet<string>();
        private int colorIndex = 0;
        private static readonly Color[] palette =
            { Color.Red, Color.Green, Color.Blue, Color.Magenta, Color.DarkOrange, Color.Teal, Color.Brown };

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

            // Explanation of each file shown on mouse hover.
            string trainTip = "TRAINING data: a CSV with many records (features + median_house_value). Used by the Run button to train the network.";
            string testTip = "TEST data: a separate CSV, never seen during training. Used by the Test button to measure the real error (RMSE in $).";
            string singleTip = "SINGLE record: a CSV with a single row. Used by the Single button to predict that value and compare it to the real one.";

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
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
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
                lstLog.Items.Insert(0, string.Format("Loaded {0} records in {1:F2}s",
                    train.data.Count, DateTime.Now.Subtract(start).TotalSeconds));

                // 2) Rete: hidden layer ReLU secondo la UI, 1 output lineare (impostato dentro Network).
                Network net = new Network(train.FeatureCount, hiddenLayers, neurons, ActivationFunction.ReLU, 1);
                net.LearningRate = learningRate;      // input e target sono normalizzati

                // 3) Training. Salvo trainer e dati di training nei campi per Test/Single.
                trainer = new MainTrainer(net, epochs);
                TrainingData = train;
                start = DateTime.Now;
                trainer.Fit(train.data, train.labels);
                lstLog.Items.Insert(0, string.Format("Training: {0} epochs in {1:F2}s",
                    trainer.epoche, DateTime.Now.Subtract(start).TotalSeconds));

                // 4) Show the loss curve (a new Run resets the graph). Press "Test" to evaluate.
                double[] losses = trainer.lossLogs.ToArray();
                ResetPlot();
                AddCurve(losses, RunLabel(numbxEpox.Value, numbxLayers.Value, numbxNeurons.Value, losses));
                plottedRuns.Add(Path.GetFileName(RunFileName()));

                // 5) Save this run's loss curve so it can be recalled/compared later.
                saveRun();
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

        // Folder where training runs are saved/recalled. Uses the configured folder when it is
        // usable on this machine, otherwise falls back to a "Results" folder next to the app.
        private static string ResultsDir()
        {
            string configured = Properties.Settings.Default.ResultsFolder;
            string dir = configured;
            try
            {
                string? parent = Path.GetDirectoryName(Path.TrimEndingDirectorySeparator(configured));
                if (string.IsNullOrWhiteSpace(configured) || parent == null || !Directory.Exists(parent))
                    dir = Path.Combine(AppContext.BaseDirectory, "Results");
            }
            catch
            {
                dir = Path.Combine(AppContext.BaseDirectory, "Results");
            }
            Directory.CreateDirectory(dir);
            return dir;
        }

        // File name encodes the hyperparameters, so a run can be recalled by re-selecting them.
        private string RunFileName()
        {
            string name = string.Format("run_{0}_{1}_{2}.json", numbxEpox.Value, numbxLayers.Value, numbxNeurons.Value);
            return Path.Combine(ResultsDir(), name);
        }

        private void saveRun()
        {
            string fileName = RunFileName();
            string json = JsonConvert.SerializeObject(new { trainer.lossLogs });
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                File.WriteAllText(fileName, json);
                lstLog.Items.Insert(0, "Run saved: " + Path.GetFileName(fileName));
            }
            catch (Exception e)
            {
                lstLog.Items.Add("Error while saving to disk: " + e.Message);
            }
        }

        // Clears the graph and the set of drawn curves, ready for a fresh comparison.
        private void ResetPlot()
        {
            grphErr.Plot.Clear();
            grphErr.Plot.XLabel("Epoch");
            grphErr.Plot.YLabel("Error (loss)");
            plottedRuns.Clear();
            colorIndex = 0;
        }

        // Adds one loss curve to the graph (overlaid on whatever is already there) with the next
        // color from the palette and a legend entry.
        private void AddCurve(double[] losses, string label)
        {
            double[] x = new double[losses.Length];
            for (int i = 0; i < x.Length; i++) x[i] = i + 1;

            Color c = palette[colorIndex % palette.Length];
            colorIndex++;

            var scatter = grphErr.Plot.Add.Scatter(x, losses, color: ScottPlot.Color.FromColor(c));
            scatter.LegendText = label;
            grphErr.Plot.Axes.AutoScale();
            grphErr.Plot.ShowLegend(ScottPlot.Alignment.UpperRight, ScottPlot.Orientation.Vertical);
            grphErr.Refresh();
        }

        private static string RunLabel(decimal epochs, decimal layers, decimal neurons, double[] losses)
        {
            double best = losses.Length > 0 ? losses.Min() : 0;
            return string.Format("{0}ep / {1}L / {2}N  (best {3:F4})", epochs, layers, neurons, best);
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

        // When the hyperparameters change, recall a previously saved run for those exact values
        // (if one exists) and overlay its loss curve so it can be compared with the others.
        private void numbxEpox_ValueChanged(object sender, EventArgs e)
        {
            OldGraph(RunFileName());
        }

        // Loads a saved run and overlays it on the graph. Skips runs already shown so curves
        // accumulate for comparison without duplicates. A new training Run clears them (ResetPlot).
        private void OldGraph(string fileName)
        {
            if (!File.Exists(fileName)) return;

            string key = Path.GetFileName(fileName);
            if (plottedRuns.Contains(key)) return;

            // If this is the first curve (no Run yet this session), make sure the axes are labelled.
            if (plottedRuns.Count == 0)
            {
                grphErr.Plot.XLabel("Epoch");
                grphErr.Plot.YLabel("Error (loss)");
            }

            try
            {
                string json = File.ReadAllText(fileName);
                dynamic deserializedData = JsonConvert.DeserializeObject<dynamic>(json);
                if (deserializedData == null) return;

                double[]? losses = deserializedData.lossLogs.ToObject<double[]>();
                if (losses == null || losses.Length == 0) return;
                double best = losses.Min();
                plottedRuns.Add(key);
                AddCurve(losses, string.Format("{0}  (best {1:F4})", key, best));
            }
            catch
            {
                // Ignore malformed or incompatible (old-format) run files.
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            if (trainer == null || TrainingData == null)
            {
                lstLog.Items.Insert(0, "No trained model to test. Click Run first.");
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
                lstLog.Items.Insert(0, "No record found in the single file");
                return;
            }
            single.NormalizeFeaturesWith(TrainingData.FeatureMean, TrainingData.FeatureStd);
            double predScaled = trainer.network.Prediction(new List<double>(single.data[0]))[0];
            double prevision = predScaled * TrainingData.LabelStd + TrainingData.LabelMean;
            lstLog.Items.Insert(0, string.Format("Prediction: ${0:N0}, actual: ${1:N0}",
                (int)prevision, (int)single.labels[0]));
        }
    }

}