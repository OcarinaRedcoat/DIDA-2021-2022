How to run the project:

	- Every file in relation to the puppet master (script, populate data, and clients files) have to be in the folder "Scripts"
	- Also we assume that the PuppetMaster knows the IP and Port of all the known PCS's so the url of those pcs have to be placed in the file "pcs.txt" which in the folder "Scripts" 
		eg of the content of pcs.txt "http://localhost:10000" or "http://192.168.1.92:10000"
	- All the pcs have to be ran before the PuppetMaster