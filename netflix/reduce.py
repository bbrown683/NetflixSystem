import os
import sys
import random
if len(sys.argv)<2:
	sys.exit("Usage:reduce.py reduction-percent")
r = float(sys.argv[1])
users = []
for i in range(0,18000,1):
	if random.random()<r:
		users.append(i)
ifile = open("TrainingRatings.txt")
ofile = open("TrainingRatings-new.txt",'w')
for l in ifile:
	p = l.strip().split(",")
	if int(p[0]) not in users:
		continue
	ofile.write(l)
ofile.close()

ifile = open("TestingRatings.txt")
ofile = open("TestingRatings-new.txt",'w')
for l in ifile:
	p = l.strip().split(",")
	if int(p[0]) not in users:
		continue
	ofile.write(l)
ofile.close()
	
	