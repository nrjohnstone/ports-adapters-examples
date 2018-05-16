namespace Adapter.Persistence.CouchDb.Views
{
    public interface ICouchDbView
    {
        string Id { get; }
        string Json();
    }
}