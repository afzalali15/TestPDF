using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Widget;
using Java.IO;
using TestPDF;
using TestPDF.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Button = Android.Widget.Button;
using RelativeLayout = Android.Widget.RelativeLayout;

[assembly: ExportRenderer(typeof(PDFView), typeof(PDFViewRenderer))]
namespace TestPDF.Droid
{
    public class PDFViewRenderer : ViewRenderer<PDFView, RelativeLayout>
    {
        Context _context;
        private static string FILENAME = "gre_research_validity_data.pdf";

        private int pageIndex;
        private PdfRenderer pdfRenderer;
        private PdfRenderer.Page currentPage;
        private ParcelFileDescriptor parcelFileDescriptor;
        private ImageView _imageView;
        private Button prePageButton;
        private Button nextPageButton;
        public PDFViewRenderer(Context context) : base(context)
        {
            _context = context;
            pageIndex = 0;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PDFView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    RelativeLayout layout = new RelativeLayout(_context);

                    RelativeLayout.LayoutParams param = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    param.AddRule(LayoutRules.CenterInParent);
                    _imageView = new ImageView(_context);
                    _imageView.LayoutParameters = param;
                    layout.AddView(_imageView, param);

                    param = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                    param.AddRule(LayoutRules.CenterVertical);
                    param.AddRule(LayoutRules.AlignParentLeft);
                    prePageButton = new Button(_context);
                    prePageButton.Text = "<";
                    prePageButton.Click += (sender, arg) =>
                    {
                        showPage(--pageIndex);
                    };
                    prePageButton.LayoutParameters = param;
                    layout.AddView(prePageButton, param);

                    param = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                    param.AddRule(LayoutRules.CenterVertical);
                    param.AddRule(LayoutRules.AlignParentRight);
                    nextPageButton = new Button(_context);
                    nextPageButton.Text = ">";
                    nextPageButton.Click += (sender, arg) =>
                    {
                        showPage(++pageIndex);
                    };
                    nextPageButton.LayoutParameters = param;
                    layout.AddView(nextPageButton, param);

                    SetNativeControl(layout);
                }
                try
                {
                    openRenderer(_context);
                    showPage(pageIndex);
                }
                catch (IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private void openRenderer(Context context)
        {
            // In this sample, we read a PDF from the assets directory.
            File file = new File(context.CacheDir, FILENAME);
            if (!file.Exists())
            {
                // Since PdfRenderer cannot handle the compressed asset file directly, we copy it into
                // the cache directory.
                System.IO.Stream input = context.Assets.Open(FILENAME);
                Java.IO.FileOutputStream output = new Java.IO.FileOutputStream(file);
                byte[] buffer = new byte[1024];
                int size;
                while ((size = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, size);
                }
                input.Close();
                output.Close();
            }
            parcelFileDescriptor = ParcelFileDescriptor.Open(file, ParcelFileMode.ReadOnly);
            // This is the PdfRenderer we use to render the PDF.
            if (parcelFileDescriptor != null)
            {
                pdfRenderer = new PdfRenderer(parcelFileDescriptor);
            }
        }

        private void showPage(int index)
        {
            if (pdfRenderer.PageCount <= index)
            {
                return;
            }
            // Make sure to close the current page before opening another one.
            if (null != currentPage)
            {
                currentPage.Close();
            }
            currentPage = pdfRenderer.OpenPage(index);
            Bitmap bitmap = Bitmap.CreateBitmap(currentPage.Width, currentPage.Height,
                    Bitmap.Config.Argb8888);
            currentPage.Render(bitmap, null, null, PdfRenderMode.ForDisplay);
            _imageView.SetImageBitmap(bitmap);
            updateUi();
        }

        private void updateUi()
        {
            int index = currentPage.Index;
            int pageCount = pdfRenderer.PageCount;
            prePageButton.Enabled = 0 != index;
            nextPageButton.Enabled = index + 1 < pageCount;
        }

        public int getPageCount()
        {
            return pdfRenderer.PageCount;
        }
    }
}
