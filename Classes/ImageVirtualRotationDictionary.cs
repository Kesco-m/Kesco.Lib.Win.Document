using System.Collections.Generic;
using System.Linq;

namespace Kesco.Lib.Win.Document.Classes
{
    /// <summary>
    /// Словарь углов поворота
    /// </summary>
    internal sealed class ImageVirtualRotationDictionary
    {
        internal ImageVirtualRotationDictionary()
        {
            _pageVirtualRotation = new Dictionary<int, int>();
        }

        private int _imageId;

        /// <summary>
        /// Словарь
        /// </summary>
        private readonly Dictionary<int, int> _pageVirtualRotation;

        /// <summary>
        /// Угол поворота
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public int this[int page]
        {
            get
            {
                if (_pageVirtualRotation.ContainsKey(page))
                return _pageVirtualRotation[page];

                return 0;
            }
        }

        /// <summary>
        /// Получить угол поворота страницы текущего изображения
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        internal int GetPageRotation(int imageId, int page)
        {
            if (page < 0 || !_pageVirtualRotation.ContainsKey(page))
                return 0;

            return _pageVirtualRotation[page];
        }

		/// <summary>
		/// Обновить уголы поворота текущего изображения
		/// </summary>
		/// <param name="imageId"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		internal void UpdatePageRotation(int imageId)
		{
			if(imageId != _imageId)
			{
				_imageId = imageId;

				// сброс значений
				var keys = _pageVirtualRotation.Keys.ToList();
				foreach(int t in keys)
					_pageVirtualRotation[t] = 0;
			}

			var rotations = Environment.DocImageData.GetRotateOfSignedImage(imageId);
			if(rotations != null)
				foreach(var rotation in rotations)
					_pageVirtualRotation[rotation.Item1 - 1] = rotation.Item2;
		}

        /// <summary>
        /// Обновить угол поворота страницы текущего изображения
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        internal void UpdatePageRotation(int imageId, int page)
        {
            if (imageId != _imageId)
            {
                _imageId = imageId;

                // сброс значений
                var keys = _pageVirtualRotation.Keys.ToList();
                foreach (int t in keys)
                    _pageVirtualRotation[t] = 0;
            }

            var rotation = Environment.DocImageData.GetRotateOfSignedImage(imageId, page + 1);

            _pageVirtualRotation[page] = rotation ?? 0;
        }

        /// <summary>
        /// Обновить угол поворота страницы текущего изображения
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        internal RotateTransition SyncPageRotation(int imageId, int page)
        {
            if (!_pageVirtualRotation.ContainsKey(page))
            {
                var rotation = Environment.DocImageData.GetRotateOfSignedImage(_imageId, page);

                _pageVirtualRotation[page] = rotation ?? 0;
            }

            var result = new RotateTransition {From = _pageVirtualRotation[page]};

            UpdatePageRotation(imageId, page);

            result.To = _pageVirtualRotation[page];

            return result;
        }

        /// <summary>
        /// Флаг. Одна или более страниц повернута
        /// </summary>
        internal bool IsAnyPageRotated
        {
            get
            {
                return _pageVirtualRotation.Values.Any(c => c != 0);
            }
        }

         /// <summary>
        /// Флаг. Одна или более страниц БЫЛА повернута
        /// </summary>
        internal bool RotationChanged
        {
            get
            {
                var orig = IsAnyPageRotated;

                if (_imageId > 0)
                    for (int i = 0; i < _pageVirtualRotation.Count; i++)
                    {
                        var rotation = Environment.DocImageData.GetRotateOfSignedImage(_imageId, i + 1);

                        _pageVirtualRotation[i] = rotation ?? 0;
                    }

                return IsAnyPageRotated != orig;
            }
        }

        /// <summary>
        /// Сохранить угол виртуального поворота. PDF
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="page"></param>
        /// <param name="right"></param>
        internal void SaveVirtualRotationLocal(int imageId, int page, bool right)
        {
            if (imageId != _imageId)
            {
                _imageId = imageId;

                // сброс значений
                var keys = _pageVirtualRotation.Keys.ToList();
                foreach (int t in keys)
                    _pageVirtualRotation[t] = 0;
            }

            // 29478 Если в словаре нет ключа, добавить
            if(!_pageVirtualRotation.ContainsKey(page))
                _pageVirtualRotation.Add(page, 0);

            int angle = _pageVirtualRotation[page];

		

			_pageVirtualRotation[page] = (angle + 270 - (right?180:0)) % 360;
        }

        /// <summary>
        /// Сохранить угол виртуального поворота.
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="page"></param>
        /// <param name="rotateType"></param>
        internal void SaveVirtualRotation(int imageId, int page, int rotateType)
        {
            int angle;

            switch (rotateType)
            {
                case 1:
                    angle = 90;
                    break;
                case 2:
                    angle = 180;
                     break;
                case 3:
                    angle = 270;
                     break;
                default:
                    angle = 0;
                     break;
            }

            var rotation = Environment.DocImageData.GetRotateOfSignedImage(imageId, page);

            if (rotation == null)
                rotation = 0;

            rotation += angle;

            rotation %= 360;

            Environment.DocImageData.ApplyRotateToSignedImage(imageId, page, rotation.Value);
        }

        /// <summary>
        /// Сохранить угол виртуального поворота.  TIFF
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="page"></param>
        internal void SaveVirtualRotation(int imageId, int page)
        {
            var rotation = _pageVirtualRotation[page - 1];

            Environment.DocImageData.ApplyRotateToSignedImage(imageId, page, rotation);
        }

		/// <summary>
		/// сброс поворотов 
		/// </summary>
		/// <returns></returns>
		public bool CleanRotations()
		{
			var keys = _pageVirtualRotation.Keys.ToList();
			foreach(int t in keys)
				_pageVirtualRotation[t] = 0;
			return true;
		}
	}
}
