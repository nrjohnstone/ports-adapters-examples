namespace Adapter.Persistence.Test
{
    public class PersistenceAdapter
    {
        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;
        }

        public void Register()
        {
            if (!_initialized)
                throw new AdpaterNotInitializedException();
        }
    }
}