namespace DspSharp.AudioSource
{
    public interface IAudioSource
    {
        int BlockSize { get; }
        event BufferCompletedEventHandler BufferCompleted;
    }
}