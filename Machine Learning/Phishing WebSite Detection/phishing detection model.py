import pandas as pd
import numpy as np
import seaborn as sns
import matplotlib.pyplot as plt

df = pd.read_csv("5.urldata.csv")
df.head()

df.shape

df.info()

df.describe()

df.isna().sum()

df.hist(bins = 50,figsize = (15,15))
plt.show()

plt.figure(figsize=(15,13))
sns.heatmap(df.select_dtypes(include=[np.number]).corr(), annot=True)
plt.show()

data =df.drop(['Domain'], axis = 1).copy()
df.head()

data.head()

# shuffling the rows in the dataset so that when splitting the train and test set are equally distributed
data = data.sample(frac=1).reset_index(drop=True)
data.head()

y = data['Label']
X = data.drop('Label',axis=1)
X.shape, y.shape

from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size = 0.2, random_state = 12)
X_train.shape, X_test.shape

from xgboost import XGBClassifier

xgb = XGBClassifier(learning_rate=0.4,max_depth=7)
xgb.fit(X_train, y_train)

y_test_XGB = xgb.predict(X_test)
y_train_XGB = xgb.predict(X_train)

acc_train = accuracy_score(y_train,y_train_XGB)
acc_test = accuracy_score(y_test,y_test_XGB)
print("Accuracy on training Data: {:.3f}".format(acc_train))
print("Accuracy on test Data: {:.3f}".format(acc_test))

from xgboost import plot_importance

plt.figure(figsize=(12, 6))
plot_importance(xgb)
plt.show()

from sklearn.metrics import confusion_matrix, ConfusionMatrixDisplay

cm = confusion_matrix(y_test, y_test_XGB)
disp = ConfusionMatrixDisplay(confusion_matrix=cm)
disp.plot(cmap='Blues')
plt.show()

from sklearn.metrics import roc_curve, auc

y_probs = xgb.predict_proba(X_test)[:,1] 
fpr, tpr, _ = roc_curve(y_test, y_probs)
roc_auc = auc(fpr, tpr)

plt.figure(figsize=(8,6))
plt.plot(fpr, tpr, color='blue', label=f'ROC curve (area = {roc_auc:.2f})')
plt.plot([0, 1], [0, 1], color='grey', linestyle='--')
plt.xlabel('False Positive Rate')
plt.ylabel('True Positive Rate')
plt.title('Receiver Operating Characteristic (ROC) Curve')
plt.legend()
plt.show()

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

