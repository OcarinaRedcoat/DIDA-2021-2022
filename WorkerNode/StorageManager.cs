using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;

namespace WorkerNode
{
    class StorageManager
    {
        // TODO: Implements IDIDAStorage????
        private string url;
        private StorageService.StorageServiceClient client;

        public StorageManager(string url)
        {
            this.url = url;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            client = new StorageService.StorageServiceClient(channel);
        }

        public DIDAWorker.DIDARecord ReadFromStorage(string id, DIDAWorker.DIDAVersion version)
        {
            try
            {
                DIDAVersion grpcVersionInput;
                grpcVersionInput = new DIDAVersion
                {
                    ReplicaId = version.replicaId,
                    VersionNumber = version.versionNumber
                };

                var reply = client.Read(
                    new ReadRequest
                    {
                        Id = id,
                        Version = grpcVersionInput
                    }
                );

                DIDAWorker.DIDAVersion newVersion = new DIDAWorker.DIDAVersion
                {
                    replicaId = reply.Record.Version.ReplicaId,
                    versionNumber = reply.Record.Version.VersionNumber
                };

                DIDAWorker.DIDARecord record = new DIDAWorker.DIDARecord
                {
                    id = reply.Record.Id,
                    version = newVersion,
                    val = reply.Record.Val
                };
                return record;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public DIDAWorker.DIDAVersion WriteToStorage(string id, string value)
        {
            try
            {
                var reply = client.Write(
                    new WriteRequest
                    {
                        Id = id,
                        Val = value
                    }
                );

                DIDAWorker.DIDAVersion newVersion = new DIDAWorker.DIDAVersion
                {
                    replicaId = reply.Version.ReplicaId,
                    versionNumber = reply.Version.VersionNumber
                };

                return newVersion;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        // TODO: Update if value is
    }
}
