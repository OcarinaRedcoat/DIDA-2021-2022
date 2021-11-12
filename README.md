## DAD 20/21

Group 06 (87704, 87699, 89559)

## How to run the project:

	- Every file in relation to the puppet master (script, populate data, and clients files) have to be in the folder "Scripts"
	- Also we assume that the PuppetMaster knows the IP and Port of all the known PCS's so the url of those pcs have to be placed in the file "pcs.txt" which in the folder "Scripts" 
		eg of the content of pcs.txt "http://localhost:10000" or "http://192.168.1.92:10000"
	- All the pcs have to be ran before the PuppetMaster

## Smaple Known PCS's: (pcs.txt)

```
http://localhost:10000
```

## Running the defaults files: (PuppetMasterCLI)
```
y
sample_script
exit
```

### Sample Client App: (ManyUpdatesApp)

```
operator UpdateAndChainOperator 0
operator UpdateAndChainOperator 1
operator UpdateAndChainOperator 2
operator UpdateAndChainOperator 3
operator UpdateAndChainOperator 4
```

### Sample Client App: (ManyWritesApp)

```
operator IncrementOperator 0
operator IncrementOperator 1
operator IncrementOperator 2
operator IncrementOperator 3
```

### Sample Populate file: (data_load)

```
13,1
14,one_ahead_of_13
23,11
24,one_ahead_of_23
```

### Sample Script: (sample_script)

```
debug
scheduler sc1 http://localhost:5645
storage s1 http://localhost:3000 2000
storage s2 http://localhost:3001 2000
storage s3 http://localhost:3002 2000
worker w1 http://localhost:4001 0
worker w2 http://localhost:4002 0
wait 2000
status
listServer s1
listGlobal
populate data_load
wait 1000
client 23 ManyUpdatesApp
wait 2000
client 13 ManyWritesApp
client 13 ManyUpdatesApp
wait 2000
status
listServer s1
listGlobal
```

## Test Scripts:
We prepared some test scripts:
```
 - consistency_script
 - failure_tolerance_script
 - load_test_script
 - max_versions_script 
 - sample_script
 - update_tiebreak_script
```