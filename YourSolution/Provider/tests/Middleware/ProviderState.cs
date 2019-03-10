namespace tests.Middleware{
    public class ProviderState
    {

        /// <summary>
        /// Name of the Consumer  requesting the state change
        /// </summary>
        /// <value></value>
        public string Consumer{get;set;}

        /// <summary>
        /// stores the state we want the provider to be in
        /// </summary>
        /// <value></value>
        public string State{get;set;}
    }
}