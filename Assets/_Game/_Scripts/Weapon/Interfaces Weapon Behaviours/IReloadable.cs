using System;

public interface IReloadable
{
    event Action<float> OnReload;
    event Action<int, int> OnAmmoChanged;
    float ReloadTime { get; }
    void Reload();
}