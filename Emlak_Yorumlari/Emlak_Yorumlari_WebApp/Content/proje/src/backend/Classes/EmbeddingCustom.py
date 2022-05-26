import pickle
import numpy as np

from keras.preprocessing.text import Tokenizer
from keras.preprocessing.sequence import pad_sequences
from sklearn.model_selection import train_test_split


class EmbeddingCustom:

    def __init__(self, df):
        self.raw_docs_train = df["text"].values
        self.sentiment_train = df['sentiment'].values
        self.tokenizer = Tokenizer()

    def prepare_training(self, test_size=0.15):
        X_train, X_test, Y_train, Y_test = train_test_split(self.raw_docs_train, self.sentiment_train,
                                                            stratify=self.sentiment_train,
                                                            random_state=42,
                                                            test_size=test_size, shuffle=True)

        return X_train, X_test, Y_train, Y_test

    def tokenize_forCustom(self, X_train, X_test, path, name, max_len=100):
        self.tokenizer.fit_on_texts(X_train)
        X_train = self.tokenizer.texts_to_sequences(X_train)
        X_test = self.tokenizer.texts_to_sequences(X_test)
        X_train = pad_sequences(X_train, maxlen=max_len, padding='post')
        X_test = pad_sequences(X_test, maxlen=max_len, padding='post')

        # we need to save tokenizer.
        self.save_tokenizer(path, name)

        return X_train, X_test

    def save_tokenizer(self, path, name):
        with open(path + name + '.pickle', 'wb') as handle:
            pickle.dump(self.tokenizer, handle, protocol=pickle.HIGHEST_PROTOCOL)

    def y_tokenize(self, Y_train, Y_test):
        from keras.utils import np_utils
        num_labels = len(np.unique(self.sentiment_train))
        Y_train = np_utils.to_categorical(Y_train, num_labels)
        Y_test = np_utils.to_categorical(Y_test, num_labels)

        return Y_train, Y_test

    def get_vocab_length(self):
        vocab_length = len(self.tokenizer.word_index) + 1
        return vocab_length