from transformers import BertTokenizerFast
import numpy as np
tokenizer = BertTokenizerFast.from_pretrained('bert-base-uncased', do_lower_case=True)
maxlen = 100

def tokenize(data,max_len=maxlen) :

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

    return np.array(input_ids),np.array(attention_masks)