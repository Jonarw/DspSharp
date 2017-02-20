namespace Filter.AudioSource
{
    public interface IAudioSource
    {
        event BufferCompletedEventHandler BufferCompleted;
        int BlockSize { get; }
    }
}
