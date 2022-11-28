import numpy as np
import torch
import urllib.request
import cv2
from torchvision.transforms import Compose

models = {
    "hybrid": ("DPT_Hybrid", 384, 384),
    "large": ("DPT_Large", 384, 384)
}

sel_model = models["hybrid"]

url, filename = ("https://github.com/pytorch/hub/raw/master/images/dog.jpg", "dog.jpg")
urllib.request.urlretrieve(url, filename)

model = torch.jit.load("weights/hybrid_module.dat")
model.load_state_dict(torch.load("hybrid_model_weights.dat"))
device = torch.device("cpu")
model.to(device)
model.eval()

torch.onnx.export(model, img_input, "hybrid.onnx")
# 
# #midas_transforms = torch.hub.load("intel-isl/MiDaS", "transforms")
# 
# #img = cv2.imread(filename)
# #img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
# 
# #img_input = np.zeros((3, 384, 384), np.float32)
# 
# #transform = midas_transforms.dpt_transform
# 
# transform = Compose(
#     [
#         lambda img: {"image": img / 255.0},
#         midas_transforms.Resize(
#             384,
#             384,
#             resize_target=None,
#             keep_aspect_ratio=True,
#             ensure_multiple_of=32,
#             resize_method="minimal",
#             image_interpolation_method=cv2.INTER_CUBIC,
#         ),
#         lambda sample: torch.from_numpy(sample["image"]).unsqueeze(0),
#     ]
# )
# 
# 
# 
# #deviced = transform(img)
# 
# with torch.no_grad():
#     sample = torch.from_numpy(img_input).unsqueeze(0)
#     prediction = model.forward(sample)
#     prediction = (
#         torch.nn.functional.interpolate(
#             prediction.unsqueeze(1),
#             size=img_input.shape[:2],
#             mode="bicubic",
#             align_corners=False,
#         )
#         .squeeze()
#         .cpu()
#         .numpy()
#     )
#     torch.onnx.export(model, sample, "weights/hybrid.onnx", verbose=True, opset_version=9)
