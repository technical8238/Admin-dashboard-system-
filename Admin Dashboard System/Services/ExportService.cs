using Admin_Dashboard_System.ViewModels;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace Admin_Dashboard_System.Services
{
    public interface IExportService
    {
        byte[] ExportProductsToExcel(IEnumerable<ProductViewModel> products);
        byte[] ExportProductsToPdf(IEnumerable<ProductViewModel> products);
        byte[] ExportUsersToExcel(IEnumerable<UserViewModel> users);
        byte[] ExportUsersToPdf(IEnumerable<UserViewModel> users);
        byte[] ExportOrdersToExcel(IEnumerable<OrderViewModel> orders);
        byte[] ExportOrdersToPdf(IEnumerable<OrderViewModel> orders);
    }

    public class ExportService : IExportService
    {
        public byte[] ExportProductsToExcel(IEnumerable<ProductViewModel> products)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Products");
            
            worksheet.Cell("A1").Value = "ID";
            worksheet.Cell("B1").Value = "Name";
            worksheet.Cell("C1").Value = "Description";
            worksheet.Cell("D1").Value = "Price";
            worksheet.Cell("E1").Value = "Stock Quantity";
            worksheet.Cell("F1").Value = "Category";
            worksheet.Cell("G1").Value = "Status";
            worksheet.Cell("H1").Value = "Created Date";

            int row = 2;
            foreach (var product in products)
            {
                worksheet.Cell(row, 1).Value = product.Id;
                worksheet.Cell(row, 2).Value = product.Name;
                worksheet.Cell(row, 3).Value = product.Description;
                worksheet.Cell(row, 4).Value = product.Price;
                worksheet.Cell(row, 5).Value = product.StockQuantity;
                worksheet.Cell(row, 6).Value = product.CategoryName;
                worksheet.Cell(row, 7).Value = product.IsActive ? "Active" : "Inactive";
                worksheet.Cell(row, 8).Value = product.CreatedDate.ToString("yyyy-MM-dd");
                row++;
            }

            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportProductsToPdf(IEnumerable<ProductViewModel> products)
        {
            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var title = new Paragraph("Products Report")
                .SetFontSize(18)
                .SetFont(font)
                .SetMarginBottom(20);
            document.Add(title);

            var table = new Table(8);
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Name");
            table.AddHeaderCell("Description");
            table.AddHeaderCell("Price");
            table.AddHeaderCell("Stock");
            table.AddHeaderCell("Category");
            table.AddHeaderCell("Status");
            table.AddHeaderCell("Created Date");

            foreach (var product in products)
            {
                table.AddCell(product.Id.ToString());
                table.AddCell(product.Name);
                table.AddCell(product.Description);
                table.AddCell($"${product.Price:F2}");
                table.AddCell(product.StockQuantity.ToString());
                table.AddCell(product.CategoryName);
                table.AddCell(product.IsActive ? "Active" : "Inactive");
                table.AddCell(product.CreatedDate.ToString("yyyy-MM-dd"));
            }

            document.Add(table);
            document.Close();

            return stream.ToArray();
        }

        public byte[] ExportUsersToExcel(IEnumerable<UserViewModel> users)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Users");
            
            worksheet.Cell("A1").Value = "ID";
            worksheet.Cell("B1").Value = "Username";
            worksheet.Cell("C1").Value = "Email";
            worksheet.Cell("D1").Value = "First Name";
            worksheet.Cell("E1").Value = "Last Name";
            worksheet.Cell("F1").Value = "Role";
            worksheet.Cell("G1").Value = "Status";
            worksheet.Cell("H1").Value = "Created Date";

            int row = 2;
            foreach (var user in users)
            {
                worksheet.Cell(row, 1).Value = user.Id;
                worksheet.Cell(row, 2).Value = user.UserName;
                worksheet.Cell(row, 3).Value = user.Email;
                worksheet.Cell(row, 4).Value = user.FirstName;
                worksheet.Cell(row, 5).Value = user.LastName;
                worksheet.Cell(row, 6).Value = user.Role;
                worksheet.Cell(row, 7).Value = user.IsActive ? "Active" : "Inactive";
                worksheet.Cell(row, 8).Value = user.CreatedDate.ToString("yyyy-MM-dd");
                row++;
            }

            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportUsersToPdf(IEnumerable<UserViewModel> users)
        {
            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var title = new Paragraph("Users Report")
                .SetFontSize(18)
                .SetFont(font)
                .SetMarginBottom(20);
            document.Add(title);

            var table = new Table(8);
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Username");
            table.AddHeaderCell("Email");
            table.AddHeaderCell("First Name");
            table.AddHeaderCell("Last Name");
            table.AddHeaderCell("Role");
            table.AddHeaderCell("Status");
            table.AddHeaderCell("Created Date");

            foreach (var user in users)
            {
                table.AddCell(user.Id);
                table.AddCell(user.UserName);
                table.AddCell(user.Email);
                table.AddCell(user.FirstName);
                table.AddCell(user.LastName);
                table.AddCell(user.Role);
                table.AddCell(user.IsActive ? "Active" : "Inactive");
                table.AddCell(user.CreatedDate.ToString("yyyy-MM-dd"));
            }

            document.Add(table);
            document.Close();

            return stream.ToArray();
        }

        public byte[] ExportOrdersToExcel(IEnumerable<OrderViewModel> orders)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Orders");
            
            worksheet.Cell("A1").Value = "Order ID";
            worksheet.Cell("B1").Value = "Customer";
            worksheet.Cell("C1").Value = "Email";
            worksheet.Cell("D1").Value = "Total Amount";
            worksheet.Cell("E1").Value = "Status";
            worksheet.Cell("F1").Value = "Items";
            worksheet.Cell("G1").Value = "Order Date";

            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value = order.Id;
                worksheet.Cell(row, 2).Value = order.UserName;
                worksheet.Cell(row, 3).Value = order.UserEmail;
                worksheet.Cell(row, 4).Value = order.TotalAmount;
                worksheet.Cell(row, 5).Value = order.Status;
                worksheet.Cell(row, 6).Value = order.ItemCount;
                worksheet.Cell(row, 7).Value = order.OrderDate.ToString("yyyy-MM-dd");
                row++;
            }

            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportOrdersToPdf(IEnumerable<OrderViewModel> orders)
        {
            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var title = new Paragraph("Orders Report")
                .SetFontSize(18)
                .SetFont(font)
                .SetMarginBottom(20);
            document.Add(title);

            var table = new Table(7);
            table.AddHeaderCell("Order ID");
            table.AddHeaderCell("Customer");
            table.AddHeaderCell("Email");
            table.AddHeaderCell("Total Amount");
            table.AddHeaderCell("Status");
            table.AddHeaderCell("Items");
            table.AddHeaderCell("Order Date");

            foreach (var order in orders)
            {
                table.AddCell(order.Id.ToString());
                table.AddCell(order.UserName);
                table.AddCell(order.UserEmail);
                table.AddCell($"${order.TotalAmount:F2}");
                table.AddCell(order.Status);
                table.AddCell(order.ItemCount.ToString());
                table.AddCell(order.OrderDate.ToString("yyyy-MM-dd"));
            }

            document.Add(table);
            document.Close();

            return stream.ToArray();
        }
    }
}
