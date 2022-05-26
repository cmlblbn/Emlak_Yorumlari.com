from pydantic import BaseModel

class PredictRequestModel(BaseModel):
    text: str

class SpecPredictRequestModel(BaseModel):
    model_id: int
    text: str

class TrainRequestModel(BaseModel):
    type: str
    maxlen: int
    batch_size: int
    epoch: int