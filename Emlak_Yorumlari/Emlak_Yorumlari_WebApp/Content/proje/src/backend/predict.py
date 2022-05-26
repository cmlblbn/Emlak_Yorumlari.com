from .tokenizer import tokenize
from transformers import TFBertModel
import tensorflow as tf
import numpy as np

from keras.preprocessing.sequence import pad_sequences
import psycopg2
import pickle

labels = ['olumsuz', 'olumlu', 'küfürlü']

conn = psycopg2.connect(
    database="MyDataBase", user='postgres', password='123321', host='127.0.0.1', port='5432'
)
cur = conn.cursor()
sql = ''' SELECT * FROM dbo."Models" WHERE "isActive" = TRUE '''
cur.execute(sql)
data = cur.fetchall()
modelName = data[0][1]
model_path = "./backend/Models/" + modelName + "/"


def predict(text, maxlen=100):

    if data[0][2] == "bert":
        reloaded_model = tf.keras.models.load_model(model_path + "bert_model.h5",
                                                    custom_objects={'TFBertModel': TFBertModel})

        pred_sentences = [text]
        input_id, input_mask = tokenize(pred_sentences, maxlen)
        tf_outputs = reloaded_model.predict([input_id, input_mask])
        tf_predictions = tf.nn.softmax(tf_outputs[0], axis=-1)
        label = np.argmax(tf_predictions)
        return labels[label]

    elif data[0][2] == "custom":
        reloaded_model = tf.keras.models.load_model(model_path + "CustomModel.h5")
        with open(model_path + "embedding.pickle", 'rb') as handle:
            reloaded_tokenizer = pickle.load(handle)
        test_data = np.array([text])
        test_data = reloaded_tokenizer.texts_to_sequences(test_data)
        test_data = pad_sequences(test_data, maxlen=data[0][7], padding='post')
        label = np.argmax(reloaded_model.predict(test_data))
        return labels[label]
