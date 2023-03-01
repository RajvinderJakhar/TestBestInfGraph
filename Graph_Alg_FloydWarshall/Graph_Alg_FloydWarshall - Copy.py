
from math import inf
import sys
import datetime

_st = datetime.datetime.now()
_CL = 0

def floydWarshall(graph,n):
    global _CL
    final = list(map(lambda i: list(map(lambda j: j, i)), graph))
    for h in range(n): 
        for i in range(n):
            for j in range(n):
                _CL += 1
                final[i][j] = min(final[i][j],
                                 final[i][h] + final[h][j]
                                 )
    print(f"floydWarshall depth loop called: {_CL}")
    return final

def best_influencer(n, m, s, links, inf_vals):
    global _CL
    _CL = 0
    adj_graph = [[inf]*n  for _ in range(0 , n) ]
    for i in range(0 , n):
        adj_graph[i][i] = 0
    ret = []
    for ind in range (0,m):
        i = links[ind][0]   
        j = links[ind][1]
        adj_graph[i][j] = links[ind][-1]
    
        
    adj_graph = floydWarshall(adj_graph,n)
    for i in range(0,s):
        bi,ui = 0,0
        for j in range(0,n):
            sum = 0
            for k in range(0,n):
                _CL += 1
                if k!=j:
                    sum += max(0,(inf_vals[i][j]-adj_graph[j][k]))
            if sum > ui :
                ui = sum
                bi = j          
                
        ret.append(bi)
    
    return ret
 

# Set SUBMIT_TO_SZKOPUL=True when submitting
# your solution to the Szkopul webserver.
# Set SUBMIT_TO_SZKOPUL=False in order
# to test your code by reading the input from
# a test file ("input.txt").
SUBMIT_TO_SZKOPUL = True

if SUBMIT_TO_SZKOPUL:
    reader = sys.stdin
else:
    reader = inputReader = open("input.txt","r")
 
# Reads the input
astr = reader.readline().split()
n=int(astr[0])
m=int(astr[1])
s=int(astr[2])
reader.readline()
links = [[int(val) for val in reader.readline().split()] for _ in range(m)]
reader.readline()
inf_vals = [[int(val) for val in reader.readline().split()] for _ in range(s)]

# Calls your function
ret = best_influencer(n, m, s, links, inf_vals)

# Writes the output
#for i in range(s):
#    print(ret[i])
    
_et = datetime.datetime.now()
print(f"\t Executing time: {_et - _st}")
print()
_output = ""
# Writes the output
for i in range(s):
    if (i == 0) or ((i % 10) == 0):
        print(_output)
        _output = ""
    _output += f"\t{ret[i]}"
    #_output += f"\t {ret[i]}"
  
print(_output)
print()
print(f"\tDepth loop called: {_CL}")
print()