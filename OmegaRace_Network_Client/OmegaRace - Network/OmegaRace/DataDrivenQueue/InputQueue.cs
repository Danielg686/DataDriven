using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace
{
    class InputQueue
    {

        private static InputQueue instance = null;
        public static InputQueue Instance()
        {
            if (instance == null)
            {
                instance = new InputQueue();
            }
            return instance;
        }

        Queue<DataMessage> inputQueue;

        private InputQueue()
        {
            inputQueue = new Queue<DataMessage>();

        }

        public static void AddToQueue(DataMessage msg)
        {
            instance.inputQueue.Enqueue(msg);

        }

        public static void Process()
        {
            while(instance.inputQueue.Count > 0)
            {
                DataMessage msg = instance.inputQueue.Dequeue();

                if (msg.sendType == SEND_TYPE.LOCAL)
                {
                    OutputQueue.AddToQueue(msg);
                }
                else
                {
                    MyClient.Instance.SendMessageToServer(msg);
                }
            }


        }

    }
}

