using System.Collections.Generic;
using System.Threading;

namespace ScrumTouchkit.Threading
{
    /// <summary>
    /// Stellt eine BlockingQueue zur Verfügung
    /// </summary>
    /// <typeparam name="T">Gibt an, welche Typen in dieser Queue gespeichert werden</typeparam>
    public class BlockingQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private bool closing;

        /// <summary>
        /// Fügt ein Element zu der Schlange hinzu
        /// </summary>
        /// <param name="element"></param>
        public void Enqueue(T element)
        {
            lock (queue)
            {
                queue.Enqueue(element);
                //Wake up dequeue if count == 1 (means there could be someone in dequeue)
                if (queue.Count == 1)
                    Monitor.PulseAll(queue);
            }
        }

        /// <summary>
        /// Wartet solange, bis ein Element in der Schlange steht und gibt
        /// dieses anschließend aus.
        /// Rückgabewert FALSE wird zurückgegeben, wenn die Schlange geschlossen wird
        /// Ansonsten TRUE.
        /// </summary>
        /// <param name="element">Das Zurückgegebene Element</param>
        /// <returns>TRUE bei Erfolg, sonst FALSE</returns>
        public bool DequeueWait(out T element)
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    if (closing)
                    {
                        element = default(T);
                        return false;
                    }
                    Monitor.Wait(queue);
                }
                element = queue.Dequeue();
                return true;
            }
        }

        /// <summary>
        /// Gibt das vorderste Element der Schlange zurück, ohne zu warten.
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            lock (queue)
            {
                return queue.Dequeue();
            }
        }

        /// <summary>
        /// Schließt die BlockingQueue.
        /// </summary>
        public void Close()
        {
            lock (queue)
            {
                closing = true;
                Monitor.PulseAll(queue);
            }
        }

    }
}
