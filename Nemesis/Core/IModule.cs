namespace Nemesis.Core
{
    internal interface IModule
    {
        string Name { get; }
        void Initialize();
        void Shutdown();
        void OnUpdate();
        void OnGUI();
    }
}
