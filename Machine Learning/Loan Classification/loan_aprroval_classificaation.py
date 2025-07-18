import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder, StandardScaler 
from sklearn.ensemble import RandomForestClassifier 
from sklearn.metrics import classification_report, confusion_matrix ,accuracy_score 
import matplotlib.pyplot as plt
import seaborn as sns

df = pd.read_csv("loan_approval_dataset.csv")

df

df.columns = df.columns.str.strip()
df = df.drop(columns=["loan_id"])

df.info()

df.isnull().sum()

le = LabelEncoder()
df["education"] = le.fit_transform(df["education"])          
df["self_employed"] = le.fit_transform(df["self_employed"])  
df["loan_status"] = le.fit_transform(df["loan_status"]) 

df.head()

plt.figure(figsize=(6, 4))
sns.countplot(data=df, x='loan_status', palette='Set2')
plt.title("Loan Approval Distribution")
plt.xticks([0, 1], ['Rejected', 'Approved'])
plt.xlabel("Loan Status")
plt.ylabel("Count")
plt.tight_layout()
plt.show()

education_labels = le.classes_  # ['Graduate', 'Not Graduate']
education_counts = df["education"].value_counts()
plt.figure(figsize=(6, 6))
plt.pie(
    education_counts,
    labels=education_labels,
    autopct="%1.1f%%",
    startangle=140,
    explode=(0.05, 0),
    shadow=True,
    colors=["#99ff99", "#66b3ff"]
)
plt.title("Education Level Distribution")
plt.axis("equal")
plt.tight_layout()
plt.show()

selected_features = ["income_annum", "loan_amount", "loan_term", "cibil_score", "loan_status"]
sns.pairplot(df[selected_features], hue="loan_status", palette='husl')
plt.show()

selected_features = ["income_annum", "loan_amount", "loan_term", "cibil_score", "education", "self_employed"]
X = df[selected_features]
y = df["loan_status"]


X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)

model = RandomForestClassifier(random_state=42)
model.fit(X_train_scaled, y_train)
y_pred = model.predict(X_test_scaled)

accuracy = accuracy_score(y_test, y_pred)
print(f"Model Accuracy: {accuracy * 100:.2f}%")

plt.figure(figsize=(10, 8))
corr = df.corr(numeric_only=True)
sns.heatmap(corr, annot=True, cmap='coolwarm', linewidths=0.5, fmt=".2f")
plt.title("Correlation Heatmap of Features")
plt.tight_layout()
plt.show()


conf_matrix = confusion_matrix(y_test, y_pred)
plt.figure(figsize=(6, 4))
sns.heatmap(conf_matrix, annot=True, fmt='d', cmap='Blues')
plt.title("Confusion Matrix")
plt.xlabel("Predicted")
plt.ylabel("Actual")
plt.show()

print("Classification Report:\n")
print(classification_report(y_test, y_pred))

feature_importance = pd.Series(model.feature_importances_, index=X.columns).sort_values(ascending=False)
plt.figure(figsize=(8, 5))
sns.barplot(x=feature_importance, y=feature_importance.index, palette='viridis')
plt.title("Feature Importance - Random Forest")
plt.xlabel("Importance Score")
plt.ylabel("Features")
plt.tight_layout()
plt.show()

import nbformat

def notebook_to_python_simple(notebook_name: str, output_name: str):
    with open(notebook_name, "r", encoding="utf-8") as f:
        notebook = nbformat.read(f, as_version=4)

    code_cells = [cell['source'] for cell in notebook.cells if cell.cell_type == 'code']
    python_code = '\n\n'.join(code_cells)

    with open(output_name, "w", encoding="utf-8") as f:
        f.write(python_code)

    print(f"Converted '{notebook_name}' to '{output_name}'")

# Example usage:
if __name__ == "__main__":
    notebook_file = input("Enter the notebook filename (e.g., 'notebook.ipynb'): ")
    output_file = input("Enter the output python filename (e.g., 'script.py'): ")
    notebook_to_python_simple(notebook_file, output_file)


import pickle

with open("loan_model.pkl", "wb") as f:
    pickle.dump(model, f)

with open("scaler.pkl", "wb") as f:
    pickle.dump(scaler, f)

with open("label_encoder.pkl", "wb") as f:
    pickle.dump(le, f)


from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import numpy as np
import pickle
import uvicorn
from fastapi.middleware.cors import CORSMiddleware

# Load the model, scaler, and label encoder from pickle files
with open("loan_model.pkl", "rb") as f:
    model = pickle.load(f)

with open("scaler.pkl", "rb") as f:
    scaler = pickle.load(f)

with open("label_encoder.pkl", "rb") as f:
    le = pickle.load(f)  # If you want to use it (not used in this example)

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # change this to your frontend URL in production
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Define input data schema
class LoanRequest(BaseModel):
    income_annum: float
    loan_amount: float
    loan_term: int
    cibil_score: int
    education: int   # Assuming already encoded, e.g., 0, 1, 2
    self_employed: int  # Assuming 0 or 1

@app.post("/loan_predict")
async def loan_predict(data: LoanRequest):
    try:
        # Convert input data to numpy array
        input_data = np.array([[data.income_annum, data.loan_amount, data.loan_term,
                                data.cibil_score, data.education, data.self_employed]])

        # Scale input features
        input_scaled = scaler.transform(input_data)

        # Predict with model
        prediction = model.predict(input_scaled)

        # Return response
        if prediction[0] == 1:
            return {"result": "Loan Approved"}
        else:
            return {"result": "Loan Denied"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Prediction error: {e}")

if __name__ == "__main__":
    import nest_asyncio
    nest_asyncio.apply()
    uvicorn.run(app, host="0.0.0.0", port=8003)
