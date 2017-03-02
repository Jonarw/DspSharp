namespace Filter.AudioSource
{
    public interface IAudioSource
    {
        int BlockSize { get; }
        event BufferCompletedEventHandler BufferCompleted;
    }
}