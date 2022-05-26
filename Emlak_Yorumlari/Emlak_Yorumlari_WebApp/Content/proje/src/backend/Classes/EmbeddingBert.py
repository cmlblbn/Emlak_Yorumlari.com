
import numpy as np

from sklearn import preprocessing
from transformers import BertTokenizerFast
from sklearn.model_selection import train_test_split


class EmbeddingBert:

    def prepare_training(self, df, test_size=0.15):
        self.raw_docs_train = df["text"].values
        self.sentiment_train = df['sentiment'].values
        X_train, X_test, Y_train, Y_test = train_test_split(self.raw_docs_train, self.sentiment_train,
                                                            stratify=self.sentiment_train,
                                                            random_state=42,
                                                            test_size=test_size, shuffle=True)

        return X_train, X_test, Y_train, Y_test

    def tokenize_forBert(self, data, max_len=100):
        tokenizer = BertTokenizerFast.from_pretrained('bert-base-uncased', do_lower_case=True)
        input_ids = []
        attention_masks = []
        for i in range(len(data)):
            encoded = tokenizer.encode_plus(
                data[i],
                add_special_tokens=True,
                max_length=max_len,
                padding='max_length',
                return_attention_mask=True,
                truncation=True
            )
            input_ids.append(encoded['input_ids'])
            attention_masks.append(encoded['attention_mask'])

        return np.array(input_ids), np.array(attention_masks)

    def y_tokenize(self, Y_train, Y_test):
        ohe = preprocessing.OneHotEncoder()
        Y_train = ohe.fit_transform(np.array(Y_train).reshape(-1, 1)).toarray()
        Y_test = ohe.fit_transform(np.array(Y_test).reshape(-1, 1)).toarray()
        return Y_train, Y_test


