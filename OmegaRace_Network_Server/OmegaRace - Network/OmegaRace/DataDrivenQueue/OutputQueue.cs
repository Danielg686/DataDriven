using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OmegaRace
{
    class OutputQueue
    {
        private static OutputQueue instance = null;
        public static OutputQueue Instance()
        {
            if (instance == null)
            {
                instance = new OutputQueue();
            }
            return instance;
        }

        Queue<DataMessage> outputQueue;

        private OutputQueue()
        {
            outputQueue = new Queue<DataMessage>();
        }

        public static void AddToQueue(DataMessage msg)
        {
            instance.outputQueue.Enqueue(msg);

        }

        public static void Process()
        {
            while (instance.outputQueue.Count > 0)
            {
                DataMessage msg = instance.outputQueue.Dequeue();

                GameManager.RecieveMessage(msg);

                /*
                switch (msg.type)
                {
                    case DataMessage_Type.GAME_STATE:
                        GameManager.RecieveMessage(msg);
                        break;
                    case DataMessage_Type.PLAYER_INPUT:
                        GameManager.RecieveMessage(msg);
                        break;
                    case DataMessage_Type.PLAYER_UPDATE:
                        
                        break;
                    default:
                        //MyServer.Instance.SendMessageToClient(msg);
                        break;
                }*/
                

            }
        }


        

    }
}
