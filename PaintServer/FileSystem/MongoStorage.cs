using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace PaintServer.FileSystem
{
    class MongoStorage
    {
        private MongoUrl mongoUrl;
        private MongoClient client;
        private IMongoCollection<BsonDocument> schamsCollection;
        private IMongoCollection<BsonDocument> lockedCollection;
        public MongoStorage()
        {
            string dbUrl = Environment.GetEnvironmentVariable("MONGODB_URI");
            mongoUrl = new MongoUrl(dbUrl);
            client = new MongoClient(mongoUrl);
            schamsCollection = client.GetDatabase("paint").GetCollection<BsonDocument>("schams");
            lockedCollection = client.GetDatabase("paint").GetCollection<BsonDocument>("locks");
        }

        public bool IsFileLocked(string filename,string clientId)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("fileName", filename);
            BsonDocument doc = lockedCollection.Find(filter).FirstOrDefault();
            if (doc == null) { return false; }
            if (doc.Contains("lockedBy"))
            {
                if (clientId != doc["lockedBy"].AsString) { return true; };
            }
            return false;
        }

        public void saveFile(string filename,string jsonData,string clientId)
        {
            BsonDocument lockDoc = new BsonDocument
            {
                {"fileName", filename },
                {"lockedBy", clientId }
            };
            BsonDocument schamDoc = new BsonDocument
            {
                {"fileName", filename },
                {"jsonData", BsonDocument.Parse(jsonData) }
            };
            FilterDefinition<BsonDocument> lockFilter = Builders<BsonDocument>.Filter.Eq("fileName", filename);
            FilterDefinition<BsonDocument> schamFilter = Builders<BsonDocument>.Filter.Eq("fileName", filename);
            lockedCollection.ReplaceOne(lockFilter,lockDoc, new ReplaceOptions {IsUpsert = true});
            schamsCollection.ReplaceOne(schamFilter,schamDoc, new ReplaceOptions { IsUpsert = true});
        }

        public void closeFile(string filename,string clientId)
        {
            if (!IsFileLocked(filename, clientId)) { return; }
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("fileName", filename);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Unset("lockedBy");
            lockedCollection.UpdateOne(filter, update);

        }

        public string openFile(string filename,string clientId)
        {
            if (IsFileLocked(filename,clientId)) { return "locked"; }
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("fileName", filename);
            BsonDocument doc = schamsCollection.Find(filter).FirstOrDefault();
            if (doc == null) { return "does not exist"; }
            BsonDocument jsonData = doc["jsonData"].AsBsonDocument;

            lockFile(filename, clientId);

            return jsonData.ToString();

        }

        public void lockFile(string filename,string clientId)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("fileName", filename);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("lockedBy", clientId);
            lockedCollection.UpdateOne(filter, update);
        }
        public List<string> GetAllFileNames()
        {
            ProjectionDefinition<BsonDocument> projection = Builders<BsonDocument>.Projection.Include("fileName").Exclude("_id");
            List<BsonDocument> docs = schamsCollection.Find(new BsonDocument()).Project(projection).ToList();

            List<string> fileNames = new List<string>();
            foreach (BsonDocument doc in docs)
            {
                if (doc.Contains("fileName"))
                {
                    fileNames.Add(doc["fileName"].AsString);
                }
            }

            return fileNames;
        }

    }
}
