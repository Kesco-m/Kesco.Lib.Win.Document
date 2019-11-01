using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Win32;

namespace Kesco.Lib.Win.Document.Controls
{
    public class RichTextBoxEx : RichTextBox
    {
        #region Interop-Defines

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARFORMAT2_STRUCT
        {
            public UInt32 cbSize;
            public UInt32 dwMask;
            public UInt32 dwEffects;
            public Int32 yHeight;
            public Int32 yOffset;
            public Int32 crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public char[] szFaceName;
            public UInt16 wWeight;
            public UInt16 sSpacing;
            public int crBackColor; // Color.ToArgb() -> int
            public int lcid;
            public int dwReserved;
            public Int16 sStyle;
            public Int16 wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }

		private const int EM_GETCHARFORMAT = (int)Msgs.WM_USER + 58;
		private const int EM_SETCHARFORMAT = (int)Msgs.WM_USER + 68;

		private const int EM_SETTEXTMODE = (int)Msgs.WM_USER + 89;
		private const int EM_GETTEXTMODE = (int)Msgs.WM_USER + 90;


        private const int SCF_SELECTION = 0x0001;
        private const int SCF_WORD = 0x0002;
        private const int SCF_ALL = 0x0004;

		// EM_SETTEXTMODE/EM_GETTEXTMODE flags
		private const int TM_PLAINTEXT = 1;
		private const int TM_RICHTEXT = 2;          // Default behavior 
		private const int TM_SINGLELEVELUNDO = 4;
		private const int TM_MULTILEVELUNDO = 8;    // Default behavior 
		private const int TM_SINGLECODEPAGE = 16;
		private const int TM_MULTICODEPAGE = 32;    // Default behavior 

        #region CHARFORMAT2 Flags

        private const UInt32 CFE_BOLD = 0x0001;
        private const UInt32 CFE_ITALIC = 0x0002;
        private const UInt32 CFE_UNDERLINE = 0x0004;
        private const UInt32 CFE_STRIKEOUT = 0x0008;
        private const UInt32 CFE_PROTECTED = 0x0010;
        private const UInt32 CFE_LINK = 0x0020;
        private const UInt32 CFE_AUTOCOLOR = 0x40000000;
        private const UInt32 CFE_SUBSCRIPT = 0x00010000; /* Superscript and subscript are */
        private const UInt32 CFE_SUPERSCRIPT = 0x00020000; /*  mutually exclusive           */

        private const int CFM_SMALLCAPS = 0x0040; /* (*)  */
        private const int CFM_ALLCAPS = 0x0080; /* Displayed by 3.0 */
        private const int CFM_HIDDEN = 0x0100; /* Hidden by 3.0 */
        private const int CFM_OUTLINE = 0x0200; /* (*)  */
        private const int CFM_SHADOW = 0x0400; /* (*)  */
        private const int CFM_EMBOSS = 0x0800; /* (*)  */
        private const int CFM_IMPRINT = 0x1000; /* (*)  */
        private const int CFM_DISABLED = 0x2000;
        private const int CFM_REVISED = 0x4000;

        private const int CFM_BACKCOLOR = 0x04000000;
        private const int CFM_LCID = 0x02000000;
        private const int CFM_UNDERLINETYPE = 0x00800000; /* Many displayed by 3.0 */
        private const int CFM_WEIGHT = 0x00400000;
        private const int CFM_SPACING = 0x00200000; /* Displayed by 3.0 */
        private const int CFM_KERNING = 0x00100000; /* (*)  */
        private const int CFM_STYLE = 0x00080000; /* (*)  */
        private const int CFM_ANIMATION = 0x00040000; /* (*)  */
        private const int CFM_REVAUTHOR = 0x00008000;


        private const UInt32 CFM_BOLD = 0x00000001;
        private const UInt32 CFM_ITALIC = 0x00000002;
        private const UInt32 CFM_UNDERLINE = 0x00000004;
        private const UInt32 CFM_STRIKEOUT = 0x00000008;
        private const UInt32 CFM_PROTECTED = 0x00000010;
        private const UInt32 CFM_LINK = 0x00000020;
        private const UInt32 CFM_SIZE = 0x80000000;
        private const UInt32 CFM_COLOR = 0x40000000;
        private const UInt32 CFM_FACE = 0x20000000;
        private const UInt32 CFM_OFFSET = 0x10000000;
        private const UInt32 CFM_CHARSET = 0x08000000;
        private const UInt32 CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
        private const UInt32 CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

        private const byte CFU_UNDERLINENONE = 0x00000000;
        private const byte CFU_UNDERLINE = 0x00000001;
        private const byte CFU_UNDERLINEWORD = 0x00000002; /* (*) displayed as ordinary underline    */
        private const byte CFU_UNDERLINEDOUBLE = 0x00000003; /* (*) displayed as ordinary underline    */
        private const byte CFU_UNDERLINEDOTTED = 0x00000004;
        private const byte CFU_UNDERLINEDASH = 0x00000005;
        private const byte CFU_UNDERLINEDASHDOT = 0x00000006;
        private const byte CFU_UNDERLINEDASHDOTDOT = 0x00000007;
        private const byte CFU_UNDERLINEWAVE = 0x00000008;
        private const byte CFU_UNDERLINETHICK = 0x00000009;
        private const byte CFU_UNDERLINEHAIRLINE = 0x0000000A; /* (*) displayed as ordinary underline    */

        #endregion

        #endregion

		bool m_PlainTextMode;

		public RichTextBoxEx()
		{
			// Otherwise, non-standard links get lost when user starts typing
			// next to a non-standard link
			DetectUrls = false;
		}

        [DefaultValue(false)]
        public new bool DetectUrls
        {
            get { return base.DetectUrls; }
            set { base.DetectUrls = value; }
        }

        /// <summary>
        ///   Insert a given text as a link into the RichTextBox at the current insert position.
        /// </summary>
        /// <param name="text"> Text to be inserted </param>
        public void InsertLink(string text)
        {
            InsertLink(text, SelectionStart);
        }

		// If this property doesn't work for you from the designer for some reason
		// (for example framework version...) then set this property from outside
		// the designer then uncomment the Browsable and DesignerSerializationVisibility
		// attributes and set the Property from your component initializer code
		// that runs after the designer's code.
		[DefaultValue(false)]
		//[Browsable(false)]
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PlainTextMode
		{
			get
			{
				return m_PlainTextMode;
			}
			set
			{
				m_PlainTextMode = value;
				if(IsHandleCreated)
				{
					IntPtr mode = value ? (IntPtr)TM_PLAINTEXT : (IntPtr)TM_RICHTEXT;
					Win32.User32.SendMessage(Handle, EM_SETTEXTMODE, mode, IntPtr.Zero);
				}
			}
		}

        /// <summary>
        ///   Insert a given text at a given position as a link.
        /// </summary>
        /// <param name="text"> Text to be inserted </param>
        /// <param name="position"> Insert position </param>
        public void InsertLink(string text, int position)
        {
            if (position < 0 || position > Text.Length)
                throw new ArgumentOutOfRangeException("position");

            SelectionStart = position;
            SelectedText = text;
            Select(position, text.Length);
            SelectionFont = new Font(SelectionFont, FontStyle.Underline);
            Select(position + text.Length, 0);
            SelectionFont = new Font(SelectionFont, 0);
        }

        /// <summary>
        ///   Insert a given text at at the current input position as a link. The link text is followed by a hash (#) and the given hyperlink text, both of them invisible. When clicked on, the whole link text and hyperlink string are given in the LinkClickedEventArgs.
        /// </summary>
        /// <param name="text"> Text to be inserted </param>
        /// <param name="hyperlink"> Invisible hyperlink string to be inserted </param>
		/// <param name="changeColor"> Use link color</param>
		public virtual void InsertLink(string text, string hyperlink, bool changeColor = false)
        {
            InsertLink(text, hyperlink, SelectionStart, changeColor);
        }

        /// <summary>
        ///   Insert a given text at a given position as a link. The link text is followed by a hash (#) and the given hyperlink text, both of them invisible. When clicked on, the whole link text and hyperlink string are given in the LinkClickedEventArgs.
        /// </summary>
        /// <param name="text"> Text to be inserted </param>
        /// <param name="hyperlink"> Invisible hyperlink string to be inserted </param>
        /// <param name="position"> Insert position </param>
		/// <param name="changeColor"> Use link color</param>
		public void InsertLink(string text, string hyperlink, int position, bool changeColor = false)
		{
			//if(position < 0 || position > Text.Length)
			//    throw new ArgumentOutOfRangeException("position");

			//SelectionStart = position;
			//SelectedRtf = @"{\rtf1\ansi\ansicpg1251 " + text + @"\v #" + hyperlink + @"\v0}";
			//Select(position, text.Length + hyperlink.Length + 1);
			//Color col = this.SelectionColor;
			//if(changeColor)
			//    this.SelectionColor = Color.Blue;
			//SelectionFont = new Font(SelectionFont, FontStyle.Underline);
			//Select(position + text.Length + hyperlink.Length + 1, 0);
			//if(changeColor)
			//    this.SelectionColor = col;
			//SelectionFont = new Font(SelectionFont, 0);
			if(position < 0 || position > Text.Length)
				throw new ArgumentOutOfRangeException("position");

			SelectionStart = position;
			SelectedRtf = string.Format("{{\\rtf1\\ansi\\ansicpg1251 {0}\\v #{1}\\v0}}", text, hyperlink);
			
			Select(position, text.Length + hyperlink.Length + 1);
			//this.SelectionFont = new Font(SelectionFont, FontStyle.Underline);
			SetSelectionLink(true);
			this.Select(position + text.Length + hyperlink.Length + 1, 0);
			//this.SelectionFont = new Font(SelectionFont, 0);
			SetSelectionLink(false);
		}


		/// <summary>
		/// Set the current selection's link style
		/// </summary>
		/// <param name="link">true: set link style, false: clear link style</param>
		public void SetSelectionLink(bool link)
		{
			SetSelectionStyle(CFM_LINK, link ? CFE_LINK   : 0);
		}

		/// <summary>
		/// Get the link style for the current selection
		/// </summary>
		/// <returns>0: link style not set, 1: link style set, -1: mixed</returns>
		public int GetSelectionLink()
		{
			return GetSelectionStyle(CFM_LINK, CFE_LINK);
		}


		private void SetSelectionStyle(UInt32 mask, UInt32 effect)
		{
			CHARFORMAT2_STRUCT cf = new CHARFORMAT2_STRUCT();
			cf.cbSize = (UInt32)Marshal.SizeOf(cf);
			cf.dwMask = mask;
			//cf.crTextColor = ColorTranslator.ToWin32(Color.Green);
			cf.dwEffects = effect;

			IntPtr wpar = new IntPtr(SCF_SELECTION);
			IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
			Marshal.StructureToPtr(cf, lpar, false);

			IntPtr res = User32.SendMessage(Handle, EM_SETCHARFORMAT, wpar, lpar);

			Marshal.FreeCoTaskMem(lpar);
		}

		private int GetSelectionStyle(UInt32 mask, UInt32 effect)
		{
			CHARFORMAT2_STRUCT cf = new CHARFORMAT2_STRUCT();
			cf.cbSize = (UInt32)Marshal.SizeOf(cf);
			cf.szFaceName = new char[32];

			IntPtr wpar = new IntPtr(SCF_SELECTION);
			IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
			Marshal.StructureToPtr(cf, lpar, false);

			IntPtr res = User32.SendMessage(Handle, EM_GETCHARFORMAT, wpar, lpar);

			cf = (CHARFORMAT2_STRUCT)Marshal.PtrToStructure(lpar, typeof(CHARFORMAT2_STRUCT));

			int state;
			// dwMask holds the information which properties are consistent throughout the selection:
			if((cf.dwMask & mask) == mask)
			{
				if((cf.dwEffects & effect) == effect)
					state = 1;
				else
					state = 0;
			}
			else
			{
				state = -1;
			}

			Marshal.FreeCoTaskMem(lpar);
			return state;
		}
    }
}