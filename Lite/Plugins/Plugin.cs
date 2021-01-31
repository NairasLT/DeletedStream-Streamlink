using System.Threading.Tasks;

public interface IPlugin
{
    Task Run();
    Task RunInfinite();
}