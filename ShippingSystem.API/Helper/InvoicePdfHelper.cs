using iTextSharp.text;
using iTextSharp.text.pdf;
using ShippingSystem.Core.Entities;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShippingSystem.API.Helper
{
    public static class InvoicePdfHelper
    {
        public static byte[] Generate(Order order, WeightSetting weightSetting)
        {
            using var ms = new MemoryStream();
            var doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            doc.Add(new Paragraph("Invoice", titleFont));
            doc.Add(new Paragraph($"Order ID: {order.Id}", textFont));
            doc.Add(new Paragraph($"Customer Name: {order.CustomerName}", textFont));
            doc.Add(new Paragraph($"Order Date: {order.CreationDate:yyyy-MM-dd HH:mm}", textFont));

            // total weight
            double totalWeight = order.Products.Sum(p => p.Weight * p.Quantity);
            doc.Add(new Paragraph($"Total Weight: {totalWeight} kg", textFont));
            doc.Add(new Paragraph("\n"));

            // items total price
            decimal itemsTotal = order.Products.Sum(p => p.Price * (decimal)p.Quantity);
            doc.Add(new Paragraph($"Items Total: {itemsTotal.ToString("0.00")} EGP", textFont));
            decimal total = itemsTotal;

            // city
            if (order.City != null)
            {
                doc.Add(new Paragraph($"City: {order.City.Name} (Delivery Price: {order.City.Price.ToString("0.00")} EGP)", textFont));
                total += order.City.Price;
            }

            // shipping type
            if (order.ShippingType != null)
            {
                doc.Add(new Paragraph($"Shipping Type: {order.ShippingType.ShippingTypeName} (Price: {order.ShippingType.ShippingPrice.ToString("0.00")} EGP)", textFont));
                total += order.ShippingType.ShippingPrice;
            }

            // is shipped to village
            if (order.IsShippedToVillage)
            {
                decimal villageCharge = 10m;
                doc.Add(new Paragraph($"Village Delivery Charge: {villageCharge.ToString("0.00")} EGP", textFont));
                total += villageCharge;
            }

            // extra weight charge
            if (weightSetting != null)
            {
                
                double maxWeight = 0;
                bool parseSuccess = false;

                // parsing string to double 
                parseSuccess = double.TryParse(weightSetting.WeightRange,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out maxWeight);

                // removing non-numeric characters and trying to parse again
                if (!parseSuccess)
                {
                    var cleanString = Regex.Replace(weightSetting.WeightRange, "[^0-9.,]", "");
                    parseSuccess = double.TryParse(cleanString,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out maxWeight);
                }

                if (parseSuccess)
                {
                    doc.Add(new Paragraph($"Max Allowed Weight: {maxWeight} kg", textFont));

                    if (totalWeight > maxWeight)
                    {
                        double extraWeight = totalWeight - maxWeight;
                        decimal extraCharge = (decimal)(extraWeight * weightSetting.ExtraPrice);

                        doc.Add(new Paragraph($"Extra Weight: {extraWeight} kg", textFont));
                        doc.Add(new Paragraph($"Extra Weight Charge: {extraCharge.ToString("0.00")} EGP", textFont));
                        total += extraCharge;
                    }
                    else
                    {
                        doc.Add(new Paragraph("Extra Weight: 0 kg (No extra charge)", textFont));
                    }
                }
                else
                {
                    doc.Add(new Paragraph("Warning: Could not parse weight setting configuration", textFont));
                }
            }
            else
            {
                doc.Add(new Paragraph("Warning: Weight settings not found", textFont));
            }

            // total price
            doc.Add(new Paragraph($"\nTotal Price: {total.ToString("0.00")} EGP", titleFont));
            doc.Add(new Paragraph("\n"));

            // products table
            var table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.AddCell("Product");
            table.AddCell("Quantity");
            table.AddCell("Price");
            table.AddCell("Weight");

            foreach (var p in order.Products)
            {
                table.AddCell(p.Name);
                table.AddCell(p.Quantity.ToString());
                table.AddCell(p.Price.ToString("0.00"));
                table.AddCell(p.Weight.ToString());
            }

            doc.Add(table);
            doc.Close();
            return ms.ToArray();
        }
    }
}