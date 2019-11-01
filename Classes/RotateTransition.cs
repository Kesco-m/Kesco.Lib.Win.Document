namespace Kesco.Lib.Win.Document.Classes
{
    /// <summary>
    /// Изменение угла поворота
    /// </summary>
    internal struct RotateTransition
    {
        /// <summary>
        /// Было
        /// </summary>
        internal int From { get; set; }

        /// <summary>
        /// Стало
        /// </summary>
        internal int To { get; set; }

        /// <summary>
        /// Флаг. Угол изменился
        /// </summary>
        internal bool Changed 
        {
            get
            {
                return From != To;
            }
        }
    }
}
