using System.Collections;

namespace OperBlock.Modes
{
    public interface IOperMode
    {
        public Lamp[] Lamps { get; }

        public void Start();
        public void Stop();
        
        public void Tick();
    }
}