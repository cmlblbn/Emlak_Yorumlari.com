from backend.Classes.CustomModel import CustomModel
from backend.Classes.BertModel import BertModel
import psycopg2


def predict_asAModel(model_id, text):
    modeldatabase = get_model(model_id)
    print(modeldatabase)
    modelName = modeldatabase[0][1]
    modelType = modeldatabase[0][2]
    if (modelType == "custom"):

        model = CustomModel()
        model_path = "backend/" + modelName + "/"
        embedding_path = model_path + "embedding.pickle"
        model.load_model(model_path, "CustomModel")
        predict = model.predict(text, embedding_path)
        model = None
        return predict

    elif (modelType == "bert"):
        model = BertModel()
        model_path = "backend/" + modelName + "/"
        model.load_model(model_path, "bert_model")
        predict = model.predict(text)
        model = None
        return predict


def get_model(model_id):
    conn = psycopg2.connect(
        database="MyDataBase", user='postgres', password='123321', host='127.0.0.1', port='5432'
    )
    cur = conn.cursor()
    sql = ''' SELECT * FROM dbo."Models" WHERE model_id = %s '''
    cur.execute(sql, [model_id])
    data = cur.fetchall()
    conn.close()
    return data