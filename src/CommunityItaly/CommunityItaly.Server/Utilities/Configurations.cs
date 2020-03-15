namespace CommunityItaly.Server
{
    public class CosmosDbConnections
    {
        public string AccountEndpoint { get; set; }
        public string AccountKey { get; set; }
        public string DatabaseName { get; set; }
    }

    public class BlobStorageConnections
    {
        public string ConnectionString { get; set; }
    }
}
