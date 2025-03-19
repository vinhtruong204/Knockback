using System;

public interface IReloadable
{
    event Action<float> OnReload;
    float ReloadTime { get; }
    void Reload();
}