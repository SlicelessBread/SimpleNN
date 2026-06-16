using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SimpleNN.Trainer
{
    /*
     * Carica un CSV stile California Housing e lo prepara per la rete.
     *  - `data`   = righe di feature (double[]), label esclusa.
     *  - `labels` = colonna target.
     * Flusso tipico:
     *   var train = new LoadData(path, "median_house_value"); train.FitNormalization();
     *   var test  = new LoadData(testPath, "median_house_value");
     *   test.NormalizeFeaturesWith(train.FeatureMean, train.FeatureStd);  // stats del TRAIN
     */
    internal class LoadData
    {
        public List<double[]> data { get; private set; } = new List<double[]>();
        public List<double> labels { get; private set; } = new List<double>();
        public string labelColumn { get; private set; }

        public double[] FeatureMean { get; private set; } = Array.Empty<double>();
        public double[] FeatureStd { get; private set; } = Array.Empty<double>();
        public double LabelMean { get; private set; }
        public double LabelStd { get; private set; } = 1.0;

        public int FeatureCount => data.Count > 0 ? data[0].Length : 0;

        // Encoding ordinale di ocean_proximity (semplice; un one-hot sarebbe leggermente migliore).
        private static readonly Dictionary<string, double> OceanProximity = new Dictionary<string, double>
        {
            { "NEAR BAY", 9 }, { "<1H OCEAN", 8 }, { "NEAR OCEAN", 6 }, { "ISLAND", 3 }, { "INLAND", 1 }
        };

        public LoadData(string name, string labelcolumn)
        {
            labelColumn = labelcolumn;
            FileInfo fileInfo = new FileInfo(name);
            if (!fileInfo.Exists) throw new Exception("File non trovato");
            if (!fileInfo.Extension.Equals(".csv", StringComparison.OrdinalIgnoreCase)) throw new Exception("File non .csv");
            ReadRaw(name);
        }

        private void ReadRaw(string fileName)
        {
            using StreamReader sr = new StreamReader(fileName);
            int labelIndex = -1;
            int expectedCols = -1;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(',');

                if (labelIndex == -1) // intestazione
                {
                    expectedCols = parts.Length;
                    for (int i = 0; i < parts.Length; i++)
                        if (parts[i].Trim() == labelColumn) labelIndex = i;
                    if (labelIndex == -1)
                        throw new Exception(string.Format("La colonna {0} non esiste", labelColumn));
                    continue;
                }

                if (parts.Length != expectedCols) continue;

                List<double> row = new List<double>();
                for (int i = 0; i < parts.Length; i++)
                {
                    string s = parts[i].Trim();
                    double val;

                    if (s.Length == 0)
                    {
                        val = double.NaN; // mancante -> imputato dopo con la media della colonna
                    }
                    // InvariantCulture: il CSV usa il punto decimale. Senza questo, su Windows
                    // italiano "8.3252" verrebbe letto come 83252 e i dati sarebbero rovinati.
                    else if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                    {
                        val = OceanProximity.TryGetValue(s.ToUpperInvariant(), out double mapped) ? mapped : 0.0;
                    }

                    if (i == labelIndex) labels.Add(val);
                    else row.Add(val);
                }
                data.Add(row.ToArray());
            }
        }

        // Calcola le statistiche SUL TRAINING, imputa i mancanti, standardizza feature e label.
        public void FitNormalization()
        {
            int nf = FeatureCount;
            FeatureMean = new double[nf];
            FeatureStd = new double[nf];

            // Media per colonna ignorando i NaN.
            for (int j = 0; j < nf; j++)
            {
                double sum = 0; int cnt = 0;
                foreach (var r in data) if (!double.IsNaN(r[j])) { sum += r[j]; cnt++; }
                FeatureMean[j] = cnt > 0 ? sum / cnt : 0.0;
            }
            // Imputazione dei mancanti con la media.
            foreach (var r in data)
                for (int j = 0; j < nf; j++)
                    if (double.IsNaN(r[j])) r[j] = FeatureMean[j];

            // Deviazione standard per colonna.
            for (int j = 0; j < nf; j++)
            {
                double s = 0;
                foreach (var r in data) { double d = r[j] - FeatureMean[j]; s += d * d; }
                FeatureStd[j] = Math.Sqrt(s / data.Count);
                if (FeatureStd[j] == 0) FeatureStd[j] = 1.0;
            }
            // Standardizzazione delle feature.
            foreach (var r in data)
                for (int j = 0; j < nf; j++)
                    r[j] = (r[j] - FeatureMean[j]) / FeatureStd[j];

            // Statistiche e standardizzazione del target.
            LabelMean = labels.Average();
            double ls = 0;
            foreach (var v in labels) { double d = v - LabelMean; ls += d * d; }
            LabelStd = Math.Sqrt(ls / labels.Count);
            if (LabelStd == 0) LabelStd = 1.0;
            for (int i = 0; i < labels.Count; i++)
                labels[i] = (labels[i] - LabelMean) / LabelStd;
        }

        // Per il TEST: standardizza le feature con le statistiche del training.
        // Le label restano in unita' originali (dollari) per calcolare l'RMSE reale.
        public void NormalizeFeaturesWith(double[] mean, double[] std)
        {
            int nf = FeatureCount;
            foreach (var r in data)
                for (int j = 0; j < nf; j++)
                {
                    if (double.IsNaN(r[j])) r[j] = mean[j];
                    r[j] = (r[j] - mean[j]) / std[j];
                }
        }
    }
}
