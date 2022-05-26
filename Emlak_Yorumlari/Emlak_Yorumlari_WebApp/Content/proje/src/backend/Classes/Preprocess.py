import numpy as np
import pandas as pd
import re
import sqlite3



class Preprocessmaker:

    def __init__(self, df=None):
        self.df = df

    def remove_spec_chars(self, df):
        spec_chars = ["!", '"', "#", "%", "&", "'", "(", ")",
                      "*", "+", ",", "-", ".", "/", ":", ";", "<",
                      "=", ">", "<b>", "<b>", "<br />", "\\n", "?", "@", "[", "\\", "]", "^", "_",
                      "`", "{", "|", "}", "~", "–"]
        for char in spec_chars:
            df['text'] = df['text'].str.replace(char, ' ')
        return df

    def get_stopwords_list(self, stop_file_path):
        """load stop words """

        with open(stop_file_path, 'r', encoding="utf-8") as f:
            stopwords = f.readlines()
            stop_set = set(m.strip() for m in stopwords)
            return list(frozenset(stop_set))

    def delete_stopwords(self, df, stop_words):
        df['text'] = df['text'].apply(lambda x: ' '.join([word for word in x.split() if word not in (stop_words)]))
        return df

    def remove_numbers(self, df, label):
        """ This function removes numbers from a text
            inputs:
            - text """
        df[label] = df[label].apply(lambda x: re.sub(r"\d+", " ", x))
        return df

    def clean_eol_tabs(self, df, label):
        """ text lowercase
            removes \n
            removes \t
            removes \r """
        df[label] = df[label].str.lower()
        df[label] = df[label].apply(lambda x: x.replace("\n", " "))
        df[label] = df[label].apply(lambda x: x.replace("\r", " "))
        df[label] = df[label].apply(lambda x: x.replace("\t", " "))
        return df

    def more_cleaning(self, df, label):
        """ This function
        1) removes remaining one-letter words and two letters words
        2) replaces multiple spaces by one single space
        3) drop empty lines """

        df[label] = df[label].apply(lambda x: re.sub(r'\b\w{1,2}\b', " ", x))
        df[label] = df[label].apply(lambda x: re.sub(r"[ \t]{2,}", " ", x))
        df[label] = df[label].apply(lambda x: x if len(x) != 1 else '')
        df[label] = df[label].apply(lambda x: np.nan if x == '' else x)
        df = df.dropna(subset=[label], axis=0).reset_index(drop=True).copy()
        return df

    def get_df(self):
        self.conn = sqlite3.connect(':memory:')
        sqlQuery = pd.read_sql_query(''' SELECT text,sentiment FROM deneme ''', self.conn)
        self.df = pd.DataFrame(sqlQuery, columns=['text', 'sentiment'])

    def preprocess(self, df):
        """prepare data to training and testing"""

        df = self.remove_spec_chars(df)
        stop_words = self.get_stopwords_list("backend/datasets/turkish.txt")
        df = self.delete_stopwords(df, stop_words)
        df = self.remove_numbers(df, "text")
        df = self.clean_eol_tabs(df, "text")
        df = self.more_cleaning(df, "text")
        return df

