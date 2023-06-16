namespace PboSpy.Modules.StatusBar;

public interface IStatusBarManager
{
    void Reset();
    void SetStatus(string message, string details = "");
    void SetTemporaryStatus(string message, string details = "", int duration = 1000);
}
