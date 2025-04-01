import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import MinMaxScaler,StandardScaler,LabelEncoder,OneHotEncoder,RobustScaler
from sklearn.metrics import accuracy_score , mean_absolute_error,mean_squared_error , r2_score 
from sklearn.compose import ColumnTransformer
from sklearn.pipeline import Pipeline

df=pd.read_csv('CreditCardBalance.csv')
df.head()

data_cleaned = df.drop(columns=["Unnamed: 0"])

X = data_cleaned.drop(columns=["Balance"])
y = data_cleaned["Balance"]

categorical_cols = X.select_dtypes(include=["object"]).columns
numerical_cols = X.select_dtypes(include=["number"]).columns

numerical_transformer = StandardScaler()

categorical_transformer = OneHotEncoder(drop="first")

preprocessor = ColumnTransformer(
    transformers=[
        ("num", numerical_transformer, numerical_cols),
        ("cat", categorical_transformer, categorical_cols),
    ]
)

model = Pipeline(steps=[
    ("preprocessor", preprocessor),
    ("regressor", LinearRegression())
])

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

model.fit(X_train, y_train)
y_pred = model.predict(X_test)

mae = mean_absolute_error(y_test, y_pred)
mse = mean_squared_error(y_test, y_pred)
r2 = r2_score(y_test, y_pred)

# Display evaluation metrics
print("Mean Absolute Error (MAE):", mae)
print("Mean Squared Error (MSE):", mse)
print("R-squared (R²):", r2)

import nbformat
import os

def convert_ipynb_to_py(ipynb_filename):
    if not ipynb_filename.endswith(".ipynb"):
        print("Error: Please provide a valid .ipynb file.")
        return
    
    py_filename = os.path.splitext(ipynb_filename)[0] + ".py"
    
    try:
        with open(ipynb_filename, 'r', encoding='utf-8') as f:
            notebook = nbformat.read(f, as_version=4)
        
        with open(py_filename, 'w', encoding='utf-8') as f:
            for cell in notebook.cells:
                if cell.cell_type == "code":
                    f.write("\n".join(cell.source.splitlines()) + "\n\n")
        
        print(f"Successfully converted {ipynb_filename} to {py_filename}")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    filename = input("Enter the Jupyter Notebook filename (with .ipynb extension): ")
    convert_ipynb_to_py(filename)

import pickle

model_data = {
    "model": model,
    "features": X.columns.tolist()
}

with open("credit_model.pkl", "wb") as f:
    pickle.dump(model_data, f)

print("Model saved as credit_model.pkl")

import logging
import pickle
import pandas as pd
import numpy as np
from fastapi import FastAPI
from pydantic import BaseModel
import uvicorn

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Load the trained pipeline
try:
    with open("credit_model.pkl", "rb") as f:
        model_schema = pickle.load(f)
    
    model = model_schema.get("model")
    features = model_schema.get("features", [])
    
    if model is None or not features:
        raise ValueError("Model or features missing in pickle file.")

    logger.info("Model loaded successfully.")

except Exception as e:
    logger.error(f"Error loading model: {e}")
    model = None
    features = []

# FastAPI App
app = FastAPI()

# Define request schema
class CreditApplication(BaseModel):
    Income: float
    Limit: int
    Rating: int
    Cards: int
    Age: int
    Education: int
    Gender: str
    Student: str
    Married: str
    Ethnicity: str

# Preprocess input for prediction
def preprocess_input(data: CreditApplication):
    df = pd.DataFrame([data.dict()])

    # Ensure all expected features exist
    missing_cols = [col for col in features if col not in df.columns]
    for col in missing_cols:
        df[col] = 0  # Default for missing columns

    # Reorder columns to match training set
    df = df.reindex(columns=features, fill_value=0)

    return df

# Prediction endpoint
@app.post("/predict")
async def predict_credit(data: CreditApplication):
    if model is None:
        return {"error": "Model is not loaded properly."}

    try:
        processed_data = preprocess_input(data)
        prediction = model.predict(processed_data)

        # Reverse log transformation if applied in training
        predicted_amount = float(np.expm1(prediction[0])) if "log" in model_schema else float(prediction[0])

        return {"Loan amount you can take": max(0, predicted_amount)}
    except Exception as e:
        logger.error(f"Prediction error: {e}")
        return {"error": str(e)}

# Run FastAPI
if __name__ == "__main__":
    import nest_asyncio
    nest_asyncio.apply()
    uvicorn.run(app, host="0.0.0.0", port=8002)

