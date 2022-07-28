using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.PointOfService;
using WPSaturnEMoney.Models;

namespace WPSaturnEMoney.Common
{
    public partial class PrintAPI
    {
        public static PosPrinter Printer = null;
        public const string OPOS_PTR_ESC = "\x1B"; 
        public const string OPOS_PTR_CRLF = "\x1B|1lF";
        public enum HeaderFooterType
        {
            HEADER,
            FOOTER
        }

        /// <summary>
        /// Get Printer (PosPrinter object, null if not found).
        /// </summary>
        /// <param name="printerName">Service Object Name or Logical Name of Printer.</param>
        public static void GetPrinter(string printerName)
        {
            Utilities.Log.Info("Get printer PosPrinter.json >> device_name: " + printerName);

            Printer = null;
            PosExplorer posExplorer = new PosExplorer();
            DeviceCollection devices = posExplorer.GetDevices(DeviceType.PosPrinter, DeviceCompatibilities.OposAndCompatibilityLevel1);
            foreach (DeviceInfo device in devices)
            {
                if (device.ServiceObjectName == printerName || device.LogicalNames.Contains(printerName))
                {
                    Printer = posExplorer.CreateInstance(device) as PosPrinter;
                    Utilities.Log.Info("Found printer: " + device.Description);
                    return;
                }
            }
            Utilities.Log.Error("▲ Printer not found");
        }

        /// <summary>
        /// Open connection to the Printer after GetPrinter() is executed.
        /// </summary>
        public static void OpenConnection()
        {
            if (Printer is null) return;
            try
            {
                // if exception is thrown when open or claim, should see this solution for more information
                // https://stackoverflow.com/a/68279370
                Printer.Open(); 
                if (!Printer.Claimed)
                {
                    Printer.Claim(1000);
                    Printer.DeviceEnabled = true;
                    Utilities.Log.Info("Printer is enabled!");
                }
                else
                {
                    Utilities.Log.Info("Printer has been already enabled!");
                }
            }
            catch (PosControlException ex)
            {
                Utilities.Log.Error($"▲ Printer OpenConnection error! ErrorCode: {ex.ErrorCode}, ErrorCodeExtended: {ex.ErrorCodeExtended}");
            }
        }

        /// <summary>
        /// Close connection to the Printer (also set null for PosPrinter object).
        /// </summary>
        public static void CloseConnection()
        {
            if (Printer is null) return;
            try
            {
                Printer.DeviceEnabled = false;
                Printer.Release();
                Printer.Close();
                Printer = null;
                Utilities.Log.Info("Close printer connection!");
            }
            catch (PosControlException ex)
            {
                Utilities.Log.Error($"▲ Printer CloseConnection error! ErrorCode: {ex.ErrorCode}, ErrorCodeExtended: {ex.ErrorCodeExtended}");
            }
        }

        // Testing function
        public static void TestPrintReceipt()
        {
            FileStruct.CsFileRead(GlobalData.AppPath, GlobalData.PosPrinterPath, ref GlobalData.PrinterData);
            GetPrinter(GlobalData.PrinterData.device_name);
            OpenConnection();
            if (Printer != null && Printer.DeviceEnabled)
            {
                if (GlobalData.PrinterData.header is null || GlobalData.PrinterData.header is null)
                {
                    Utilities.Log.Error("No data, header or footer to print.");
                }
                else
                {
                    /*GetHeaderFooter(HeaderFooterType.HEADER); 
                    Utilities.Log.Info("Characters number: " + Printer.RecSidewaysMaxChars + " or " + Printer.RecLineChars);
                    // Printer.PrintNormal(PrinterStation.Receipt, "Hello, World!" + OPOS_PTR_CRLF);
                    PrintOneRecord("", "■■■お客様控え■■■", "Center");
                    PrintOneRecord("", "ビジュアルジャパン", "Left");
                    PrintOneRecord("", "2016年01月29日 16:34", "Right");
                    PrintOneRecord("WAONカードID", "************2678", "Justify");
                    PrintOneRecord("", "1", "Empty");
                    PrintOneRecord("", "=", "Line");
                    PrintOneRecord("に表示されている取引前残高を比較 し、残高が一致していれば成立していません", "￥1000", "Justify");
                    PrintOneRecord("", "-", "Line");
                    PrintOneRecord("", "に表示されている取引前残高を比較 し、残高が一致していれば成立していません", "Left");
                    PrintOneRecord("", "-", "Line");
                    PrintOneRecord("", "に表示されている取引前残高を比較 し、残高が一致していれば成立していません", "Right");
                    // PrintOneRecord("", "3", "Empty");
                    PrintOneRecord("売場：", "係員：", "LeftCenter");
                    GetHeaderFooter(HeaderFooterType.FOOTER);*/
                    string[] spearator = { "\\n" };
                    string[] infoLines = "testing1\\ntesting2\\ntesting3".Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    List<FileStruct.RowData> printReceiptData = new List<FileStruct.RowData>();
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Header" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 1, Label = "", Value = "交通系IC", Attr = "Center" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 2, Label = "", Value = "［支払票（支払）］", Attr = "Center" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "加盟店名", Attr = "Left" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 3, Label = "", Value = "  zeet", Attr = "Left" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 4, Label = "", Value = "  0123456789", Attr = "Left" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 5, Label = "店舗端末ＩＤ", Value = "ＩＤ123456", Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 6, Label = "端末番号", Value = "78945-654-56789", Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 7, Label = "伝票番号", Value = "54321", Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 8, Label = "ご利用日時", Value = DateTime.Now.ToString("yy/MM/dd HH:mm:ss"), Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 9, Label = "カード番号", Value = "123456789123456", Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 10, Label = "支払金額", Value = "￥" + string.Format("{0:N0}", 1000000), Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 11, Label = "カード残高", Value = "￥" + string.Format("{0:N0}", 1234000), Attr = "Justify" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                    foreach (string line in infoLines)
                    {
                        printReceiptData.Add(new FileStruct.RowData { RowID = 12, Label = "", Value = line, Attr = "Left" });
                    }
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 1314, Label = "売場：", Value = "係員：", Attr = "LeftCenter" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "1", Attr = "Empty" });
                    printReceiptData.Add(new FileStruct.RowData { RowID = 15, Label = "", Value = "お客様控え", Attr = "Center" });
                    printReceiptData.Add(new FileStruct.RowData { Label = "", Value = "", Attr = "Footer" });
                    Utilities.PrintReceipt(printReceiptData);
                }
                CloseConnection();
            }
        }

        /// <summary>
        /// Execute print header and footer in an order with specific cases.
        /// </summary>
        /// <param name="type">HeaderFooterType (HEADER or FOOTER)</param>
        public static void GetHeaderFooter(HeaderFooterType type)
        {
            string[][] dataArray = (type == HeaderFooterType.HEADER) ? GlobalData.PrinterData.header : GlobalData.PrinterData.footer;
            string logMsg = "";
            for (int i = 0; i < dataArray.Length; i++)
            {
                switch (dataArray[i][0])
                {
                    case "TRN_ST":
                        Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Transaction);
                        logMsg = "Start TransactionPrint";
                        break;
                    case "TRN_ED":
                        Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Normal);
                        logMsg = "End TransactionPrint";
                        break;
                    case "SET_WIDTH":
                        Printer.RecLineChars = GlobalData.PrinterData.width;
                        logMsg = "PosPrinter.json >> width: " + GlobalData.PrinterData.width + ", Printer.RecLineChars: " + Printer.RecLineChars;
                        logMsg += ", Printer.RecLineCharsList: ";
                        Array.ForEach(Printer.RecLineCharsList, item => logMsg += item + " ");
                        break;
                    case "PN_ESC":
                        Printer.PrintNormal(PrinterStation.Receipt, OPOS_PTR_ESC + dataArray[i][1]);
                        logMsg = "Print PN_ESC: " + OPOS_PTR_ESC + dataArray[i][1];
                        break;
                    case "PN_TXT":
                        Printer.PrintNormal(PrinterStation.Receipt, dataArray[i][1]);
                        logMsg = "Print PN_TXT: " + dataArray[i][1];
                        break;
                    case "PN_CRLF":
                        string data = dataArray[i][1];
                        if (!int.TryParse(data, out int times)) times = 1;
                        while (times > 0)
                        {
                            Printer.PrintNormal(PrinterStation.Receipt, OPOS_PTR_CRLF);
                            --times;
                        }
                        logMsg = "Print PN_CRLF: " + times;
                        break;
                    case "PB_FILE":
                        // bimapData = "FileName,Width,Alignment". Ex: "Pos/Logo1.bmp,360,-2"
                        string[] bitmapData = dataArray[i][1].Split(',');
                        string bitmapPath = GlobalData.AppPath + "/" + bitmapData[0];
                        if (!int.TryParse(bitmapData[1], out int width))
                        {
                            Utilities.Log.Error(@"▲ Cannot print bitmap. Config\PosPrinter.json error: bitmap width is not an integer.");
                        }
                        if (!int.TryParse(bitmapData[2], out int alignment))
                        {
                            Utilities.Log.Error(@"▲ Cannot print bitmap. Config\PosPrinter.json error: bitmap alignment is not an integer.");
                        }
                        Printer.RecLetterQuality = true;
                        Printer.PrintBitmap(PrinterStation.Receipt, bitmapPath, width, alignment);
                        logMsg = "Print PB_FILE: " + dataArray[i][1];
                        break;
                }
                Utilities.Log.Info(logMsg);
            }
        }

        public static void PrintOneRecord(string label, string value, string attribute)
        {
            int maxChars = Printer.RecLineChars;
            int spaceCount;
            string printData = "";
            int printDataLength;
            switch (attribute)
            {
                case "Left":
                    printData += combinePrintData(maxChars, value, ref label);
                    Printer.PrintNormal(PrinterStation.Receipt, printData + OPOS_PTR_CRLF);
                    //Utilities.Log.Info("Left; Print data: " + printData);
                    break;
                case "Center":
                    printData += combinePrintData(maxChars, value, ref label);
                    printDataLength = countChars(printData);
                    spaceCount = maxChars - printDataLength;
                    printData = addSpace(spaceCount / 2) + printData;
                    Printer.PrintNormal(PrinterStation.Receipt, printData + OPOS_PTR_CRLF);
                    //Utilities.Log.Info("Center; Print data: " + printData);
                    break;
                case "LeftCenter":
                    string tempLabel = "";
                    printData += combinePrintData(maxChars, label, ref tempLabel);
                    string tempValue = combinePrintData(maxChars, value, ref tempLabel);
                    spaceCount = maxChars - countChars(printData) - countChars(tempValue);
                    printData = printData + addSpace(spaceCount / 2) + tempValue;
                    Printer.PrintNormal(PrinterStation.Receipt, printData + OPOS_PTR_CRLF);
                    break;
                case "Right":
                    printData += combinePrintData(maxChars, value, ref label);
                    printDataLength = countChars(printData);
                    spaceCount = maxChars - printDataLength;
                    printData = addSpace(spaceCount) + printData;
                    Printer.PrintNormal(PrinterStation.Receipt, printData + OPOS_PTR_CRLF);
                    //Utilities.Log.Info("Right; Print data: " + printData);
                    break;
                case "Justify":
                    printData += combinePrintData(maxChars, value, ref label, true);
                    Printer.PrintNormal(PrinterStation.Receipt, printData + OPOS_PTR_CRLF);
                    //Utilities.Log.Info("Justify; Print data: " + printData);
                    break;
                case "Line":
                    printData = lineData(value);
                    Printer.PrintNormal(PrinterStation.Receipt, printData);
                    //Utilities.Log.Info("Line; Print data: " + printData);
                    break;
                case "Empty":
                    int times;
                    if (!int.TryParse(value, out times))
                    {
                        times = 1;
                    }
                    //Utilities.Log.Info("Empty; Lines: " + times);
                    while (times > 0)
                    {
                        Printer.PrintNormal(PrinterStation.Receipt, OPOS_PTR_CRLF);
                        --times;
                    }
                    break;
            }
        }

        private static int countChars(string data)
        {
            return Encoding.GetEncoding("Shift_JIS").GetBytes(data).Length;
        }

        private static string addSpace(int spaces)
        {
            string output = "";
            while (spaces > 0)
            {
                output += " ";
                --spaces;
            }
            return output;
        }

        private static string lineData(string character)
        {
            string output = "";
            int maxChars = Printer.RecLineChars;

            while (countChars(output) < maxChars)
            {
                output += character;
            }
            output += OPOS_PTR_CRLF;
            return output;
        }

        private static string combinePrintData(int maxChars, string value, ref string label, bool Justify = false)
        {
            int labelLength = countChars(label);
            string printData = "";
            if (labelLength > 0 && !Justify)
            {
                label += "："; //  ":";
            }
            printData += label + value;
            int printDataLength = countChars(printData);
            int spaceCount = maxChars - printDataLength;
            // Character exceeded line width
            while (spaceCount < 0)
            {
                if (Justify)
                {
                    printData = label;
                    if (countChars(printData) < maxChars)
                    {
                        Printer.PrintNormal(PrinterStation.Receipt, printData + OPOS_PTR_CRLF);
                        label = "";
                        spaceCount = maxChars - countChars(label + value);
                        printData = label + addSpace(spaceCount) + value;
                        return printData;
                    }
                }
                string tmp = "";
                int tmpLength;
                // Count chars in Shift_JIS format to ensure printdata is fit the line width
                // (1 Japanese char = 2 Latin chars)
                do
                {
                    tmp += printData[0];
                    printData = printData.Substring(1);
                    tmpLength = countChars(tmp);
                }
                while (tmpLength < maxChars);
                // if tmpLength > maxChars means the last char is Japanese and will be printed on 1 line
                if (tmpLength > maxChars)
                {
                    printData = tmp.Last() + printData;
                    tmp = tmp.Remove(tmp.Length - 1, 1);
                }
                Printer.PrintNormal(PrinterStation.Receipt, tmp + OPOS_PTR_CRLF);
                if (Justify)
                {
                    label = printData;
                    printData += value;
                }
                printDataLength = countChars(printData);
                spaceCount = maxChars - printDataLength;
            }
            if (Justify)
            {
                spaceCount = maxChars - countChars(label + value);
                printData = label + addSpace(spaceCount) + value;
            }
            return printData;
        }
    }
}
