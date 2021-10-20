using DIDAStorage;
using Google.Protobuf.Collections;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class StorageNodeLogic : IDIDAStorage
    {
        // Must be a queue instead of a list, in order to pop old values
        private Dictionary<string, List<DIDAStorage.DIDARecord>> storage = new Dictionary<string, List<DIDAStorage.DIDARecord>>();
        private int replicaId = 1;

        public DIDAStorage.DIDARecord Read(string id, DIDAStorage.DIDAVersion version)
        {
            Console.WriteLine("Reading... " + id);
            lock (this) {
                List<DIDAStorage.DIDARecord> recordValues;
                DIDAStorage.DIDARecord value;

                // Check if the version has -1 values

                if (storage.TryGetValue(id, out recordValues))
                {
                    /*
                    foreach (DIDAStorage.DIDARecord rec in recordValues)
                    {
                        // Get the most recent or the indicated version
                    }
                    */
                    value = recordValues[0];
                    return value;
                } else
                {
                    // No value in this record
                }
                throw new NotImplementedException();
            }
        }

        public DIDAStorage.DIDAVersion UpdateIfValueIs(string id, string oldvalue, string newvalue)
        {
            throw new NotImplementedException();
        }

        public DIDAStorage.DIDAVersion Write(string id, string val)
        {
            DIDAStorage.DIDAVersion didaVersion;
            Console.WriteLine("Writing... " + id + " - " + val);
            lock (this)
            {
                // Get the greater version
                didaVersion = new DIDAStorage.DIDAVersion
                {
                    replicaId = replicaId,
                    versionNumber = 1
                };

                DIDAStorage.DIDARecord didaRecord = new DIDAStorage.DIDARecord
                {
                    id = id,
                    version = didaVersion,
                    val = val
                };
                if (storage.ContainsKey(id))
                {
                    var l = storage[id];
                    l.Add(didaRecord);
                } else
                {
                    storage.Add(id, new List<DIDAStorage.DIDARecord>());
                    storage[id].Add(didaRecord);
                }
            };

            return didaVersion;
        }

        public PopulateReply PopulateSerialize(RepeatedField<KeyValuePair> keyValuePairs)
        {
            foreach (KeyValuePair pair in keyValuePairs)
            {
                DIDAStorage.DIDAVersion version;
                version.replicaId = replicaId;
                version.versionNumber = 1;

                DIDAStorage.DIDARecord record;
                record.id = pair.Key;
                record.val = pair.Value;
                record.version = version;

                List<DIDAStorage.DIDARecord> listRecords = new List<DIDAStorage.DIDARecord>();
                listRecords.Add(record);
                storage.Add(pair.Key, listRecords);

                Console.WriteLine("Stored!! Key: " + pair.Key + " Value: " + storage[pair.Key][0].val + " Id: " + storage[pair.Key][0].id + " Version Number: " + storage[pair.Key][0].version.versionNumber + " Replica Id: " + storage[pair.Key][0].version.replicaId);
            }

            return new PopulateReply { Okay = true };
        }

        public PopulateReply Populate(RepeatedField<KeyValuePair> keyValuePairs) { return PopulateSerialize(keyValuePairs); }

        public List<DIDAStorage.DIDARecord> Dump()
        {
            List<DIDAStorage.DIDARecord> data = new List<DIDAStorage.DIDARecord>();
            foreach (string key in storage.Keys)
            {
                var records = storage[key];
                foreach (DIDAStorage.DIDARecord record in records)
                {
                    data.Add(record);
                }
            }
            return data;
        }
    }
}
