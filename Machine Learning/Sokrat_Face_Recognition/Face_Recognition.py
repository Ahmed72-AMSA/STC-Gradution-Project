
import cv2
import os
import random
import numpy as np
from matplotlib import pyplot as plt
from tensorflow.keras.models import Model
from tensorflow.keras.layers import Layer, Conv2D, Dense, MaxPooling2D, Input, Flatten
import tensorflow as tf
from tensorflow.keras import regularizers
from tensorflow.keras.metrics import Precision,Recall
from sklearn.metrics import classification_report, confusion_matrix, ConfusionMatrixDisplay
from tensorflow.keras.layers import Layer



gpus = tf.config.experimental.list_physical_devices('GPU')
for gpu in gpus: 
    tf.config.experimental.set_memory_growth(gpu, True)

POS_PATH = os.path.join('data', 'positive')
NEG_PATH = os.path.join('data', 'negative')
ANC_PATH = os.path.join('data', 'anchor')

import uuid
os.path.join(ANC_PATH, '{}.jpg'.format(uuid.uuid1()))

anchor = tf.data.Dataset.list_files(ANC_PATH+'\*.jpg').take(300)
positive = tf.data.Dataset.list_files(POS_PATH+'\*.jpg').take(300)
negative = tf.data.Dataset.list_files(NEG_PATH+'\*.jpg').take(300)

dir_test = anchor.as_numpy_iterator()

print(dir_test.next())


def preprocess(file_path):
    byte_img = tf.io.read_file(file_path)
    img = tf.io.decode_jpeg(byte_img, channels=3)  
    img = tf.image.resize(img, (105, 105))
    img = img / 255.0
    
    return img

positives = tf.data.Dataset.zip((anchor, positive, tf.data.Dataset.from_tensor_slices(tf.ones(len(anchor)))))
negatives = tf.data.Dataset.zip((anchor, negative, tf.data.Dataset.from_tensor_slices(tf.zeros(len(anchor)))))
data = positives.concatenate(negatives)

data

samples = data.as_numpy_iterator()
exampple = samples.next()
exampple

def preprocess_twin(input_img, validation_img, label):
    return(preprocess(input_img), preprocess(validation_img), label)

res = preprocess_twin(*exampple)

plt.imshow(res[0])

res[2]

data = data.map(preprocess_twin)
data = data.cache()
data = data.shuffle(buffer_size=1024)

samples = data.as_numpy_iterator()
samp = samples.next()

plt.imshow(samp[1])

samp[2]

train_data = data.take(round(len(data)*.7))
train_data = train_data.batch(8)
train_data = train_data.prefetch(8)

val_data = data.skip(round(len(data) * 0.7))
val_data = val_data.take(round(len(data) * 0.15))  
val_data = val_data.batch(8)
val_data = val_data.prefetch(8)

test_data = data.skip(round(len(data) * 0.85)) 
test_data = test_data.batch(8)
test_data = test_data.prefetch(8)

def make_embedding(): 
    inp = Input(shape=(105, 105, 3), name='input_image')
    
    c1 = Conv2D(64, (10, 10), activation='relu')(inp)
    m1 = MaxPooling2D((2, 2), padding='same')(c1)
    
    c2 = Conv2D(128, (7, 7), activation='relu')(m1)
    m2 = MaxPooling2D((2, 2), padding='same')(c2)
    
    c3 = Conv2D(128, (4, 4), activation='relu')(m2)
    m3 = MaxPooling2D((2, 2), padding='same')(c3)
    
    c4 = Conv2D(256, (4, 4), activation='relu')(m3)
    f1 = Flatten()(c4)
    
    d5 = Dense(4096, activation='sigmoid')(f1)
    
    return Model(inputs=[inp], outputs=[d5], name='embedding')

embedding = make_embedding()
embedding.summary()

class L1Dist(Layer):
    
    def __init__(self, **kwargs):
        super().__init__()
       
    def call(self, input_embedding, validation_embedding):
        return tf.math.abs(input_embedding - validation_embedding)

l1 = L1Dist()

input_image = Input(name='input_img', shape=(105,105,3))
validation_image = Input(name='validation_img', shape=(105,105,3))
inp_embedding = embedding(input_image)
val_embedding = embedding(validation_image)

siamese_layer = L1Dist()
distances = siamese_layer(inp_embedding, val_embedding)

def make_siamese_model(): 
    
    input_image = Input(name='input_img', shape=(105,105,3))
    
    validation_image = Input(name='validation_img', shape=(105,105,3))
    
    siamese_layer = L1Dist()
    siamese_layer._name = 'distance'
    distances = siamese_layer(embedding(input_image), embedding(validation_image))
    
    # Classification layer 
    classifier = Dense(1, activation='sigmoid')(distances)
    
    return Model(inputs=[input_image, validation_image], outputs=classifier, name='SiameseNetwork')

siamese_model = make_siamese_model()
siamese_model.summary()

binary_cross_entropy = tf.losses.BinaryCrossentropy()

opt = tf.optimizers.Adam(1e-4)

checkpoint_dir = "./training_checkpoints"
checkpoint_prefix = os.path.join(checkpoint_dir, 'ckpt')
checkpoint = tf.train.Checkpoint(opt = opt , siamese_model = siamese_model)

@tf.function
def train_step(batch):
    with tf.GradientTape() as tape:
        X = batch[:2]
        y = batch[2]

        yhat = siamese_model(X, training=True)
        loss = binary_cross_entropy(y, yhat)
        
    
    
    grad = tape.gradient(loss, siamese_model.trainable_variables)
    opt.apply_gradients(zip(grad, siamese_model.trainable_variables))
    
    return loss

@tf.function
def val_step(batch):
    X = batch[:2]
    y = batch[2]
    
    yhat = siamese_model(X, training=False)
    loss = binary_cross_entropy(y, yhat)
    
    
    return loss

import matplotlib.pyplot as plt

def train(data, val_data, EPOCHS):
    train_losses = []
    val_losses = []

    for epoch in range(1, EPOCHS + 1):
        print(f'\nEpoch {epoch}/{EPOCHS}')
        progbar = tf.keras.utils.Progbar(len(data))
        
        epoch_loss = 0
        val_loss = 0
        
        for idx, batch in enumerate(data):
            batch_loss = train_step(batch)
            epoch_loss += batch_loss
            progbar.update(idx + 1)

        # Calculate average training loss
        train_loss = epoch_loss / len(data)
        train_losses.append(train_loss)

        # Compute validation loss for the current epoch
        for val_batch in val_data:
            val_loss += val_step(val_batch)  # Call the val_step function

        val_loss /= len(val_data)
        val_losses.append(val_loss)

        print(f"Train Loss: {train_loss:.4f}, Validation Loss: {val_loss:.4f}")

        # Save checkpoint periodically
        if epoch % 10 == 0:
            checkpoint.save(file_prefix=checkpoint_prefix)

    # Plot training vs validation loss
    plt.figure(figsize=(10, 5))
    plt.plot(range(1, EPOCHS + 1), train_losses, label='Train Loss')
    plt.plot(range(1, EPOCHS + 1), val_losses, label='Validation Loss')
    plt.xlabel('Epochs')
    plt.ylabel('Loss')
    plt.title('Training vs Validation Loss')
    plt.legend()
    plt.show()

Epochs = 50

train(train_data,val_data,Epochs)

test_input , test_val , y_true = test_data.as_numpy_iterator().next()
y_hat = siamese_model.predict([test_input, test_val])
y_hat


[1 if prediction > 0.5 else 0 for prediction in y_hat ]

y_true

recall = Recall()
recall.update_state(y_true , y_hat)
print(f'Model Recall is: {recall.result().numpy()}')

precision = Precision()
precision.update_state(y_true , y_hat)
print(f'Model precision is: {precision.result().numpy()}')

plt.figure(figsize=(15,8))

plt.subplot(1,2,1)
plt.imshow(test_input[0])

plt.subplot(1,2,2)
plt.imshow(test_val[0])

plt.show()

from sklearn.metrics import confusion_matrix, accuracy_score, ConfusionMatrixDisplay


y_hat = (y_hat > 0.5).astype(int) 

cm = confusion_matrix(y_true, y_hat)
disp = ConfusionMatrixDisplay(confusion_matrix=cm, display_labels=['Class 0', 'Class 1'])

disp.plot(cmap=plt.cm.Blues)
plt.title("Confusion Matrix")
plt.show()

accuracy = accuracy_score(y_true, y_hat)
print(f'Accuracy: {accuracy:.4f}')

siamese_model.save('siamesemodel.h5')
model = tf.keras.models.load_model('siamesemodel.h5', 
                                   custom_objects={'L1Dist':L1Dist, 'BinaryCrossentropy':tf.losses.BinaryCrossentropy})

model.predict([test_input, test_val])

model.summary()

def verify(model, detection_threshold, verification_threshold):
    results = []
    for image in os.listdir(os.path.join('application_data', 'verification_images')):
        input_img = preprocess(os.path.join('application_data', 'input_image', 'input_image.jpg'))
        validation_img = preprocess(os.path.join('application_data', 'verification_images', image))
        
        result = model.predict(list(np.expand_dims([input_img, validation_img], axis=1)))
        results.append(result)
    
    detection = np.sum(np.array(results) > detection_threshold)
    
    verification = detection / len(os.listdir(os.path.join('application_data', 'verification_images'))) 
    verified = verification > verification_threshold
    print (verified)

    print (results)
    print (verification)
    print (detection)



    
    return verified

import tkinter as tk
from tkinter import Label, Button
import cv2
import os
import numpy as np
from tensorflow.keras.models import load_model
from PIL import Image, ImageTk

class VideoApp:
    def __init__(self, root, model):
        self.root = root
        self.root.title("Video Verification")
        self.model = model

        self.status_label = Label(root, text="Status: Waiting for verification")
        self.status_label.pack()

        self.verify_button = Button(root, text="Verify Manually", command=self.manual_verification)
        self.verify_button.pack()

        self.video_label = Label(root)
        self.video_label.pack()

        self.cap = cv2.VideoCapture(0)
        if not self.cap.isOpened():
            self.status_label.config(text="Camera not found!")
        else:
            self.update_frame()  

    def capture_frame(self):
        ret, frame = self.cap.read()
        if ret:
            frame = frame[120:120 + 250, 200:200 + 250, :]
            return frame
        return None

    def update_frame(self):
        ret, frame = self.cap.read()
        if ret:
            frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            image = Image.fromarray(frame_rgb)
            photo = ImageTk.PhotoImage(image)

            self.video_label.config(image=photo)
            self.video_label.image = photo

        self.root.after(10, self.update_frame)  

    def manual_verification(self):
        self.status_label.config(text="Status: Manual verification triggered")
        frame = self.capture_frame()
        if frame is not None:
            self.save_frame(frame)
            verified = verify(model,0.9,0.8)
            self.update_status(verified)

    def save_frame(self, frame):
        save_path = os.path.join('application_data', 'input_image', 'input_image.jpg')
        os.makedirs(os.path.dirname(save_path), exist_ok=True)
        cv2.imwrite(save_path, frame)

   

    def update_status(self, verified):
        status_message = "Verification Successful" if verified else "Verification Failed"
        self.status_label.config(text=f"Status: {status_message}")

    def on_close(self):
        if self.cap.isOpened():
            self.cap.release()
        cv2.destroyAllWindows()
        self.root.destroy()

if __name__ == "__main__":
    root = tk.Tk()
    model = model 
    app = VideoApp(root, model)
    root.protocol("WM_DELETE_WINDOW", app.on_close)
    root.mainloop()

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

