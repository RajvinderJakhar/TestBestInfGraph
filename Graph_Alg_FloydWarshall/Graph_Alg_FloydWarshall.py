import sys
import datetime

_st = datetime.datetime.now()
_CL = 0
_dcl = 0

def best_influencer(n, m, s, links, inf_vals):
    global _CL
    global _dcl
    ret = [0 for _ in range(s)]
    graph = []
    for i in range(n):
        row = []
        for j in range (n):
            if i != j :
                row.append(float('inf'))
            else:
                row.append(0)
        graph.append(row)
    
    for l in range (m):
        i = links[l][0]   
        j = links[l][1]
        graph[i][j] = links[l][2]
    # Find shortest distance from all points to all points
    for h in range(n): 
        for i in range(n):
            for j in range(n):
                _dcl += 1
                graph[i][j] = min(graph[i][j], graph[i][h] + graph[h][j])

    # Find user with maximum influence and smallest id for each hour
    for h in range(0,s):
        influence = []
        for i in range(0,n):
            total_inf = 0
            for j in range(0,n):
                _CL += 1
                if i!=j:
                    total_inf += max(0,(inf_vals[h][i]-graph[i][j]))
            influence.append(total_inf)
        
        id = [i for i, j in enumerate(influence) if j == max(influence)] 
        ret[h] = min(id)
    
    return ret
 

# Set SUBMIT_TO_SZKOPUL=True when submitting
# your solution to the Szkopul webserver.
# Set SUBMIT_TO_SZKOPUL=False in order
# to test your code by reading the input from
# a test file ("input.txt").
SUBMIT_TO_SZKOPUL = False

if SUBMIT_TO_SZKOPUL:
    reader = sys.stdin
else:
    reader = inputReader = open("input4.txt","r")
 
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
print(f"\tDistence depth loop called: {_dcl}")
print(f"\tDepth loop called: {_CL}")
print()