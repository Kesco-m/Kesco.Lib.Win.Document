using System;

namespace Kesco.Lib.Win.Document.Blocks
{
    /// <summary>
    ///   Событие блока (аргументы?)
    /// </summary>
    public class BlockEventArgs : EventArgs
    {
        public BlockEventArgs(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; private set; }
        public string Name { get; private set; }
    }

    /// <summary>
    ///   Делегат для обработки события
    /// </summary>
    public delegate void BlockEventHandler(object source, BlockEventArgs e);
}