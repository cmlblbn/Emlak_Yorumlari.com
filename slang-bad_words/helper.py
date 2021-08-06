import re











def fib(n):
    if n == 0:
        return 0
    elif n == 1:
        return 1
    else:
        return fib(n-1) + fib(n-2)

def givenNumberclosestfib(number):

    for i in range(number):
        if(fib(i) > number):
            return fib(i)

def getEndtoEndLongestString(string):
    longest = ""
    for i in range(len(string)):
        head = string[i:i+3]
        for j in range(i+4,len(string)):
            tail = string[j:j+3]
            if(head == tail):
                result = string[i:j+3]
                if(len(result)>=len(longest)):
                    longest = result
    return longest



#string = "Benidinleedeyipadanapideyemeyiunutma"
#print(getEndtoEndLongestString(string))

f = open("slangWords.txt","r+",encoding="utf-8")

i = 0
fw = open("slangClean.txt","w+",encoding="utf-8")
for line in f.readlines():
    print(line)
    match = str(i)+":"+"string"
    cleanString = re.sub(match,"",line)
    cleanquot = re.sub("\"","",cleanString)
    fw.write(cleanquot)
    i+=1