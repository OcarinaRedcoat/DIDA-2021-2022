# How to run the project:

	- Every file in relation to the puppet master (script, populate data, and clients files) have to be in the folder "Scripts"
	- Also we assume that the PuppetMaster knows the IP and Port of all the known PCS's so the url of those pcs have to be placed in the file "pcs.txt" which in the folder "Scripts" 
		eg of the content of pcs.txt "http://localhost:10000" or "http://192.168.1.92:10000"
	- All the pcs have to be ran before the PuppetMaster

## Running the defaults files: (PuppetMasterCLI)
```
y
sample_script
exit
```


### Client App: (app1)

```
operator CounterOperator 0
operator CounterOperator 1
operator CounterOperator 2
operator CounterOperator 3
operator CounterOperator 4
operator CounterOperator 5
```

### Populate file: (data)

```
app1_r1,the_data_of_request_1
app1_r2,the_data_of_request_2
app1_r3,the_data_of_request_3
1,0
```

### Sample Script: (sample_script)

```
debug
scheduler sc1 http://localhost:4000 0
storage s1 http://localhost:3000 0
storage s2 http://localhost:3003 0
worker w1 http://localhost:3001 0
worker w2 http://localhost:3002 0
crash s2
populate data
listServer s1
client 1 app1
wait_interval 5000

listGlobal
status
```