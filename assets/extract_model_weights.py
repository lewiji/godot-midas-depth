import torch
import torch_to_torchsharp

model = torch.hub.load("intel-isl/MiDaS", "DPT_Hybrid")
model.eval()

f = open("hybrid_model_weights.dat", "wb")
torch_to_torchsharp.save_state_dict(model.state_dict(), f)
f.close()


