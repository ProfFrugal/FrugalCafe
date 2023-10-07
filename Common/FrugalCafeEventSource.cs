using System.Diagnostics.Tracing;

namespace FrugalCafe
{
    public class FrugalCafeEventSource : EventSource
    {
        public static readonly FrugalCafeEventSource Instance = new FrugalCafeEventSource();

        public FrugalCafeEventSource() : base("FrugalCafe")
        {
        }

        [Event(1, Opcode = EventOpcode.Start)]
        public void Start(string task) 
        { 
            base.WriteEvent(1, task); 
        }

        [Event(2, Opcode = EventOpcode.Stop)]
        public void Stop(string task) 
        { 
            base.WriteEvent(2, task); 
        }

        [Event(3)]
        public void Message(string message) 
        { 
            base.WriteEvent(3, message); 
        }

        [Event(4)]
        public void MessageWithInt(int count, string message)
        {
            base.WriteEvent(4, count, message);
        }
    }
}
