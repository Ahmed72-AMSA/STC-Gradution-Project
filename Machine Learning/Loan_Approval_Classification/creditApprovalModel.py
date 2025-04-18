import numpy as np
import pandas as pd
import matplotlib
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn.preprocessing import MinMaxScaler,StandardScaler,LabelEncoder,OneHotEncoder,RobustScaler

df = pd.read_csv("Credit_card.csv")
df_label = pd.read_csv("Credit_card_label.csv")
df.head()

categorical_columns = df.select_dtypes(include=['object']).columns

# Display unique values for each feature
for column in categorical_columns:
    print(f"Possible values for {column}: {df[column].unique()}")

df_label.head()

merged_df = pd.merge(df, df_label, on="Ind_ID", how="inner")
merged_df.head()

merged_df.info()

merged_df.isna().sum()

sns.heatmap(merged_df.isnull(), cmap='coolwarm', cbar=False)
plt.title('Missing Values')
plt.show()

merged_df.loc[(merged_df['Employed_days'] > 0) & (merged_df['Type_Occupation'].isna()), 'Type_Occupation'] = 'unemployed'

merged_df.head()

merged_df.isnull().sum()

merged_df['Annual_income'] = merged_df['Annual_income'].interpolate(method='linear', limit_direction='forward',axis=0)
merged_df['Birthday_count'] = merged_df['Birthday_count'].interpolate(method='linear', limit_direction='forward',axis=0)
merged_df['Type_Occupation'] = merged_df['Type_Occupation'].fillna(df['Type_Occupation'].mode()[0])
merged_df['GENDER'] = merged_df['GENDER'].fillna(df['GENDER'].mode()[0])
merged_df.isnull().sum()

merged_df.label.value_counts()

plt.figure(figsize=(12,6))
sns.boxplot(merged_df)
plt.title('Outliers Detection')
plt.show()

numeric_columns = merged_df.select_dtypes(include=['float64', 'int64']).columns
numeric_columns = numeric_columns.drop('label', errors='ignore')

# Calculate the IQR for each numeric column
Q1 = merged_df[numeric_columns].quantile(0.25)
Q3 = merged_df[numeric_columns].quantile(0.75)
IQR = Q3 - Q1

outliers = ((merged_df[numeric_columns] < (Q1 - 1.5 * IQR)) | (merged_df[numeric_columns] > (Q3 + 1.5 * IQR)))

# Remove rows with any outliers
merged_df = merged_df[~outliers.any(axis=1)]

# Verify that the outliers were removed
merged_df.label.value_counts()

plt.figure(figsize=(12,6))
sns.boxplot(merged_df[numeric_columns])
plt.title('Outliers Detection')
plt.show()

X = merged_df.drop(['label','Ind_ID','Birthday_count'], axis=1)
y = merged_df['label']

import pandas as pd
from sklearn.compose import ColumnTransformer
from sklearn.preprocessing import StandardScaler, OneHotEncoder
from imblearn.over_sampling import SMOTE
from collections import Counter

# Assuming merged_df is your original dataframe
X = merged_df.drop(['label'], axis=1)
y = merged_df['label']

# Manually define the categorical and numerical columns
categorical_cols = ['GENDER', 'Car_Owner', 'Propert_Owner', 'Type_Income', 'EDUCATION',
                    'Marital_status', 'Housing_type', 'Type_Occupation']
numerical_cols = ['CHILDREN', 'Annual_income', 'Employed_days',
                  'Mobile_phone', 'Work_Phone', 'Phone', 'EMAIL_ID', 'Family_Members']

# Ensure that columns are correctly specified
print(f"Categorical columns: {categorical_cols}")
print(f"Numerical columns: {numerical_cols}")

# Define transformers for numerical and categorical columns
numerical_transformer = StandardScaler()
categorical_transformer = OneHotEncoder(sparse_output=False, drop='first')  # Ensure sparse_output=False

# Create a column transformer to apply transformations only to the correct columns
preprocessor = ColumnTransformer(
    transformers=[
        ("num", numerical_transformer, numerical_cols),        # Apply to numerical columns only
        ("cat", categorical_transformer, categorical_cols),    # Apply to categorical columns only
    ]
)

X_transformed = preprocessor.fit_transform(X)

smote = SMOTE(random_state=42, k_neighbors=1)
X_sampled, y_sampled = smote.fit_resample(X_transformed, y)

df_sampled = pd.DataFrame(X_sampled, columns=[f"feature_{i}" for i in range(X_sampled.shape[1])])

# Concatenate the resampled features and target label
merged_df_sampled = pd.concat([df_sampled, y_sampled], axis=1)

# Check the class distribution after resampling
print(f"Original class distribution: {Counter(y)}")
print(f"Resampled class distribution: {Counter(y_sampled)}")

X

gender_counts = merged_df["GENDER"].value_counts().reset_index()
gender_counts.columns = ["GENDER", "Count"]

sns.barplot(x="GENDER", y="Count", data=gender_counts, palette="pastel")
plt.title("Gender Distribution")
plt.xlabel("Gender")
plt.ylabel("Count")
plt.tight_layout()

plt.show()

gender_counts = merged_df["GENDER"].value_counts()

plt.figure(figsize=(6, 6))
gender_counts.plot(kind="pie", autopct="%1.1f%%", startangle=90, colors=["skyblue", "pink"], labels=["Male", "Female"])
plt.title("Gender Distribution")
plt.ylabel("") 
plt.tight_layout()

plt.show()

label_counts = merged_df["label"].value_counts().reset_index()
label_counts.columns = ["Label", "Count"]

sns.barplot(x="Label", y="Count", data=label_counts, palette="Set2")

plt.title("Label Distribution")
plt.xlabel("Label")
plt.ylabel("Count")
plt.tight_layout()

plt.show()

label_counts = merged_df["label"].value_counts()

plt.figure(figsize=(6, 6))
label_counts.plot(kind="pie", autopct="%1.1f%%", startangle=90, colors=["green", "red"], labels=["Label 1", "Label 0"])

# Customize the plot
plt.title("Label Distribution")
plt.ylabel("") 
plt.tight_layout()

# Show the plot
plt.show()

import seaborn as sns

sns.boxplot(y=merged_df["Annual_income"])

plt.title("Box Plot of Annual Income")
plt.xlabel("Annual Income")
plt.tight_layout()

plt.show()

X

from sklearn.preprocessing import LabelEncoder

# Copy the original data before encoding
corr_df = merged_df.copy()

# Encode categorical columns
categorical_cols = ['GENDER', 'Car_Owner', 'Propert_Owner', 'Type_Income', 'EDUCATION',
                    'Marital_status', 'Housing_type', 'Type_Occupation']

le = LabelEncoder()
for col in categorical_cols:
    corr_df[col] = le.fit_transform(corr_df[col])

# Now compute correlation with target 'label'
correlations = corr_df.corr()['label'].drop('label').sort_values(ascending=False)

# Plot correlation
plt.figure(figsize=(10, 6))
sns.barplot(x=correlations.values, y=correlations.index, palette="coolwarm")
plt.title('Feature Correlation with Target Label')
plt.xlabel('Correlation Coefficient')
plt.ylabel('Features')
plt.grid(True)
plt.tight_layout()
plt.show()

from sklearn.model_selection import train_test_split

# Split the balanced/resampled data
x_train, x_test, y_train, y_test = train_test_split(X_sampled, y_sampled, test_size=0.2, random_state=42)



print("Resampled class distribution:")
print(pd.Series(y_sampled).value_counts())

from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import ShuffleSplit , cross_val_score
cv=ShuffleSplit(n_splits=5, test_size =0.2, random_state=42)
cross_val_score(LogisticRegression(),X_sampled,y_sampled,cv=cv).mean()

from sklearn.ensemble import RandomForestClassifier, GradientBoostingClassifier
from sklearn.svm import SVC
models = {
    'SVC': SVC(),
    'RandomForestClassifier': RandomForestClassifier(),
    'GradientBoostingClassifier': GradientBoostingClassifier(),
}

cv = ShuffleSplit(n_splits=5, test_size=0.2, random_state=42)

from sklearn.metrics import classification_report , confusion_matrix, accuracy_score
LR = LogisticRegression(C= 1, fit_intercept= True, l1_ratio = 0.1, penalty= 'l1', solver = 'saga')
LR.fit(x_train,y_train)
LR_pred = LR.predict(x_test)
accuracy_score(y_test,LR_pred)

svc_model = SVC()
svc_model.fit(x_train,y_train)
svc_model.score(x_test,y_test)

rfc_model = RandomForestClassifier(random_state=42)
rfc_model.fit(x_train,y_train)
rfc_model.score(x_test,y_test)

rfc_pred = rfc_model.predict(x_test)
rfc_pred

svc_pred = svc_model.predict(x_test)

from sklearn.neighbors import KNeighborsClassifier
KNN = KNeighborsClassifier()
KNN.fit(x_train,y_train)
KNN.score(x_test,y_test)

KNN_pred = KNN.predict(x_test)

from sklearn.metrics import roc_curve, auc, roc_auc_score
y_score = LR.predict_proba(x_test)[:, 1] 

fpr, tpr, _ = roc_curve(y_test, y_score) 
roc_auc = auc(fpr, tpr)

# Plot the ROC curve
plt.figure(figsize=(10, 8))
plt.plot(fpr, tpr, label=f"Logistic Regression (AUC = {roc_auc:.2f})")

plt.plot([0, 1], [0, 1], 'k--', label="Random Guess (AUC = 0.5)")

plt.xlabel("False Positive Rate")
plt.ylabel("True Positive Rate")
plt.title("ROC Curve for Logistic Regression")
plt.legend(loc="lower right")
plt.grid()
plt.show()

# Print the overall AUC
print(f"Overall ROC AUC for Logistic Regression: {roc_auc:.4f}")

LR_report = classification_report(y_test, LR_pred, output_dict=True)
# Convert the classification report to a DataFrame
LR_report_df = pd.DataFrame(LR_report).transpose()
# Plot the heatmap
plt.figure(figsize=(10, 6))
sns.heatmap(LR_report_df.iloc[:-1, :-1], annot=True, cmap='viridis', fmt=".2f")
plt.title("Classification Report of Logistic Regression Model")
plt.show()

y_test_pred = LR.predict(x_test)
cm = confusion_matrix(y_test, y_test_pred)

plt.figure(figsize=(8, 6))
sns.heatmap(cm, annot=True, fmt="d", cmap='Blues')
plt.title('Confusion Matrix for Logistic Regression')
plt.xlabel('Predicted')
plt.ylabel('Actual')
plt.show()

y_score = KNN.predict_proba(x_test)[:, 1]  # Probabilities for the positive class (index 1)

# Compute the ROC curve and AUC
fpr, tpr, _ = roc_curve(y_test, y_score)  # No need to binarize y_test for binary classification
roc_auc = auc(fpr, tpr)

# Plot the ROC curve
plt.figure(figsize=(10, 8))
plt.plot(fpr, tpr, label=f"Kneighbors Model (AUC = {roc_auc:.2f})")

# Plot diagonal line for random guessing
plt.plot([0, 1], [0, 1], 'k--', label="Random Guess (AUC = 0.5)")

plt.xlabel("False Positive Rate")
plt.ylabel("True Positive Rate")
plt.title("ROC Curve for Knneighbors Model")
plt.legend(loc="lower right")
plt.grid()
plt.show()

# Print the overall AUC
print(f"Overall ROC AUC for Kneighbors Model: {roc_auc:.4f}")

KN_report = classification_report(y_test, KNN_pred, output_dict=True)
# Convert the classification report to a DataFrame
KN_report_df = pd.DataFrame(KN_report).transpose()
plt.figure(figsize=(10, 6))
sns.heatmap(KN_report_df.iloc[:-1, :-1], annot=True, cmap='viridis', fmt=".2f")
plt.title("Classification Report of Kneighbors Model")
plt.show()

y_test_pred = KNN.predict(x_test)
cm = confusion_matrix(y_test, y_test_pred)

plt.figure(figsize=(8, 6))
sns.heatmap(cm, annot=True, fmt="d", cmap='Blues')
plt.title('Confusion Matrix for Kneighbors Model')
plt.xlabel('Predicted')
plt.ylabel('Actual')
plt.show()

y_score = svc_model.decision_function(x_test)  # Decision function outputs for binary classification

# Compute the ROC curve and AUC
fpr, tpr, _ = roc_curve(y_test, y_score)  # No need to binarize y_test for binary classification
roc_auc = auc(fpr, tpr)

# Plot the ROC curve
plt.figure(figsize=(10, 8))
plt.plot(fpr, tpr, label=f"Support Vector Classifier (AUC = {roc_auc:.2f})")

# Plot diagonal line for random guessing
plt.plot([0, 1], [0, 1], 'k--', label="Random Guess (AUC = 0.5)")

plt.xlabel("False Positive Rate")
plt.ylabel("True Positive Rate")
plt.title("ROC Curve for Support Vector Classifier")
plt.legend(loc="lower right")
plt.grid()
plt.show()

# Print the overall AUC
print(f"Overall ROC AUC for Support Vector Classifier Model: {roc_auc:.4f}")

y_score = rfc_model.predict_proba(x_test)[:, 1]

fpr, tpr, _ = roc_curve(y_test, y_score)  # No need to binarize y_test for binary classification
roc_auc = auc(fpr, tpr)

# Plot the ROC curve
plt.figure(figsize=(10, 8))
plt.plot(fpr, tpr, label=f"Random forest Classifier (AUC = {roc_auc:.2f})")

# Plot diagonal line for random guessing
plt.plot([0, 1], [0, 1], 'k--', label="Random Guess (AUC = 0.5)")

plt.xlabel("False Positive Rate")
plt.ylabel("True Positive Rate")
plt.title("ROC Curve for Support Vector Classifier")
plt.legend(loc="lower right")
plt.grid()
plt.show()

# Print the overall AUC
print(f"Overall ROC AUC for Random Forest Classifier Model: {roc_auc:.4f}")

rfc_report = classification_report(y_test, rfc_pred, output_dict=True)
rfc_report_df = pd.DataFrame(rfc_report).transpose()
# Plot the heatmap
plt.figure(figsize=(10, 6))
sns.heatmap(rfc_report_df.iloc[:-1, :-1], annot=True, cmap='viridis', fmt=".2f")
plt.title("Classification Report of random forest Model")
plt.show()

y_test_pred = rfc_model.predict(x_test)
cm = confusion_matrix(y_test, y_test_pred)

plt.figure(figsize=(8, 6))
sns.heatmap(cm, annot=True, fmt="d", cmap='Blues')
plt.title("Confusion Matrix for Random Forest Classifer")
plt.xlabel('Predicted')
plt.ylabel('Actual')
plt.show()

models = {
    'Logistic Regression': LR,
    'SVM': svc_model,
    'Kneighbors Classifier': KNN,
    "RandomForestClassifier": rfc_model

}

accuracies = {}

for model_name, model in models.items():
    y_test_pred = model.predict(x_test)
    accuracy = accuracy_score(y_test, y_test_pred)
    accuracies[model_name] = accuracy

plt.figure(figsize=(10, 6))
sns.barplot(x=list(accuracies.keys()), y=list(accuracies.values()), palette="viridis")

for i, (model_name, accuracy) in enumerate(accuracies.items()):
    plt.text(i, accuracy + 0.01, f"{accuracy:.2f}", ha='center', va='bottom', fontsize=10)

plt.title("Model Accuracy Comparison")
plt.xlabel("Model")
plt.ylabel("Accuracy")
plt.ylim(0, 1)
plt.xticks(rotation=45)
plt.show()

x_train[0]

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

# Assuming x_test is a numpy array and y_test is a pandas Series
single_sample = x_test[10].reshape(1, -1)  # Reshape it to (1, n_features)

# Now you can make the prediction
prediction = svc_model.predict(single_sample)

# Get the true label for comparison using .iloc[] (position-based indexing)
y_sample = y_test.iloc[10]  # Access the 100th sample in y_test

# Print the prediction and the true label
print(f"Prediction: {prediction}")
print(f"True label: {y_sample}")

import pandas as pd
from sklearn.preprocessing import StandardScaler, OneHotEncoder
from sklearn.compose import ColumnTransformer
from sklearn.pipeline import Pipeline
from sklearn.ensemble import RandomForestClassifier

# Example of how your preprocessor might be set up
# Adjust this based on your actual preprocessing pipeline
# Ensure you have preprocessor and LR (Logistic Regression) already defined

# Preprocessing pipeline (assuming you have this defined previously)
categorical_features = ["GENDER", "Car_Owner", "Propert_Owner", "Type_Income", "EDUCATION", "Marital_status", "Housing_type"]
numerical_features = ["CHILDREN", "Annual_income", "Employed_days", "Mobile_phone", "Work_Phone", "Phone", "EMAIL_ID", "Type_Occupation", "Family_Members"]

# Categorical preprocessing: OneHotEncoder
categorical_transformer = OneHotEncoder(handle_unknown='ignore')

# Numerical preprocessing: StandardScaler
numerical_transformer = StandardScaler()

# Combine transformers into a column transformer
preprocessor = ColumnTransformer(
    transformers=[
        ('cat', categorical_transformer, categorical_features),
        ('num', numerical_transformer, numerical_features)
    ])

# Assuming your model has been trained using the same preprocessor
# Step 1: Prepare the new input data
new_input_data = {
    "GENDER": ["F"],
    "Car_Owner": ["Y"],
    "Propert_Owner": ["N"],
    "CHILDREN": [2],
    "Annual_income": [157500],
    "Type_Income": ["Commercial associate"],
    "EDUCATION": ["Incomplete higher"],
    "Marital_status": ["Married"],
    "Housing_type": ["House / apartment"],
    "Employed_days": [-12857],
    "Mobile_phone": [-1399],
    "Work_Phone": [1],
    "Phone": [0],
    "EMAIL_ID": [0],
    "Type_Occupation": [1],
    "Family_Members": [4]
}

# Convert the new input data to DataFrame
new_input_df = pd.DataFrame(new_input_data)

# Step 2: Handle missing values
new_input_df.fillna('Unknown', inplace=True)  # Fill any missing values with 'Unknown' (or appropriate strategy)

# Step 3: Preprocess the new input data
# Ensure you apply the same preprocessing steps (encoding, scaling, etc.) here
new_input_processed = preprocessor.transform(new_input_df)

# Step 4: Load your pre-trained model (example with Logistic Regression model)
# Assuming you have the model saved and loaded as 'LR'
# prediction = LR.predict(new_input_processed)

# Step 5: Make prediction with your model (example with Logistic Regression model)
# Here you can use the model to predict, assuming 'LR' is a pre-trained classifier
# prediction = LR.predict(new_input_processed)

# Step 6: Output the prediction
# print(f"Prediction for the new input: {prediction[0]}")

# import joblib

# # Save the model
# joblib.dump(rfc_model, "credit_model.pkl")

# # Save your preprocessor (ColumnTransformer)
# joblib.dump(preprocessor, "preprocessor.pkl")

# import joblib

# # Load the model and preprocessor from the .pkl files
# model = joblib.load('credit_model.pkl')
# preprocessor = joblib.load('preprocessor.pkl')

# # Check what is in the model and preprocessor
# print("Model:", model)
# print("Preprocessor:", preprocessor)

# from fastapi import FastAPI
# from pydantic import BaseModel
# import joblib
# import pandas as pd
# import uvicorn

# # Load model and preprocessor
# model = joblib.load("credit_model.pkl")
# preprocessor = joblib.load("preprocessor.pkl")

# app = FastAPI()

# # Define input format
# class CreditInput(BaseModel):
#     GENDER: str
#     Car_Owner: str
#     Propert_Owner: str
#     CHILDREN: int
#     Annual_income: float
#     Type_Income: str
#     EDUCATION: str
#     Marital_status: str
#     Housing_type: str
#     Employed_days: int
#     Mobile_phone: int
#     Work_Phone: int
#     Phone: int
#     EMAIL_ID: int
#     Type_Occupation: str
#     Family_Members: int

# @app.post("/predict")
# def predict_credit_status(data: CreditInput):
#     input_data = data.dict()

#     # Convert input to DataFrame
#     df = pd.DataFrame([input_data])

#     # Preprocess the input
#     processed_input = preprocessor.transform(df)

#     # Predict
#     prediction = model.predict(processed_input)[0]

#     # Return the prediction result
#     return {
#         "Predicted_Label": int(prediction),
#         "Prediction": "Approved" if prediction == 1 else "Rejected"
#     }

# if __name__ == "__main__":
#     import nest_asyncio
#     nest_asyncio.apply()
#     uvicorn.run(app, host="0.0.0.0", port=8002)

