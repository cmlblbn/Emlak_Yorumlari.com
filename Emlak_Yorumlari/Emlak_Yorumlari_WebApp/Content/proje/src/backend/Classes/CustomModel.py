import numpy as np
import pandas as pd
from keras.preprocessing.sequence import pad_sequences
from backend.Classes.Preprocess import Preprocessmaker




# import Preprocessmaker

import pickle
import tensorflow as tf
from keras.callbacks import EarlyStopping
from keras.models import Model
from keras.layers import Dense, Input, Dropout, Conv1D, GRU, LSTM, Activation, Bidirectional, TimeDistributed, \
    GlobalAveragePooling1D, GlobalMaxPooling1D, concatenate, SpatialDropout1D, Flatten
from keras.layers.embeddings import Embedding
import matplotlib.pyplot as plt
import seaborn as sns


# import Preprocessmaker

class CustomModel:

    def __init__(self, vocab_length=64052, maxlen=100):
        self.maxlen = maxlen
        self.vocab_length = vocab_length
        self.model = None
        self.history = None
        self.earlystop = EarlyStopping(monitor='val_loss', min_delta=0, patience=3, verbose=0, mode='auto')

    def construct_custom_model(self, input_shape):

        input = Input(shape=input_shape, dtype='int32')
        embeddings = Embedding(self.vocab_length, output_dim=32, input_length=self.maxlen, mask_zero=True)(input)
        X = Bidirectional(LSTM(50, dropout=0.2, recurrent_dropout=0.5, return_sequences=True))(embeddings)
        X = Conv1D(100, 3, activation='relu')(X)
        X = Dropout(0.2)(X)
        X = Conv1D(50, 3, activation='relu')(X)
        X = GRU(50, dropout=0.2, recurrent_dropout=0.5)(X)
        X = Dropout(0.2)(X)
        X = Dense(3, activation='softmax')(X)

        self.model = Model(inputs=input, outputs=X)

        return self.model

    def compile(self):
        self.model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['acc'])

    def summary(self):
        self.model.summary()

    def fit(self, X_train, Y_train, X_test, Y_test, batch_size=64, epochs=20):
        self.history = self.model.fit(x=X_train, y=Y_train, batch_size=batch_size, epochs=epochs,
                                      callbacks=self.earlystop,
                                      verbose=1, validation_data=(X_test, Y_test))

    def get_accuracy(self, X_test, Y_test):
        self.model.evaluate(X_test, Y_test)
        result = []
        result.append(self.history.history['val_acc'][-1])
        result.append(self.history.history['val_loss'][-1])
        return result

    def save_model(self, path, name):
        try:
            self.model.save(path + name)
        except:
            print("Model kaydedilirken bir hata oluştu")

    def load_model(self, path, name):
        try:
            self.model = tf.keras.models.load_model(path + name + ".h5")
        except:
            print("Model Yüklenirken bir hata oluştu")

    def draw_metrics(self, path):

        try:
            acc = self.history.history['acc']
            val_acc = self.history.history['val_acc']
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

            plt.savefig(path + "/acc", dpi=250, bbox_inches='tight')

            plt.figure(figsize=(10, 5))
            plt.plot(epochs, loss, 'r', label='Training loss')
            plt.plot(epochs, val_loss, 'b', label='Validation loss')
            plt.title('Training and validation loss')
            plt.ylabel('Loss')
            plt.legend()

            plt.savefig(path + "/loss", dpi=250, bbox_inches='tight')
        except:
            print("çizimler yapılırken bir hata oluştu")

    def predict(self, text, embedding_path):

        test_data = self.clean_forPredict(text)

        with open(embedding_path, 'rb') as handle:
            reloaded_tokenizer = pickle.load(handle)
            classes = {0: 'Olumsuz', 1: 'Olumlu', 2: 'Kufurlu'}

            test_data = reloaded_tokenizer.texts_to_sequences(test_data)
            test_data = pad_sequences(test_data, maxlen=self.maxlen, padding='post')
            return str(classes[np.argmax(self.model.predict(test_data))])

    def clean_forPredict(self, text):
        test_data = np.array([text])
        df = pd.DataFrame(test_data, columns=['text'])
        prep = Preprocessmaker(df)
        df = prep.preprocess(df)
        return df['text'].to_numpy()