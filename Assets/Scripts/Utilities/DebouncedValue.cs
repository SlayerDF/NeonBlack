using System;

public class DebouncedValue<T> where T : struct
{
    private readonly float debounceTime;
    private readonly Action<T> onDebounceComplete;
    private readonly Func<T> valueProvider;

    private bool isDebouncing;
    private T lastInvokedValue;
    private T lastValue;
    private float timer;

    public DebouncedValue(Func<T> valueProvider, Action<T> onDebounceComplete, float debounceTime)
    {
        this.valueProvider = valueProvider;
        this.onDebounceComplete = onDebounceComplete;
        this.debounceTime = debounceTime;

        lastValue = valueProvider.Invoke();
    }

    public void Update(float deltaTime)
    {
        var currentValue = valueProvider.Invoke();

        if (!currentValue.Equals(lastValue))
        {
            timer = 0f;
            lastValue = currentValue;
            isDebouncing = true;
        }
        else if (isDebouncing && (timer += deltaTime) >= debounceTime)
        {
            isDebouncing = false;

            if (currentValue.Equals(lastInvokedValue))
            {
                return;
            }

            lastInvokedValue = currentValue;
            onDebounceComplete.Invoke(currentValue);
        }
    }
}
