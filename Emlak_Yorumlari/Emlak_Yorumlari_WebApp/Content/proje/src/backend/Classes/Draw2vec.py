from backend.Classes.Preprocess import Preprocessmaker
from scipy.stats import entropy
import psycopg2
import datetime
from keras.preprocessing.text import Tokenizer
import nltk
import matplotlib.pyplot as plt
import seaborn as sns
import pandas as pd
import uuid
import os
import numpy as np


def draw2Vec(df=None, isTrained=1):
    if df is None and isTrained == 1:
        conn = psycopg2.connect(
            database="MyDataBase", user='postgres', password='123321', host='127.0.0.1', port='5432'
        )
        # Creating a cursor object using the cursor() method
        cur = conn.cursor()
        sqlQuery = pd.read_sql_query(
            """SELECT text,actual_sentiment FROM dbo."Embeddings" WHERE \"isTrained" = TRUE ORDER BY embedding_id ASC""",
            conn)
        sqldf = pd.DataFrame(sqlQuery)
        sqldf.rename(columns={'actual_sentiment': 'sentiment'}, inplace=True)
        conn.close()
        path_name = make_dir("trained")
    elif df is None and isTrained == 0:
        conn = psycopg2.connect(
            database="MyDataBase", user='postgres', password='123321', host='127.0.0.1', port='5432'
        )
        # Creating a cursor object using the cursor() method
        cur = conn.cursor()
        sqlQuery = pd.read_sql_query("""SELECT text,actual_sentiment FROM dbo."Embeddings" ORDER BY embedding_id ASC""",
                                     conn)
        sqldf = pd.DataFrame(sqlQuery)
        sqldf.rename(columns={'actual_sentiment': 'sentiment'}, inplace=True)
        conn.close()
        path_name = make_dir("nonTrained")
    else:
        path_name = make_dir()
        sqldf = df
    prep = Preprocessmaker()
    df = prep.preprocess(sqldf)

    trained_path = ["backend/Classes/Embedding/trained/" + path_name + "/swear", "backend/Classes/Embedding/trained/" + path_name + "/negative",
                    "backend/Classes/Embedding/trained/" + path_name + "/positive", "backend/Classes/Embedding/trained/" + path_name + "/embedding",
                    "backend/Classes/Embedding/trained/" + path_name + "/classes"]
    nonTrained_path = ["backend/Classes/Embedding/nonTrained/" + path_name + "/swear", "backend/Classes/Embedding/nonTrained/" + path_name + "/negative",
                       "backend/Classes/Embedding/nonTrained/" + path_name + "/positive", "backend/Classes/Embedding/nonTrained/" + path_name + "/embedding",
                       "backend/Classes/Embedding/nonTrained/" + path_name + "/classes"]
    if (isTrained == 1):
        wordCount = DrawEmbedding(df, trained_path[0], trained_path[1], trained_path[2], trained_path[3], trained_path[4])
    elif (isTrained == 0):
        wordCount = DrawEmbedding(df, nonTrained_path[0], nonTrained_path[1], nonTrained_path[2], nonTrained_path[3],
                      nonTrained_path[4])
    return path_name,wordCount

def DrawEmbedding(df, swear_path, negative_path, positive_path, embedding_path, classes_path):
    swearWordCount = nltk.FreqDist(
        word for text in df[df["sentiment"] == 2]["text"] for word in text.lower().split()).most_common(40)
    swear_fdist = pd.Series(dict(swearWordCount))
    plt.subplots(figsize=(10, 10))
    sns.barplot(x=swear_fdist.values, y=swear_fdist.index, orient='h')
    plt.xticks(rotation=30)
    plt.savefig(swear_path, dpi=200, bbox_inches='tight')


    positiveWordCount = nltk.FreqDist(
        word for text in df[df["sentiment"] == 1]["text"] for word in text.lower().split()).most_common(40)
    positive_fdist = pd.Series(dict(positiveWordCount))
    plt.subplots(figsize=(10, 10))
    sns.barplot(x=positive_fdist.values, y=positive_fdist.index, orient='h')
    plt.xticks(rotation=30)
    plt.savefig(positive_path, dpi=200, bbox_inches='tight')


    negativeWordCount = nltk.FreqDist(
        word for text in df[df["sentiment"] == 0]["text"] for word in text.lower().split()).most_common(40)
    negative_fdist = pd.Series(dict(negativeWordCount))
    plt.subplots(figsize=(10, 10))
    sns.barplot(x=negative_fdist.values, y=negative_fdist.index, orient='h')
    plt.xticks(rotation=30)
    plt.savefig(negative_path, dpi=200, bbox_inches='tight')


    plt.subplots(figsize=(10, 10))
    df['length'] = [len(x) for x in df['text']]
    sns.kdeplot(data=df, x='length', hue='sentiment', palette='Dark2')
    plt.savefig(classes_path, dpi=200, bbox_inches='tight')


    tokenizer = Tokenizer()
    embedding = df['text'].values
    tokenizer.fit_on_texts(embedding)
    embedding = tokenizer.texts_to_sequences(embedding)

    word_count = []

    for encoded_tweet in embedding:
        word_count.append(len(encoded_tweet))

    parameters = {'axes.labelsize': 20,
                  'axes.titlesize': 30}
    #
    plt.rcParams.update(parameters)
    fig, ax1 = plt.subplots(1, 1)
    fig.set_size_inches(18.5, 8)
    sns.histplot(word_count, palette='Blues', stat='density', bins=30, ax=ax1)
    sns.kdeplot(word_count, color='red', ax=ax1)
    ax1.set_xlabel('Word count per data')
    ax1.tick_params(axis='x', labelsize=16)
    ax1.tick_params(axis='y', labelsize=16)
    ax1.set_ylabel("")
    ax1.set_title("Data length distribution", color="#292421")
    fig.tight_layout(pad=2.0)
    plt.rcParams.update(parameters)
    plt.savefig(embedding_path, dpi=250, bbox_inches='tight')

    return word_count

def make_dir(half_path = "trained"):
    parenth_dir = "backend/Classes/Embedding/" + half_path #backend/Classes/Embedding
    mkdir_name = str(uuid.uuid4())
    path = os.path.join(parenth_dir,mkdir_name)
    print(path)
    os.mkdir(path)
    return mkdir_name

def drawmaker():
    trained_pathname,p = draw2Vec(None, 1)
    nontrained_pathname,q = draw2Vec(None, 0)
    difference = abs(len(p) - len(q))
    if len(p) > len(q):
        for i in range(difference):
            q.append(0)
    elif len(q) > len(p):
        for i in range(difference):
            p.append(0)
    kl_divergence = entropy(p, q)

    conn = psycopg2.connect(
        database="MyDataBase", user='postgres', password='123321', host='127.0.0.1', port='5432'
    )
    # Creating a cursor object using the cursor() method
    cur = conn.cursor()
    sql = ''' INSERT INTO dbo."Embedding_Analyse"(\"lastAnalyseDate",\"isActive",\"kl_divergenceValue",trained_path,nontrained_path)
                 VALUES(%s,%s,%s,%s,%s) '''
    createdOn = datetime.datetime.today()
    entry = (createdOn, True, kl_divergence,str(trained_pathname),str(nontrained_pathname))
    cur.execute(sql, entry)
    conn.commit()
    conn.close()
    return True