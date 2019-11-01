﻿using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs.PrintDialogs
{
    public class NoDoubleClickAutoCheckListView : ListView
    {
        private bool _checkFromDoubleClick = false;

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (_checkFromDoubleClick)
            {
                ice.NewValue = ice.CurrentValue;
                _checkFromDoubleClick = false;
            }
            else
                base.OnItemCheck(ice);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks > 1))
            {
                _checkFromDoubleClick = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _checkFromDoubleClick = false;
            base.OnKeyDown(e);
        }
    }
}