namespace AzureBackend
{
    public class DbConnectionString
    {
        public bool InMemory { get; set; }
        public string DbName { get; set; }
        public string DomainConnectionString { get; set; }
    }
}