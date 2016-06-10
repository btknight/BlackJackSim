using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    public class ChipStack
    {
        public int Value { get; protected set; }

        public ChipStack(int value)
        {
            Value = value;
        }

        public static ChipStack operator +(ChipStack stack1, ChipStack stack2)
        {
            return new ChipStack(stack1.Value + stack2.Value);
        }

        public static ChipStack operator -(ChipStack stack1, ChipStack stack2)
        {
            return new ChipStack(stack1.Value - stack2.Value);
        }

        public ChipStack RemoveChips(int value)
        {
            if (Value < value) { return null; }
            Value -= value;
            return new ChipStack(value);
        }

        public void AddChips(ChipStack Stack)
        {
            Value += Stack.Value;
            Stack.Value = 0;
        }
    }
}
