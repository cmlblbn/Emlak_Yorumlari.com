from backend.train import train_task
from backend.Classes import Draw2vec
from backend.Classes.PredictSpec import predict_asAModel
from .predict import predict

from fastapi import APIRouter
from fastapi.responses import Response
from .schemas import PredictRequestModel
from .schemas import TrainRequestModel
from.schemas import SpecPredictRequestModel

import json
from .helpers import NumpyEncoder
router = APIRouter()

@router.post("/predict/")
async def prediction(request_data: PredictRequestModel):
    response = predict(request_data.text)
    json_resp = json.dumps({"response": response}, ensure_ascii=False, cls=NumpyEncoder)
    return Response(content=json_resp, media_type="application/json")


@router.post("/train/")
async def train(request_data: TrainRequestModel):
    train_task.delay(request_data.type,request_data.maxlen,request_data.batch_size,request_data.epoch)
    resp = {"message": True}
    return resp


@router.post("/drawembedding/")
async def drawvector():
    Draw2vec.drawmaker()
    return True

@router.post("/specPredict/")
async def specprediction(request_data: SpecPredictRequestModel):
    response = predict_asAModel(request_data.model_id,request_data.text)
    json_resp = json.dumps({"response": response}, ensure_ascii=False, cls=NumpyEncoder)
    return Response(content=json_resp, media_type="application/json")