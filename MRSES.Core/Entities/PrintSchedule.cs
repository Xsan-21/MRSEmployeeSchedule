using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MRSES.Core.Entities
{
    public class PrintSchedule : IDisposable
    {
        static System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-PR");

	    public string Position {get; set;}
        string StoreLocation { get { return Configuration.Location; } }
        public string ReportFolderLocation { get { return @Configuration.ReportFolderLocation; } }
	    public NodaTime.LocalDate Week {get; set;}
	    public Schedule[] Schedule {get; set;}
	
	    string _reportName;
	    public string ReportName 
	    {
		    private get 
		    {		
			    if(string.IsNullOrEmpty(_reportName))
			    {
				    string week = Week.ToString("yyyy-MM-dd", culture);
				    _reportName = string.Format("\\Horario-{0}-Semana-{1}.pdf", Position, week);
			    }
			
			    return _reportName;
		    }
		
		    set
		    {
			    _reportName = string.Format("\\{0}.pdf", value);
		    }
	    }
	
	    BaseFont _bfHelvetica;
	    FileStream _fileStream;
	    Document _document;
	    PdfWriter _pdfWriter;
	    PdfPTable _table;
	
	    public PrintSchedule()
	    {            
		    _bfHelvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);		
		    _document = new Document(PageSize.LETTER.Rotate(), 36, 36, 2, 2); // 1 inch = 72 points	
		    _table = new PdfPTable(9) {SpacingBefore = 20f, SpacingAfter = 20f};
	    }
	
	    public PrintSchedule(string position, Schedule[] schedule) : this()
	    {
		    Position = position;
		    Schedule = schedule;
	    }
	
	    public void Print()
	    {
		    ValidatePositionAndWeekSchedule();
		    _fileStream = new FileStream(ReportFolderLocation + ReportName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
		    _pdfWriter = PdfWriter.GetInstance(_document, _fileStream);
		    WriteDocumentInformationProperties();
		    PrintEmployeesSchedule();
	    }
	
	    void ValidatePositionAndWeekSchedule()
	    {
		    if(string.IsNullOrEmpty(Position))
			    throw new Exception("No ha indicado la posición de los empleados a imprimir. Ej. Baggers, Cajeras, etc.");
		
		    if(Week == new NodaTime.LocalDate())
			    throw new Exception("No ha indicado la semana que desea imprimir.");
		
		    if(Schedule == null)
		    throw new Exception("No ha indicado el horario a imprimir.");
	    }
	
	    string GetWeekTitle()
	    {
		    string weekStart = Week.ToString("d \\de MMMM \\de yyyy", culture);
		    string weekEnd = Week.PlusDays(6).ToString("d \\de MMMM \\de yyyy", culture);
		    return ": Semana del " + weekStart + " al " + weekEnd;
	    }
	
	    List<string> GetWeekDays()
	    {
		    return Shared.DateFunctions.DaysOfWeekInString(Week, culture)
		    .Select(u => u.ToUpper())
		    .ToList();
	    }
	
	    void PrepareNewReportPage()
	    {
		    _table = new PdfPTable(9) {SpacingBefore = 20f, SpacingAfter = 20f};	
		    _table.TotalWidth = 725f;
		    _table.LockedWidth = true;
		    _table.SetWidths(new float[]{150f, 150f, 150f, 150f, 150f, 150f, 150f, 150f, 100f});
		
		    var columnNames = new List<string>();
		    columnNames.Add("EMPLEADO");
		    columnNames.AddRange(GetWeekDays());
		    columnNames.Add("TOTAL HORAS");
		
		    foreach (var columnName in columnNames)
			    _table.AddCell(CellWithValue(columnName, "center"));
			
		    _document.Add(InsertDocumentHeader());
	    }
	
	    void PrintEmployeesSchedule()
	    {		
		    _document.Open();
		    PrepareNewReportPage();
		
		    int recordIndex = 0, printedRecords = 0;
		    foreach (var schedule in Schedule)
		    {
			    WriteEmployeeScheduleToPDF(schedule);
			
			    recordIndex++;
			
			    if(recordIndex % 15 == 0) // print 15 records per page
			    {
				    InsertTotalHoursPerDay(printedRecords, recordIndex);
				    printedRecords += 15;
				    _document.Add(_table);
				    _document.Add(InsertDocumentFooter());
				    _document.NewPage();
				    PrepareNewReportPage();
			    }
		    }	
		
		    InsertTotalHoursPerDay(printedRecords, Schedule.Length - printedRecords);
		    _document.Add(_table);
            _document.Add(InsertDocumentFooter());
            _document.Close();
	    }
	
	    void WriteEmployeeScheduleToPDF(Schedule schedule)
	    {
		    var valuesToPrint = new List<string>();		
		    var employee_name = schedule.Name.ToUpper().Split(' ').Take(2).Aggregate("", (first,last) => first + last + "\n");
		
		    valuesToPrint.Add(employee_name);
		
		    foreach (var turn in schedule.WeekDays)
			    valuesToPrint.Add(turn.FirstTurn + "\n" + turn.SecondTurn);
		
		    valuesToPrint.Add(schedule.HoursOfWeek.ToString());
		
		    _table.AddCell(CellWithValue(valuesToPrint.First(), "left")); // Insert employee name
		
		    foreach (var turn in valuesToPrint.Skip(1))	// skip the inserted name in the above step
			    _table.AddCell(CellWithValue(turn, "center"));
	    }
	
	    void InsertTotalHoursPerDay(int skip, int take)
	    {		
		    _table.AddCell(CellWithValue("TOTAL HORAS", "left"));
		
		    var hoursPerDay = GetToTalHoursPerDay(skip, take);
		
		    foreach (var hourPerDay in hoursPerDay)
			    _table.AddCell(CellWithValue(hourPerDay.ToString(), "center"));
			
		    _table.AddCell(CellWithValue(hoursPerDay.Sum().ToString(), "center")); // sum total hours in week
	    }
	
	    IEnumerable<double> GetToTalHoursPerDay(int skip, int take)
	    {
		    return from turns in Schedule.Skip(skip).Take(take).Select(a => a.WeekDays)
					      from turn in turns
					      group turn by turn.Date into groupedTurns
					      select groupedTurns.Sum(a => a.Hours);
	    }
	
	    void WriteDocumentInformationProperties()
	    {
		    _document.AddTitle("Horario de " + Position);
		    _document.AddSubject(GetWeekTitle());
		    _document.AddKeywords("Horario, " + Position);
		    _document.AddCreator("Xavier Sanchez with iTextSharp");
	    }
	
	    IElement InsertDocumentHeader()
	    {
		    var header = new Paragraph();
		    header.Add(new Chunk("SUPERMERCADOS MR. SPECIAL, INC\n", new Font(_bfHelvetica, 14)));
		    header.Add(new Chunk("TIENDA DE " + StoreLocation.ToUpper() + "\n", new Font(_bfHelvetica, 12)));
		    header.Add(new Chunk("HORARIO DE ", new Font(_bfHelvetica, 14, Font.BOLD)));
		    header.Add(new Chunk(Position.ToUpper() , new Font(_bfHelvetica, 14, Font.BOLDITALIC + Font.UNDERLINE)));
		    header.Add(new Chunk(GetWeekTitle().ToUpper(), new Font(_bfHelvetica, 14, Font.BOLD)));
		    header.Alignment = Element.ALIGN_CENTER;
		
		    return header;
	    }
	
	    IElement InsertDocumentFooter()
	    {
		    var footer = new Paragraph();
		    footer.Add(new Chunk("SUJETO A CAMBIOS DE ACUERDO A LA NECESIDAD DE LA TIENDA", new Font(_bfHelvetica, 14, Font.BOLD)));
		    footer.Alignment = Element.ALIGN_CENTER;
		
		    return footer;
	    }
	
	    PdfPCell CellWithValue(string text, string align)
	    {
		    return new PdfPCell(new Phrase(text, new Font(_bfHelvetica, 9)))
		    {
			    HorizontalAlignment = align == "left" ? 0 : align == "center" ? 1 : 2, //0=Left, 1=Centre, 2=Right
			    PaddingBottom = 5f,
			    PaddingTop = 5f
		    };
	    }
	
	    public void Dispose()
	    {
		    Position = null;
		    Schedule = null;
		    Week = new NodaTime.LocalDate();
		    ReportName = null;		    
            if(_pdfWriter != null) _pdfWriter.Close();
            if(_fileStream != null) _fileStream.Close();			    
	    }
    }
}
