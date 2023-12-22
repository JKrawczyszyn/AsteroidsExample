using System;

public class DelayedUpdater : IDisposable
{
    private Action _action;
    private float _delayTime;
    private float _currentTime;

    public void Init(Action action, float delayTime)
    {
        _action = action;
        _delayTime = delayTime;

        _currentTime = 0;
    }

    public void Update(float deltaTime)
    {
        _currentTime += deltaTime;

        if (_currentTime < _delayTime)
            return;

        _action();

        _currentTime = 0;
    }

    public void Dispose()
    {
        _action = null;
    }
}
