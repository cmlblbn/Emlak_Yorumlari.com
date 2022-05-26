from backend.Classes.BertModel import BertModel
from backend.Classes.EmbeddingBert import EmbeddingBert
from backend.Classes.Preprocess import Preprocessmaker

from backend.Classes.CustomModel import CustomModel
from backend.Classes.EmbeddingCustom import EmbeddingCustom
import psycopg2
import datetime
import os
import pandas as pd


def Factory(type, maxlen=100, batch_size=64, epoch=15):
    maxlen = int(maxlen)
    batch_size = int(batch_size)
    epoch = int(epoch)
    if type == "custom":
        TrainerCustom(maxlen, batch_size, epoch)
    elif type == "bert":
        TrainerBert(maxlen, batch_size, epoch)
    else:
        print("Büyük bir problem algılandı, eğitim iptal ediliyor.")


def TrainerCustom(maxlen, batch_size, epoch):
    # get data from database, when database implemented, write here
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

    df = sqldf
    path = makedirectory()
    pathList = str(path).split("/")
    modelName = pathList[-1]

    prep = Preprocessmaker(df)
    df = prep.preprocess(df)
    embed = EmbeddingCustom(df)
    X_train, X_test, Y_train, Y_test = embed.prepare_training()
    X_train, X_test = embed.tokenize_forCustom(X_train, X_test, path, "/embedding")
    Y_train, Y_test = embed.y_tokenize(Y_train, Y_test)
    vocab_length = embed.get_vocab_length()
    cus_model = CustomModel(vocab_length)
    cus_model.construct_custom_model((maxlen,))
    cus_model.summary()
    cus_model.compile()
    cus_model.fit(X_train, Y_train, X_test, Y_test, batch_size, epoch)

    evaluate = cus_model.get_accuracy(X_test, Y_test)
    acc = evaluate[0]
    loss = evaluate[1]
    strpath = str(path)
    cus_model.save_model(strpath, "/CustomModel.h5")

    strpath = strpath + "/"
    statistic = "statistic"
    stapath = os.path.join(strpath, statistic)
    os.mkdir(stapath)
    cus_model.draw_metrics(stapath)

    type = "custom"
    realEpoch = len(cus_model.history.history['val_loss'])
    createdOn = datetime.datetime.today()
    isActive = False

    conn = psycopg2.connect(
        database="MyDataBase", user='postgres', password='123321', host='127.0.0.1', port='5432'
    )
    # Creating a cursor object using the cursor() method
    cur = conn.cursor()
    sql = ''' INSERT INTO dbo."Models"(\"modelName",type,\"Accuracy","loss","batch_size","epoch","maxlen",\"createdOn",\"isActive")
                VALUES(%s,%s,%s,%s,%s,%s,%s,%s,%s) '''

    model2database = (modelName, type, acc, loss, batch_size, realEpoch, maxlen, createdOn, isActive)
    cur.execute(sql, model2database)
    conn.commit()
    sql = ''' UPDATE dbo."Embeddings" SET "isTrained" = TRUE WHERE "isTrained" = FALSE'''
    cur.execute(sql)
    conn.commit()
    conn.close()



def TrainerBert(df, maxlen, batch_size, epoch):
    # get data from database, when database implemented, write here
    path = makedirectory()
    pathList = str(path).split("/")
    modelName = pathList[-1]

    prep = Preprocessmaker(df)
    df = prep.preprocess(df)
    embed = EmbeddingBert(df)
    X_train, X_test, Y_train, Y_test = embed.prepare_training()
    train_input_ids, train_attention_masks = embed.tokenize_forBert(X_train)
    test_input_ids, test_attention_masks = embed.tokenize_forBert(X_test)
    Y_train, Y_test = embed.y_tokenize(Y_train, Y_test)
    bert_model = BertModel()
    bert_model.construct_custom_model()
    bert_model.compile()
    bert_model.fit(train_input_ids, train_attention_masks, test_input_ids, test_attention_masks, Y_train, Y_test,
                   batch_size, epoch)

    evaluatedData = bert_model.get_accuracy(test_input_ids, test_attention_masks, Y_test)
    acc = evaluatedData[0]
    loss = evaluatedData[1]
    strpath = str(path)
    bert_model.save_model(strpath, "/BertModel.h5")

    strpath = strpath + "/"
    statistic = "statistic"
    stapath = os.path.join(strpath, statistic)
    os.mkdir(stapath)
    bert_model.draw_metrics(stapath)

    type = "custom"
    realEpoch = len(bert_model.history.history['val_loss'])
    createdOn = datetime.datetime.today()
    isActive = False

    conn = psycopg2.connect(
        database="MyDataBase", user='postgres', password='123321', host='127.0.0.1', port='5432'
    )
    # Creating a cursor object using the cursor() method
    cur = conn.cursor()
    sql = ''' INSERT INTO dbo."Models"(\"modelName",type,\"Accuracy","loss","batch_size","epoch","maxlen",\"createdOn",\"isActive")
                VALUES(%s,%s,%s,%s,%s,%s,%s,%s,%s) '''

    model2database = (modelName, type, acc, loss, batch_size, realEpoch, maxlen, createdOn, isActive)
    cur.execute(sql, model2database)
    conn.commit()
    sql = ''' UPDATE dbo."Embeddings" SET "isTrained" = TRUE WHERE "isTrained" = FALSE'''
    cur.execute(sql)
    conn.commit()
    conn.close()


def makedirectory():
    parent_dir = "backend/Models"
    date = datetime.datetime.now()
    mkdir_name = str(date.day) + "-" + str(date.month) + "-" + str(date.year) + "-" + str(date.hour) + "." + str(
        date.minute) + "-Custom"
    print(parent_dir + mkdir_name)
    path = os.path.join(parent_dir, mkdir_name)
    os.mkdir(path)
    return path