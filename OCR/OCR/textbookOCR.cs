using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace OCR
{
    public class textbookOCR
    {
        OcrEngine ocr;

        // Edit this to change criteria for filter.
        bool isValid(string text)
        {
            if (text.Contains("Hack"))
            {
                return true;
            }
            return false;
        }


        // constructor
        public textbookOCR()
        {
            var textbookLanguage = new Windows.Globalization.Language(
                Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
            this.ocr = OcrEngine.TryCreateFromLanguage(textbookLanguage);
        }

        public async void finalImage(SoftwareBitmap image)
        {
            OcrResult ocrRes = await ocr.RecognizeAsync(image);
            List<Tuple<Rect, string>> valids = await processResult(ocrRes);

            int imgWidth = image.PixelWidth;
            int imgHeight = image.PixelHeight;

        }

        // given the image,
        // returns a list of tuple of words and their bounding boxes
        // when the words fulfil the filter criteria defined above.
        public List<Tuple<Rect, string>>
            processResult(OcrResult ocrRes)
        {
            // debug
            string wholeText = ocrRes.Text;
            Debug.Write(wholeText);
            // end debug

            List<Tuple<Rect, string>> resultList = new List<Tuple<Rect, string>>();

            IReadOnlyList<OcrLine> lines = ocrRes.Lines;
            foreach (OcrLine line in lines)
            {
                IReadOnlyList<OcrWord> words = line.Words;
                foreach (OcrWord word in words)
                {
                    Rect bounds = word.BoundingRect;
                    string text = word.Text;
                    
                    if (isValid(text))
                    {
                        Tuple<Rect, string> current = Tuple.Create(bounds, text);
                        resultList.Add(current);
                    }
                }
            }
            return resultList;
        }

        private SoftwareBitmap createBitmap(
            int imgWidth, int imgHeight, List<Tuple<Rect, string>> resultList)
        {
            SoftwareBitmap result = new SoftwareBitmap(
                BitmapPixelFormat.Rgba8, imgWidth, imgHeight, BitmapAlphaMode.Straight);
            return result;
        }
    }
}

// https://msdn.microsoft.com/en-us/library/windows/apps/windows.globalization.language.aspx