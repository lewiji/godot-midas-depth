import cv2
import torch
import urllib.request

model = torch.load("weights/dpt_hybrid-midas-501f0c75.pt", map_location=torch.device("cpu"))
#model = torch.hub.load("intel-isl/MiDaS", "DPT_Hybrid")
model = torch.jit.script(model)

url, filename = ("https://github.com/pytorch/hub/raw/master/images/dog.jpg", "dog.jpg")
urllib.request.urlretrieve(url, filename)
img = cv2.imread(filename)
img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

traced = torch.jit.trace(model, img)

traced.save("weights/hybrid_module.dat")
