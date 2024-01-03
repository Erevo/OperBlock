namespace OperBlock.Modes
{
    public abstract class OperModeBase : IOperMode
    {
        public Lamp[] Lamps { get; private set; }

        protected OperModeBase(Lamp[] lamps)
        {
            Lamps = lamps;
        }
        
        public virtual void Start()
        {
            foreach (var lamp in Lamps)
            {
                lamp.ToggleOn(0f);
            }
        }

        public virtual void Stop()
        {
            foreach (var lamp in Lamps)
            {
                lamp.ToggleOff();
            }
        }

        public virtual void Tick()
        {
        }
    }
}