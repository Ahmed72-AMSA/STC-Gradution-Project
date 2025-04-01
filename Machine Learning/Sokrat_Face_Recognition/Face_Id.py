import tkinter as tk
from tkinter import Label, Button
import cv2
import os
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
from tensorflow.keras.layers import Layer
from PIL import Image, ImageTk
import sys

# Check if the app is frozen (running as .exe)
if getattr(sys, 'frozen', False):
    MODEL_PATH = os.path.join(sys._MEIPASS, 'siamesemodel.h5')
    verification_dir = os.path.join(sys._MEIPASS, 'verification_images')
else:  
    # Development paths for model and images
    MODEL_PATH = r"D:\studying section\projects\GP\STC-Gradution-Project\Machine Learning\Sokrat_Face_Recognition\siamesemodel.h5"
    verification_dir = r"D:\studying section\projects\GP\STC-Gradution-Project\Machine Learning\Sokrat_Face_Recognition\application_data\verification_images"

# Ensure the model file exists
if not os.path.exists(MODEL_PATH):
    raise FileNotFoundError(f"Model file not found: {MODEL_PATH}")

# Custom Distance Layer for Model
class L1Dist(Layer):
    def __init__(self, **kwargs):
        super().__init__()

    def call(self, input_embedding, validation_embedding):
        return tf.math.abs(input_embedding - validation_embedding)

# Load Model
model = load_model(MODEL_PATH, custom_objects={'L1Dist': L1Dist, 'BinaryCrossentropy': tf.losses.BinaryCrossentropy})

# Preprocessing Function
def preprocess(file_path):
    byte_img = tf.io.read_file(file_path)
    img = tf.io.decode_jpeg(byte_img, channels=3)
    img = tf.image.resize(img, (105, 105))
    img = img / 255.0
    return img

# Verification Function
def verify(model, detection_threshold=0.9, verification_threshold=0.8):
    input_img_path = os.path.abspath(os.path.join('application_data', 'input_image', 'input_image.jpg'))
    
    # Ensure verification directory exists
    os.makedirs(verification_dir, exist_ok=True)

    # Debugging: Print paths
    print(f"Input Image Path: {input_img_path}")
    print(f"Verification Directory: {verification_dir}")

    # Check if input image exists
    if not os.path.exists(input_img_path):
        print("Error: Input image not found!")
        return False

    # List valid verification images
    verification_images = [img for img in os.listdir(verification_dir) if img.lower().endswith(('.jpg', '.jpeg', '.png'))]
    
    # Debugging: Print found files
    print(f"Files found in verification directory: {os.listdir(verification_dir)}")
    print(f"Filtered valid images: {verification_images}")

    if len(verification_images) == 0:
        print("Error: No verification images found! Please check the path and file extensions.")
        return False

    results = []
    for image in verification_images:
        input_img = preprocess(input_img_path)
        validation_img = preprocess(os.path.join(verification_dir, image))

        # Expand dimensions and reshape for model input
        input_pair = np.expand_dims([input_img, validation_img], axis=1)
        result = model.predict(list(input_pair))
        results.append(result)

    detection = np.sum(np.array(results) > detection_threshold)
    verification = detection / len(verification_images)
    verified = verification > verification_threshold

    print(f"Results: {results}")
    print(f"Detection count: {detection}")
    print(f"Verification Score: {verification}")
    print(f"Verification Status: {'Success' if verified else 'Failed'}")

    return verified

# Video App Class
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
            h, w, _ = frame.shape
            # Ensuring crop does not exceed frame dimensions
            if h > 370 and w > 450:
                frame = frame[120:370, 200:450, :]
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
            verified = verify(model, 0.9, 0.8)
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
    app = VideoApp(root, model)
    root.protocol("WM_DELETE_WINDOW", app.on_close)
    root.mainloop()
