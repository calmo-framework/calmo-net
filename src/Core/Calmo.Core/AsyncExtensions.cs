namespace System.Threading.Tasks
{
    public static class AsyncExtensions
    {
        public static void FireAndForget(this Task task)
        {
            try
            {
                Task.Run(() => task).ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }
    }
}
