

public interface IGameEventListener
{
	void OnEventInvoked();
}

public interface IGameEventListener<T>
{
	void OnEventInvoked(T value);
}