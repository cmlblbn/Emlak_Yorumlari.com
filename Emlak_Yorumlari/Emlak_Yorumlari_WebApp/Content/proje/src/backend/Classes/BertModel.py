import numpy as np
import pandas as pd
from transformers import BertTokenizerFast
from transformers import TFBertModel
import tensorflow as tf
from keras.callbacks import EarlyStopping
import matplotlib.pyplot as plt
import seaborn as sns

from backend.Classes.EmbeddingBert import EmbeddingBert
from backend.Classes.Preprocess import Preprocessmaker
# import Tokenizer

class BertModel:

    def __init__(self, maxlen=100, pretrained_bert='bert-base-uncased'):
        self.maxlen = maxlen
        self.bertmodel = TFBertModel.from_pretrained(pretrained_bert)
        self.model = None
        self.history = None
        self.earlystop = EarlyStopping(monitor='val_loss', min_delta=0, patience=3, verbose=0, mode='auto')

    def construct_custom_model(self):

        opt = tf.keras.optimizers.Adam(learning_rate=1e-5, decay=1e-7)
        loss = tf.keras.losses.CategoricalCrossentropy()
        accuracy = tf.keras.metrics.CategoricalAccuracy()

        input_ids = tf.keras.Input(shape=(self.maxlen,), dtype='int32')

        attention_masks = tf.keras.Input(shape=(self.maxlen,), dtype='int32')

        embeddings = self.bertmodel([input_ids, attention_masks])[1]

        output = tf.keras.layers.Dense(3, activation="softmax")(embeddings)

        self.model = tf.keras.models.Model(inputs=[input_ids, attention_masks], outputs=output)

        self.model.compile(opt, loss=loss, metrics=accuracy)

        return self.model

    def fit(self, train_input_ids, train_attention_masks, test_input_ids, test_attention_masks, y_train, y_test,
            batch_size=32, epochs=10):
        self.history = self.model.fit([train_input_ids, train_attention_masks], y_train,
                                      validation_data=([test_input_ids, test_attention_masks], y_test),
                                      epochs=epochs, batch_size=batch_size)

    def get_metric(self, test_input_ids, test_attention_masks, y_test):
        self.model.evaluate(x=[test_input_ids, test_attention_masks], y=y_test)
        result = []
        result.append(self.history.history['val_categorical_accuracy'][-1])
        result.append(self.history.history['val_loss'][-1])
        return result

    def save_model(self, path, name):
        try:
            self.model.save(path + name)
        except:
            print("Model kaydedilirken bir hata oluştu")

    def load_model(self, path, name):
        try:
            self.model = tf.keras.models.load_model(path + name + ".h5", custom_objects={'TFBertModel': TFBertModel})
        except:
            print("Model Yüklenirken bir hata oluştu")

    def draw_metrics(self, path):

        try:
            acc = self.history.history['categorical_accuracy']
            val_acc = self.history.history['val_categorical_accuracy']
            loss = self.history.history['loss']
            val_loss = self.history.history['val_loss']

            epochs = range(1, len(acc) + 1)
            fig = plt.figure(figsize=(10, 5))
            fig.tight_layout()

            plt.plot(epochs, acc, 'r', label='Training acc')
            plt.plot(epochs, val_acc, 'b', label='Validation acc')
            plt.title('Training and validation accuracy')
            plt.ylabel('Accuracy')
            plt.legend()

            plt.savefig(path, dpi=250, bbox_inches='tight')

            plt.figure(figsize=(10, 5))
            plt.plot(epochs, loss, 'r', label='Training loss')
            plt.plot(epochs, val_loss, 'b', label='Validation loss')
            plt.title('Training and validation loss')
            plt.ylabel('Loss')
            plt.legend()

            plt.savefig(path, dpi=250, bbox_inches='tight')
        except:
            print("çizimler yapılırken bir hata oluştu")

    def predict(self, text):

        test_data = self.clean_forPredict(text)

        pred_sentences = test_data
        tok = EmbeddingBert()
        input_id, input_mask = tok.tokenize_forBert(pred_sentences, self.maxlen)
        tf_outputs = self.model.predict([input_id, input_mask])
        tf_predictions = tf.nn.softmax(tf_outputs[0], axis=-1)
        labels = ['olumsuz', 'olumlu', 'küfürlü']
        label = np.argmax(tf_predictions)

        return labels[label]

    def clean_forPredict(self, text):
        test_data = np.array([text])
        df = pd.DataFrame(test_data, columns=['text'])
        prep = Preprocessmaker(df)
        df = prep.preprocess(df)
        return df['text'].to_numpy()