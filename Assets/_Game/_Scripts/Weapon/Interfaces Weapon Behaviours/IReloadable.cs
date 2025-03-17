using System;

public interface IReloadable
{
    event Action OnReload;
    float ReloadTime { get; }
    void Reload();
}