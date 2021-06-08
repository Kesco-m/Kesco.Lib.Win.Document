using System;

namespace Kesco.Lib.Win.Document.Items
{
    public class VariantListItem : ListItem
    {
        public VariantListItem(int id, VariantType type, string text)
            : base(id, text)
        {
            ImageType = "TIF";
            EditedTime = DateTime.MinValue;
            CreateTime = DateTime.MinValue;
            Type = type;
        }

        #region Accessors

        public DateTime CreateTime { get; set; }
        public DateTime EditedTime { get; set; }
        public bool Printed { get; set; }
        public VariantType Type { get; set; }
        public string ImageType { get; set; }

        #endregion

		public bool IsPDF()
		{
			return ImageType.Equals("pdf", StringComparison.OrdinalIgnoreCase);
		}
    }

    /// <summary>
    ///   Типы изображений
    /// </summary>
    public enum VariantType
    {
        Image,
        MainImage,
        ImageOriginal,
        MainImageOriginal,
        ImagePrinted,
        MainImagePrinted,
        ImageOriginalPrinted,
        MainImageOriginalPrinted,
        Data,
        ICConnect,
        ScalaConnect,
        ICMRConnect
    }
}