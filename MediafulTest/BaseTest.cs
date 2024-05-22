namespace MediafulTest
{
    public abstract class BaseTest : IDisposable
    {
        protected Bunit.TestContext Context { get; private set; }

        public BaseTest()
        {
            Context = new();
            Context.AddTestServices();
        }


        public void Dispose()
        {
            try
            {
                Context.Dispose();
            }
            catch(Exception) { }
        }
    }
}
