using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;
using System.Collections.Generic;
using System.Linq;

namespace STC.Services
{
    public class OnnxModelService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _modelA;
        private readonly PredictionEngine<CreditApprovalClassificationModelInput, CreditApprovalClassificationModelOutput> _predictionEngineA;
        private readonly string _modelPath;

        public OnnxModelService()
        {
            _mlContext = new MLContext();
            
            // Get the ONNX model path dynamically
            _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "credit_approval_classification_model.onnx");

            if (!File.Exists(_modelPath))
            {
                throw new FileNotFoundException($"ONNX model file not found at: {_modelPath}");
            }

            _modelA = LoadOnnxModel(_modelPath);
            _predictionEngineA = _mlContext.Model.CreatePredictionEngine<CreditApprovalClassificationModelInput, CreditApprovalClassificationModelOutput>(_modelA);
        }

        private ITransformer LoadOnnxModel(string modelPath)
        {
            var pipeline = _mlContext.Transforms.ApplyOnnxModel(
                outputColumnNames: new[] { "output_label", "output_probability" }, // Match ONNX output names
                inputColumnNames: GetOnnxModelInputSchema(modelPath), // Dynamically retrieve input names
                modelFile: modelPath);

            IDataView emptyDataView = _mlContext.Data.LoadFromEnumerable(new List<CreditApprovalClassificationModelInput>());
            return pipeline.Fit(emptyDataView);
        }

        public CreditApprovalClassificationModelOutput PredictModelA(CreditApprovalClassificationModelInput inputData)
        {
            return _predictionEngineA.Predict(inputData);
        }

        /// <summary>
        /// 🔍 Extracts ONNX model input schema dynamically
        /// </summary>
        private string[] GetOnnxModelInputSchema(string modelPath)
        {
            var pipeline = _mlContext.Transforms.ApplyOnnxModel(modelFile: modelPath);
            var emptyDataView = _mlContext.Data.LoadFromEnumerable(new List<CreditApprovalClassificationModelInput>());
            var transformer = pipeline.Fit(emptyDataView);

            // Extract schema
            var modelSchema = transformer.GetOutputSchema(emptyDataView.Schema);
            var inputNames = modelSchema.Select(col => col.Name).ToArray();
            Console.WriteLine($"🔍 ONNX Model Inputs: {string.Join(", ", inputNames)}");
            return inputNames;
        }
    }

    // Input Model
    public class CreditApprovalClassificationModelInput
    {
        [ColumnName("input")]
        public float[] Inputs { get; set; }
    }

    public class CreditApprovalClassificationModelOutput
    {
        public long[] Label { get; set; }

        // 🔥 Fix: Replace `OnnxSequenceType<float>` with `float[]`
        [ColumnName("output_probability")]
        public float[] Probabilities { get; set; }
    }
}
