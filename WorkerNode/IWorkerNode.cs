using System;

namespace DIDAWorker {
	/*
	public enum OperationType { ReadOp, WriteOp, UpdateIfOp};

	public delegate DIDAStorageNode delLocateStorageId(string id, OperationType type);
	public interface IDIDAOperator {
		public string ProcessRecord(DIDAMetaRecord meta, string input);
		public void ConfigureStorage(DIDAStorageNode[] storageReplicas, delLocateStorageId locationFunction);
		// the location function is passed to the operator so it may know in which storage node to do an operation
		// based on the record id and the operation type.
	}


	public struct DIDARequest {
		public DIDAMetaRecord meta;
		public string input;
		public int next;
		public int chainSize;
		public DIDAAssignment[] chain;
	}


	public struct DIDAAssignment {
		public DIDAOperatorID op;
		public string host;
		public int port;
		public string output;
	}


	public class DIDAMetaRecord {
		public int id;
	}

	public struct DIDAStorageNode {
		public string serverId;
		public string host;
		public int port;
	}

	public struct DIDAOperatorID {
		public string classname;
		public int order;
	}
	public struct DIDARecord
	{
		public string id;
		public DIDAVersion version;
		public string val;
	}

	public struct DIDAVersion
	{
		public int versionNumber;
		public int replicaId;
	}
	
	*/
}
