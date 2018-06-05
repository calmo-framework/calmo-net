namespace Calmo.Forms
{
    public interface IPlatformFeatures
    {
        void Exit();
        string PackageName { get; }
    }
}