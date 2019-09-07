using System;
using Foundation;
using TestPDF;
using TestPDF.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PDFView), typeof(PDFViewRenderer))]
namespace TestPDF.iOS
{
    public class PDFViewRenderer : ViewRenderer<PDFView, PdfKit.PdfView>
    {
        PdfKit.PdfView _pdfView;
        protected override void OnElementChanged(ElementChangedEventArgs<PDFView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _pdfView = new PdfKit.PdfView();
                    SetNativeControl(_pdfView);
                }
                var documentURL = NSBundle.MainBundle.GetUrlForResource("gre_research_validity_data", "pdf");
                if (documentURL != null)
                {
                    var document = new PdfKit.PdfDocument(documentURL);
                    if (document != null)
                    {
                        _pdfView.DisplayMode = PdfKit.PdfDisplayMode.SinglePageContinuous;
                        _pdfView.AutoScales = true;
                        _pdfView.Document = document;
                        _pdfView.GoToPage(document.GetPage(0));
                    }
                }
            }
        }
    }
}