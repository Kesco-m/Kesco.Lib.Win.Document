using System;
using System.ComponentModel;

namespace Kesco.Lib.Win.Document.Components
{
    public class SavePartComponent : SaveToArchiveComponent
    {
        private DateTime _date;

        public SavePartComponent()
        {
        }

        public SavePartComponent(IContainer container, Controls.DocControl control)
            : base(container, control)
        {
        }

        public SavePartComponent(IContainer container)
            : base(container)
        {
        }

        public override void Save()
        {
        }

        public void SetDateTime(DateTime date)
        {
            _date = date;
        }

        public override DateTime RetrieveScanDate()
        {
            return _date;
        }
    }
}