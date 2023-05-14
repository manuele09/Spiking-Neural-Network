from ultralytics import YOLO
from ultralytics.yolo.v8.detect.predict import DetectionPredictor
import cv2
from PIL import Image

#model = YOLO("yolov8n.pt")  # load a pretrained model (recommended for training)
#results = model.predict(source="1", show=True)

#print(results)
####################

# Initialize the video capture object
cap = cv2.VideoCapture(1)  # 0 is usually the built-in webcam

# Load the model
model = YOLO("yolov8n.pt")  # load a pretrained model

# Loop until the user decides to exit
# Loop until the user decides to exit
try:
    while True:
        # Capture frame-by-frame
        ret, frame = cap.read()
        if not ret:
            print("Can't receive frame (stream end?). Exiting ...")
            break

        # Convert the frame to a PIL Image
        image = Image.fromarray(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))

        # Make a prediction
        results = model.predict(image)

        # Extract bounding boxes, segmentation masks, and class probabilities
        for result in results:
            boxes = result.boxes  # Assuming boxes are in format (x1, y1, x2, y2)
            # Since we don't know the format of 'boxes', 'masks', 'probs' exactly, this is a general example

            # Draw the bounding box
            cv2.rectangle(frame, (boxes[0], boxes[1]), (boxes[2], boxes[3]), (0, 255, 0), 2)

            # Write the class probabilities as labels
            # This assumes that 'probs' is a list of probabilities, one for each class
            #label = f"Probabilities: {probs}"
            #cv2.putText(frame, label, (boxes[0], boxes[1] - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0, 255, 0), 2)

        # Display the resulting frame
        cv2.imshow('Frame', frame)

        # Break the loop on 'q' key press
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
except KeyboardInterrupt:
    pass


# After the loop release the cap object
cap.release()

# Destroy all the windows
cv2.destroyAllWindows()
