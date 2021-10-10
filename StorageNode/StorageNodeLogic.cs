using DIDAStorage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class StorageNodeLogic : IDIDAStorage
    {
        private ConcurrentDictionary<string, string> storage = new ConcurrentDictionary<string, string>();

        DIDAStorage.DIDARecord IDIDAStorage.Read(string id, DIDAStorage.DIDAVersion version)
        {
            throw new NotImplementedException();
            /*string value;
            storage.TryGetValue(id, out value);
            return new DIDAStorage.DIDARecord
            {
                Id = "s1",
                Version = new DIDAVersion
                {
                    VersionNumber = 1,
                    ReplicaId = 2

                },
                Val = value
            };*/
        }

        DIDAStorage.DIDAVersion IDIDAStorage.Write(string id, string val)
        {
            throw new NotImplementedException();
        }

        DIDAStorage.DIDAVersion IDIDAStorage.UpdateIfValueIs(string id, string oldvalue, string newvalue)
        {
            throw new NotImplementedException();
        }
    }
}
