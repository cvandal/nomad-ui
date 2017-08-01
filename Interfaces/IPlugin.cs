namespace Nomad.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }

        void Do();
    }
}
